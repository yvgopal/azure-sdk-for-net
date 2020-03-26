// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Parser = System.Func<bool, Azure.Storage.Internal.Avro.AvroParser, System.Threading.Tasks.Task<object>>;

namespace Azure.Storage.Internal.Avro
{
    internal class AvroParser
    {
        private Stream _stream;

        public AvroParser(Stream stream)
        {
            _stream = stream;
        }

        private async Task<byte> ReadByte(bool async)
        {
            byte[] data = new byte[1];
            if (async)
            {
                await _stream.ReadAsync(data, 0, 1).ConfigureAwait(false);
            }
            else
            {
                _stream.Read(data, 0, 1);
            }

            if (data[0] < 0)
                throw new InvalidOperationException("Unexpected end of input.");
            return (byte)data[0];
        }

        public async Task<byte[]> ReadBytes(bool async, int length)
        {
            byte[] data = new byte[length];
            int start = 0;
            while (length > 0)
            {
                int n;
                if (async)
                {
                    n = await _stream.ReadAsync(data, start, length).ConfigureAwait(false);
                }
                else
                {
                    n = _stream.Read(data, start, length);
                }

                start += n;
                length -= n;
            }
            return data;
        }

        public async Task<Dictionary<string, T>> ParseMap<T>(bool async, Func<bool, AvroParser, Task<T>> itemParser)
        {
            List<KeyValuePair<string, T>> list = await ParseArray(async, async (async, p) => new KeyValuePair<string, T>(
                await ParseString(async).ConfigureAwait(false),
                await itemParser(async, p).ConfigureAwait(false))).ConfigureAwait(false);
            return list.ToDictionary(p => p.Key, p => p.Value);
        }

        public async Task<List<T>> ParseArray<T>(bool async, Func<bool, AvroParser, Task<T>> itemParser)
        {
            List<T> list = new List<T>();
            for (long length = await ParseLong(async).ConfigureAwait(false); length != 0; length = await ParseLong(async).ConfigureAwait(false))
            {
                // Ignore block sizes because we're not skipping anything
                if (length < 0)
                { await ParseLong(async).ConfigureAwait(false); length = -length; }
                while (length-- > 0)
#pragma warning disable AZC0110 // DO NOT use await keyword in possibly synchronous scope.
                    list.Add(await itemParser(async, this).ConfigureAwait(false));
#pragma warning restore AZC0110 // DO NOT use await keyword in possibly synchronous scope.
            }
            return list;
        }

        private async Task<long> ZigZag(bool async)
        {
            byte b = await ReadByte(async).ConfigureAwait(false);
            ulong next = b & 0x7FUL;
            int shift = 7;
            while ((b & 0x80) != 0)
            {
                b = await ReadByte(async).ConfigureAwait(false);
                next |= (b & 0x7FUL) << shift;
                shift += 7;
            }
            long value = (long)next;
            return (-(value & 0x01L)) ^ ((value >> 1) & 0x7fffffffffffffffL);
        }

        private static Task<object> ParseNull(bool async)
        {
            // I know this is dumb.  I had to use the async parameter, or I couldn't build,
            // even after suppressing the rule.
            if (async)
            {
                return Task.FromResult((object)null);
            }
            else
            {
                return Task.FromResult((object)null);
            }
        }

        private async Task<bool> ParseBool(bool async)
        {
            byte data = await ReadByte(async).ConfigureAwait(false);
            return data != 0;
        }

        public async Task<long> ParseLong(bool async)
            => await ZigZag(async).ConfigureAwait(false);

        private async Task<int> ParseInt(bool async)
        {
            long data = await ParseLong(async).ConfigureAwait(false);
            return (int)data;
        }

        private async Task<float> ParseFloat(bool async)
        {
            byte[] data = await ReadBytes(async, 4).ConfigureAwait(false);
            return BitConverter.ToSingle(data, 0);
        }

        private async Task<double> ParseDouble(bool async)
        {
            byte[] data = await ReadBytes(async, 8).ConfigureAwait(false);
            return BitConverter.ToDouble(data, 0);
        }

        public async Task<byte[]> ParseBytes(bool async)
        {
            int length = await ParseInt(async).ConfigureAwait(false);
            return await ReadBytes(async, length).ConfigureAwait(false);
        }

        public async Task<string> ParseString(bool async)
        {
            int length = await ParseInt(async).ConfigureAwait(false);
            byte[] bytes = await ReadBytes(async, length).ConfigureAwait(false);
            return Encoding.UTF8.GetString(bytes);
        }

        public Parser BuildParser(JsonElement schema)
        {
            switch (schema.ValueKind)
            {
                // Primitives
                case JsonValueKind.String:
                    {
                        string type = schema.GetString();
                        switch (type)
                        {
                            case "null":
                                return (Parser)(async (async, p) => await ParseNull(async).ConfigureAwait(false));
                            case "boolean":
                                return (Parser)(async (async, p) => await p.ParseBool(async).ConfigureAwait(false));
                            case "int":
                                return (Parser)(async (async, p) => await p.ParseInt(async).ConfigureAwait(false));
                            case "long":
                                return (Parser)(async (async, p) => await p.ParseLong(async).ConfigureAwait(false));
                            case "float":
                                return (Parser)(async (async, p) => await p.ParseFloat(async).ConfigureAwait(false));
                            case "double":
                                return (Parser)(async (async, p) => await p.ParseDouble(async).ConfigureAwait(false));
                            case "bytes":
                                return (Parser)(async (async, p) => await p.ParseBytes(async).ConfigureAwait(false));
                            case "string":
                                return (Parser)(async (async, p) => await p.ParseString(async).ConfigureAwait(false));
                            default:
                                throw new InvalidOperationException($"Unexpected Avro type {type} in {schema}");
                        }
                    }
                // Union types
                case JsonValueKind.Array:
                    {
                        List<Parser> parsers = SelectArray(schema, BuildParser);
#pragma warning disable AZC0109 // Misuse of 'async' parameter.
                        return (Parser)(async (async, p) => parsers[await p.ParseInt(async).ConfigureAwait(false)](async, p));
#pragma warning restore AZC0109 // Misuse of 'async' parameter.
                    }
                // Everything else
                case JsonValueKind.Object:
                    {
                        string type = schema.GetProperty("type").GetString();
                        switch (type)
                        {
                            // Primitives can be defined as strings or objects
                            case "null":
                                return (Parser)(async (async, p) => await ParseNull(async).ConfigureAwait(false));
                            case "boolean":
                                return (Parser)(async (async, p) => await p.ParseBool(async).ConfigureAwait(false));
                            case "int":
                                return (Parser)(async (async, p) => await p.ParseInt(async).ConfigureAwait(false));
                            case "long":
                                return (Parser)(async (async, p) => await p.ParseLong(async).ConfigureAwait(false));
                            case "float":
                                return (Parser)(async (async, p) => await p.ParseFloat(async).ConfigureAwait(false));
                            case "double":
                                return (Parser)(async (async, p) => await p.ParseDouble(async).ConfigureAwait(false));
                            case "bytes":
                                return (Parser)(async (async, p) => await p.ParseBytes(async).ConfigureAwait(false));
                            case "string":
                                return (Parser)(async (async, p) => await p.ParseString(async).ConfigureAwait(false));
                            case "record":
                                {
                                    if (schema.TryGetProperty("aliases", out var _))
                                        throw new InvalidOperationException($"Unexpected aliases on {schema}");
                                    var fields = SelectArray(
                                        schema.GetProperty("fields"),
                                        f => new KeyValuePair<string, Parser>(
                                            f.GetProperty("name").GetString(),
                                            BuildParser(f.GetProperty("type")))).ToDictionary(p => p.Key, p => p.Value);
                                    var name = schema.GetProperty("name").GetString();
                                    return (Parser)((async, p) =>
                                    {
                                        Dictionary<string, object> record = new Dictionary<string, object>();
                                        record["$schemaName"] = name;
                                        foreach (KeyValuePair<string, Parser> field in fields)
                                        {
#pragma warning disable AZC0109 // Misuse of 'async' parameter.
                                            record[field.Key] = field.Value(async, p);
#pragma warning restore AZC0109 // Misuse of 'async' parameter.
                                        }
                                        return Task.FromResult((object)record);
                                    });
                                }
                            case "enum":
                                {
                                    if (schema.TryGetProperty("aliases", out var _))
                                        throw new InvalidOperationException($"Unexpected aliases on {schema}");
                                    List<string> symbols = SelectArray(schema.GetProperty("symbols"), s => s.GetString());
                                    return (Parser)(async (async, p) => symbols[await p.ParseInt(async).ConfigureAwait(false)]);
                                }
                            case "map":
                                {
                                    Parser values = BuildParser(schema.GetProperty("values"));
                                    return (Parser)(async (async, p) => await p.ParseMap(async, values).ConfigureAwait(false));
                                }
                            case "array": // Unused today
                            case "union": // Unused today
                            case "fixed": // Unused today
                            default:
                                throw new InvalidOperationException($"Unexpected Avro type {type} in {schema}");
                        }
                    }
                default:
                    throw new InvalidOperationException($"Unexpected JSON Element: {schema}");
            }
        }

        private static List<T> SelectArray<T>(JsonElement array, Func<JsonElement, T> selector)
        {
            var values = new List<T>();
            foreach (JsonElement element in array.EnumerateArray())
            {
                values.Add(selector(element));
            }
            return values;
        }
    }
}
