// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncParser = System.Func<bool, Azure.Storage.Internal.Avro.AvroParser, System.Threading.Tasks.Task<object>>;
using System.Text.Json;
using Azure.Core.Pipeline;

namespace Azure.Storage.Internal.Avro
{
    internal class AvroReader
    {
        private Stream _stream;
        private AvroParser _parser;
        private AsyncParser _asyncParserFunction;
        private byte[] _syncMarker;
        private Dictionary<string, string> _metadata;
        private long _itemsRemainingInCurrentBlock;
        private bool _initalized;

        public AvroReader(Stream stream)
        {
            _stream = stream;
            _parser = new AvroParser(stream);
            _metadata = new Dictionary<string, string>();
            _initalized = false;
        }

        private async Task Initalize(bool async)
        {
            // Get and validate initBytes
            byte[] initBytes = await _parser.ReadBytes(async, AvroConstants.InitBytesLength).ConfigureAwait(false);


            if (!initBytes.SequenceEqual(AvroConstants.InitBytes))
            {
                throw new ArgumentException("Stream is not an Avro file.");
            }

            // Get metadata
            _metadata = await _parser.ParseMap<string>(async, async (async, p) =>
                await p.ParseString(async).ConfigureAwait(false)).ConfigureAwait(false);


            // Get sync marker
            _syncMarker = await _parser.ReadBytes(async, AvroConstants.SyncMarkerSize).ConfigureAwait(false);


            // Validate codec
            _metadata.TryGetValue(AvroConstants.CodecKey, out string codec);

            if (codec == AvroConstants.DeflateCodec)
            {
                throw new ArgumentException("Deflate codec is not supported");
            }

            // Build parser functions
            using JsonDocument schema = JsonDocument.Parse(_metadata[AvroConstants.SchemaKey]);

             _asyncParserFunction = _parser.BuildParser(schema.RootElement);

            // Populate _itemsRemainingInCurrentBlock
            _itemsRemainingInCurrentBlock = await _parser.ParseLong(async).ConfigureAwait(false);
            // skip block length
            await _parser.ParseLong(async).ConfigureAwait(false);
            _initalized = true;
        }

        public async Task<Dictionary<string, object>> Next(bool async)
        {
            // Initalize AvroReader, if necessary.
            if (!_initalized)
            {
                if (async)
                {
                    await Initalize(async: true).ConfigureAwait(false);
                }
                else
                {
                    Initalize(async: false).EnsureCompleted();
                }
            }

            if (!HasNext())
            {
                throw new ArgumentException("There are no more items in the stream");
            }

#pragma warning disable AZC0110 // DO NOT use await keyword in possibly synchronous scope.
            object result = await _asyncParserFunction(async, _parser).ConfigureAwait(false);
#pragma warning restore AZC0110 // DO NOT use await keyword in possibly synchronous scope.
            _itemsRemainingInCurrentBlock--;

            if (_itemsRemainingInCurrentBlock == 0)
            {
                // Skip sync marker
                await _parser.ReadBytes(async, 16).ConfigureAwait(false);

                try
                {
                    _itemsRemainingInCurrentBlock = await _parser.ParseLong(async).ConfigureAwait(false);
                }
                catch (InvalidOperationException)
                {
                    // We hit the end of the stream.
                }

                if (_itemsRemainingInCurrentBlock > 0)
                {
                    // Ignore block size
                    await _parser.ParseLong(async).ConfigureAwait(false);
                }
            }

            return (Dictionary<string, object>)result;
        }

        public bool HasNext()
        {
            if (!_initalized
                || _itemsRemainingInCurrentBlock > 0)
            {
                return true;
            }

            return false;
        }
    }
}
