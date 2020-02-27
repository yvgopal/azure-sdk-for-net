// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avro.File;
using Avro.Generic;
using Azure.Storage.QuickQuery.Models;

namespace Azure.Storage.QuickQuery
{
    /// <summary>
    /// QuickQueryStream
    /// </summary>
    internal class BlobQuickQueryStream : Stream
    {
        internal Stream _avroStream;
        internal IFileReader<GenericRecord> _avroReader;
        internal byte[] _buffer;
        internal int _bufferOffset;
        internal int _bufferLength;
        internal IProgress<long> _ProgressHandler;
        internal IBlobQueryErrorReceiver _nonFatalErrorHandler;

        public BlobQuickQueryStream(
            Stream avroStream,
            IProgress<long> progressHandler = default,
            IBlobQueryErrorReceiver nonFatalErrorHandler = default)
        {
            _avroStream = avroStream;
            _avroReader = DataFileReader<GenericRecord>.OpenReader(_avroStream);
            //TODO may need to revisit this.
            _buffer = new byte[4 * Constants.MB];
            _bufferOffset = 0;
            _bufferLength = 0;
            _ProgressHandler = progressHandler;
            _nonFatalErrorHandler = nonFatalErrorHandler;
        }

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
        public override long Length => throw new NotSupportedException();
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations

        /// <inheritdoc/>
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations

        /// <inheritdoc/>
        public override void Flush() => throw new NotSupportedException();

        /// <inheritdoc/>
        // Note - offest is with respect to buffer.
        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateReadParameters(buffer, offset, count);

            int remainingBytes = _bufferLength - _bufferOffset;

            // We have enough bytes in the buffer and don't need to read the next Record.
            if (count <= remainingBytes)
            {
                Array.Copy(
                    sourceArray: _buffer,
                    sourceIndex: _bufferOffset,
                    destinationArray: buffer,
                    destinationIndex: offset,
                    length: count);
                _bufferOffset += count;
                return count;
            }

            // Copy remaining buffer
            if (remainingBytes > 0)
            {
                Array.Copy(
                    sourceArray: _buffer,
                    sourceIndex: _bufferOffset,
                    destinationArray: buffer,
                    destinationIndex: offset,
                    length: remainingBytes);
            }

            // Decrement count to reflect how maybe bytes we'll still try to copy.
            count -= remainingBytes;

            // Reset _bufferOffset, _bufferLength, and remainingBytes
            _bufferOffset = 0;
            _bufferLength = 0;

            // We've caught up to the end of the _avroStream, but it isn't necessarly the end of the stream.
            // TODO what to do in this case?  If we return 0, we are indicating the end of stream
            if (!_avroReader.HasNext())
            {
                return 0;
            }

            // Get next Record.
            //TODO in the future, this is where we will call the async version of this.
            GenericRecord record = _avroReader.Next();

            switch (record.Schema.Fullname)
            {
                // Data Record
                case Constants.QuickQuery.DataRecordName:
                    record.TryGetValue(Constants.QuickQuery.Data, out object byteObject);
                    byte[] bytes = (byte[])byteObject;
                    Array.Copy(
                        sourceArray: bytes,
                        sourceIndex: 0,
                        destinationArray: _buffer,
                        destinationIndex: 0,
                        length: bytes.Length);

                    _bufferLength = bytes.Length;

                    // Don't remove this reset, it is used in the final array copy below.
                    remainingBytes = bytes.Length;
                    break;

                // Progress Record
                case Constants.QuickQuery.ProgressRecordName:
                    if (_ProgressHandler != default)
                    {
                        record.TryGetValue(Constants.QuickQuery.BytesScanned, out object progress);
                        _ProgressHandler.Report((long)progress);
                    }
                    break;

                // Error Record
                case Constants.QuickQuery.ErrorRecordName:
                    ProcessErrorRecord(record);
                    break;

                // End Record
                case Constants.QuickQuery.EndRecordName:
                    return 0;
            }

            int length = Math.Min(count, remainingBytes);
            Array.Copy(
                sourceArray: _buffer,
                sourceIndex: _bufferOffset,
                destinationArray: buffer,
                destinationIndex: offset,
                length: length);

            _bufferOffset += length;
            return length;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            _avroStream.Dispose();
            _avroReader.Dispose();
        }

        internal static void ValidateReadParameters(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException($"{nameof(buffer)} cannot be null.");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(offset)} cannot be negative.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(count)} cannot be negative.");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentException($"The sum of {nameof(offset)} and {nameof(count)} cannot be greater than {nameof(buffer)} length.");
            }
        }

        internal void ProcessErrorRecord(GenericRecord record)
        {
            record.TryGetValue(Constants.QuickQuery.Fatal, out object fatal);
            record.TryGetValue(Constants.QuickQuery.Name, out object name);
            record.TryGetValue(Constants.QuickQuery.Description, out object description);
            record.TryGetValue(Constants.QuickQuery.Position, out object position);

            if ((bool)fatal)
            {
                throw new RequestFailedException(
                    $"Fatal Error\nName: {(string)name}\nDescription: {(string)description}\nPosition: {(long)position}");
            }
            else
            {
                if (_nonFatalErrorHandler != null)
                {
                    BlobQueryError blobQueryError = new BlobQueryError
                    {
                        IsFatal = false,
                        Name = (string)name,
                        Description = (string)description,
                        Position = (long)position
                    };
                    _nonFatalErrorHandler.ReportError(blobQueryError);
                }
            }
        }
    }
}
