// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;

namespace Azure.Storage.Internal.Avro
{
    internal class AvroReader
    {
        private Stream _stream;
        private byte[] _syncMarker;
        private Dictionary<string, string> _metadata;
        private AvroType _itemType;
        private long _itemsRemainingInCurrentBlock;
        private bool _initalized;

        public AvroReader(Stream stream)
        {
            _stream = stream;
            _metadata = new Dictionary<string, string>();
            _initalized = false;
        }

        private async Task Initalize(bool async, CancellationToken cancellationToken = default)
        {
            // Four bytes, ASCII 'O', 'b', 'j', followed by 1.
            byte[] header = await AvroParser.ReadFixedBytesAsync(_stream, AvroConstants.InitBytes.Length, async, cancellationToken).ConfigureAwait(false);
            if (!header.SequenceEqual(AvroConstants.InitBytes))
            {
                throw new ArgumentException("Stream is not an Avro file.");
            }

            // File metadata is written as if defined by the following map schema:
            // { "type": "map", "values": "bytes"}
            _metadata = await AvroParser.ReadMapAsync(_stream, AvroParser.ReadStringAsync, async, cancellationToken).ConfigureAwait(false);

            // Validate codec
            _metadata.TryGetValue(AvroConstants.CodecKey, out string codec);
            if (codec == AvroConstants.DeflateCodec)
            {
                throw new ArgumentException("Deflate codec is not supported");
            }

            // The 16-byte, randomly-generated sync marker for this file.
            _syncMarker = await AvroParser.ReadFixedBytesAsync(_stream, AvroConstants.SyncMarkerSize, async, cancellationToken).ConfigureAwait(false);

            // Parse the schema
            using JsonDocument schema = JsonDocument.Parse(_metadata[AvroConstants.SchemaKey]);
            _itemType = AvroType.FromSchema(schema.RootElement);

            // Populate _itemsRemainingInCurrentBlock
            _itemsRemainingInCurrentBlock = await AvroParser.ReadLongAsync(_stream, async, cancellationToken).ConfigureAwait(false);

            // skip block length
            await AvroParser.ReadLongAsync(_stream, async, cancellationToken).ConfigureAwait(false);

            _initalized = true;
        }

        public bool HasNext() => !_initalized || _itemsRemainingInCurrentBlock > 0;

        public async Task<Dictionary<string, object>> Next(bool async, CancellationToken cancellationToken = default)
        {
            // Initialize AvroReader, if necessary.
            if (!_initalized)
            {
                await Initalize(async, cancellationToken).ConfigureAwait(false);
            }

            if (!HasNext())
            {
                throw new ArgumentException("There are no more items in the stream");
            }


            object result = await _itemType.ReadAsync(_stream, async, cancellationToken).ConfigureAwait(false);

            _itemsRemainingInCurrentBlock--;

            if (_itemsRemainingInCurrentBlock == 0)
            {
                byte[] marker = await AvroParser.ReadFixedBytesAsync(_stream, 16, async, cancellationToken).ConfigureAwait(false);
                if (!_syncMarker.SequenceEqual(marker))
                {
                    throw new ArgumentException("Stream is not a valid Avro file.");
                }

                try
                {
                    _itemsRemainingInCurrentBlock = await AvroParser.ReadLongAsync(_stream, async, cancellationToken).ConfigureAwait(false);
                }
                catch (InvalidOperationException)
                {
                    // We hit the end of the stream.
                }

                if (_itemsRemainingInCurrentBlock > 0)
                {
                    // Ignore block size
                    await AvroParser.ReadLongAsync(_stream, async, cancellationToken).ConfigureAwait(false);
                }
            }

            return (Dictionary<string, object>)result;
        }
    }
}
