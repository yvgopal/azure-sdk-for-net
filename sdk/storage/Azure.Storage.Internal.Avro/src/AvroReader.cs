// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Storage.Internal.Avro
{
    internal class AvroReader
    {
        private Stream _stream;
        private BinaryReader _reader;
        private byte[] _syncMarker;
        private Dictionary<string, byte[]> _metadata;

        public AvroReader(Stream stream)
        {
            _stream = stream;
            _reader = new BinaryReader(stream);
            _metadata = new Dictionary<string, byte[]>();
        }

        public async Task Initalized(bool async)
        {
            // Get and validate initBytes
            byte[] initBytes;

            if (async)
            {
                initBytes = await _reader.ReadBytesAsync(Constants.InitBytesLength).ConfigureAwait(false);
            }
            else
            {
                initBytes = _reader.ReadBytes(Constants.InitBytesLength);
            }

            if (!initBytes.SequenceEqual(Constants.InitBytes))
            {
                throw new ArgumentException("Stream is not an Avro file.");
            }

            // Read metadata
            long length;

            if (async)
            {
                length = await _reader.ParseItemCountAsync().ConfigureAwait(false);
            }
            else
            {
                length = _reader.ParseItemCount();
            }

            if (length > 0)
            {
                if (async)
                {
                    do
                    {
                        for (int i = 0; i < length; i++)
                        {
                            string key = await _reader.ParseStringAsync().ConfigureAwait(false);
                            byte[] value = await _reader.ParseBytesAsync().ConfigureAwait(false);
                            _metadata.Add(key, value);
                        }
                        length = await _reader.ParseItemCountAsync().ConfigureAwait(false);
                    }
                    while (length != 0);
                }
                else
                {
                    do
                    {
                        for (int i = 0; i < length; i++)
                        {
                            string key = _reader.ParseString();
                            byte[] value = _reader.ParseBytes();
                            _metadata.Add(key, value);
                        }
                        length = _reader.ParseItemCount();
                    }
                    while (length != 0);
                }
            }

            // Get sync marker
            if (async)
            {
                _syncMarker = await _reader.ReadBytesAsync(Constants.SyncMarkerSize).ConfigureAwait(false);
            }
            else
            {
                _syncMarker = _reader.ReadBytes(Constants.SyncMarkerSize);
            }
        }
    }
}
