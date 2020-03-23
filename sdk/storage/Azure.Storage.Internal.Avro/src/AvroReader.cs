// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser = System.Func<Azure.Storage.Internal.Avro.AvroParser, object>;
using AsyncParser = System.Func<Azure.Storage.Internal.Avro.AvroParser, System.Threading.Tasks.Task<object>>;
using System.Text.Json;
using Azure.Core.Pipeline;

namespace Azure.Storage.Internal.Avro
{
    internal class AvroReader
    {
        private Stream _stream;
        private AvroParser _parser;
        private Parser _parserFunction;
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
            byte[] initBytes;

            if (async)
            {
                initBytes = await _parser.ReadBytesAsync(AvroConstants.InitBytesLength).ConfigureAwait(false);
            }
            else
            {
                initBytes = _parser.ReadBytes(AvroConstants.InitBytesLength);
            }

            if (!initBytes.SequenceEqual(AvroConstants.InitBytes))
            {
                throw new ArgumentException("Stream is not an Avro file.");
            }

            // Get metadata
            if (async)
            {
                _metadata = await _parser.ParseMapAsync<string>(async (p) =>
                    await p.ParseStringAsync().ConfigureAwait(false)).ConfigureAwait(false);
            }
            else
            {
                _metadata = _parser.ParseMap<string>(p => p.ParseString());
            }

            // Get sync marker
            if (async)
            {
                _syncMarker = await _parser.ReadBytesAsync(AvroConstants.SyncMarkerSize).ConfigureAwait(false);
            }
            else
            {
                _syncMarker = _parser.ReadBytes(AvroConstants.SyncMarkerSize);
            }

            // Validate codec
            _metadata.TryGetValue(AvroConstants.CodecKey, out string codec);

            if (codec == AvroConstants.DeflateCodec)
            {
                throw new ArgumentException("Deflate codec is not supported");
            }

            // Build parser functions
            using JsonDocument schema = JsonDocument.Parse(_metadata[AvroConstants.SchemaKey]);

            if (async)
            {
                _asyncParserFunction = _parser.BuildAsyncParser(schema.RootElement);
            }
            else
            {
                _parserFunction = _parser.BuildParser(schema.RootElement);
            }

            // Populate _itemsRemainingInCurrentBlock
            if (async)
            {
                _itemsRemainingInCurrentBlock = await _parser.ParseLongAsync().ConfigureAwait(false);
                // skip block length
                await _parser.ParseLongAsync().ConfigureAwait(false);
            }
            else
            {
                _itemsRemainingInCurrentBlock = _parser.ParseLong();
                // skip block length
                _parser.ParseLong();
            }
            _initalized = true;
        }

        public async Task<object> Next(bool async)
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

            object result;

            if (async)
            {
                result = _asyncParserFunction(_parser);
                _itemsRemainingInCurrentBlock--;

                if (_itemsRemainingInCurrentBlock == 0)
                {
                    // Skip sync marker
                    await _parser.ReadBytesAsync(16).ConfigureAwait(false);

                    if (_stream.Position < _stream.Length)
                    {
                        _itemsRemainingInCurrentBlock = await _parser.ParseLongAsync().ConfigureAwait(false);
                        // Ignore block size
                        await _parser.ParseLongAsync().ConfigureAwait(false);
                    }
                }
            }
            else
            {
                result = _parserFunction(_parser);
                _itemsRemainingInCurrentBlock--;

                if (_itemsRemainingInCurrentBlock == 0)
                {
                    // Skip sync marker
                    _parser.ReadBytes(16);

                    if (_stream.Position < _stream.Length)
                    {
                        _itemsRemainingInCurrentBlock = _parser.ParseLong();
                        // Ignore block size
                        _parser.ParseLong();
                    }
                }
            }

            return result;
        }

        public bool HasNext()
        {
            if (!_initalized)
            {
                return true;
            }

            if (_itemsRemainingInCurrentBlock > 0)
            {
                return true;
            }

            // TODO this might not work.
            return _stream.Position < _stream.Length;
        }
    }
}
