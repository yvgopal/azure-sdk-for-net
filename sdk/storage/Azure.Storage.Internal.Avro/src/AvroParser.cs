// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Parser = System.Func<Azure.Storage.Internal.Avro.AvroParser, object>;
using AsyncParser = System.Func<Azure.Storage.Internal.Avro.AvroParser, System.Threading.Tasks.Task<object>>;

namespace Azure.Storage.Internal.Avro
{
    internal class AvroParser
    {
        private Stream _stream;

        public AvroParser(Stream stream)
        {
            _stream = stream;
        }

        private byte ReadByte()
        {
            int data = _stream.ReadByte();
            if (data < 0)
                throw new InvalidOperationException("Unexpected end of input.");
            return (byte)data;
        }

        private async Task<byte> ReadByteAsync()
        {
            byte[] data = new byte[1];
            await _stream.ReadAsync(data, 0, 1).ConfigureAwait(false);
            if (data[0] < 0)
                throw new InvalidOperationException("Unexpected end of input.");
            return (byte)data[0];
        }

        public byte[] ReadBytes(int length)
        {
            byte[] data = new byte[length];
            int start = 0;
            while (length > 0)
            {
                int n = _stream.Read(data, start, length);
                start += n;
                length -= n;
            }
            return data;
        }

        public async Task<byte[]> ReadBytesAsync(int length)
        {
            byte[] data = new byte[length];
            int start = 0;
            while (length > 0)
            {
                int n = await _stream.ReadAsync(data, start, length).ConfigureAwait(false);
                start += n;
                length -= n;
            }
            return data;
        }

        public Dictionary<string, T> ParseMap<T>(Func<AvroParser, T> itemParser)
            => ParseArray(p => new KeyValuePair<string, T>(ParseString(), itemParser(p))).ToDictionary(p => p.Key, p => p.Value);

        public async Task<Dictionary<string, T>> ParseMapAsync<T>(Func<AvroParser, Task<T>> itemParser)
        {
            List<KeyValuePair<string, T>> list = await ParseArrayAsync(async (p) => new KeyValuePair<string, T>(
                await ParseStringAsync().ConfigureAwait(false),
                await itemParser(p).ConfigureAwait(false))).ConfigureAwait(false);
            return list.ToDictionary(p => p.Key, p => p.Value);
        }

        public List<T> ParseArray<T>(Func<AvroParser, T> itemParser)
        {
            List<T> list = new List<T>();
            for (long length = ParseLong(); length != 0; length = ParseLong())
            {
                // Ignore block sizes because we're not skipping anything
                if (length < 0)
                { ParseLong(); length = -length; }
                while (length-- > 0)
                    list.Add(itemParser(this));
            }
            return list;
        }

        public async Task<List<T>> ParseArrayAsync<T>(Func<AvroParser, Task<T>> itemParser)
        {
            List<T> list = new List<T>();
            for (long length = await ParseLongAsync().ConfigureAwait(false); length != 0; length = await ParseLongAsync().ConfigureAwait(false))
            {
                // Ignore block sizes because we're not skipping anything
                if (length < 0)
                { ParseLong(); length = -length; }
                while (length-- > 0)
                    list.Add(await itemParser(this).ConfigureAwait(false));
            }
            return list;
        }

        public long ParseItemCount()
        {
            long count = ParseLong();
            if (count < 0)
            {
                ParseLong();
                count = -count;
            }
            return count;
        }

        public async Task<long> ParseItemCountAsync()
        {
            long count = await ParseLongAsync().ConfigureAwait(false);
            if (count < 0)
            {
                await ParseLongAsync().ConfigureAwait(false);
                count = -count;
            }
            return count;
        }

        private long ZigZag()
        {
            byte b = ReadByte();
            ulong next = b & 0x7FUL;
            int shift = 7;
            while ((b & 0x80) != 0)
            {
                b = ReadByte();
                next |= (b & 0x7FUL) << shift;
                shift += 7;
            }
            long value = (long)next;
            return (-(value & 0x01L)) ^ ((value >> 1) & 0x7fffffffffffffffL);
        }

        private async Task<long> ZigZagAsync()
        {
            byte b = await ReadByteAsync().ConfigureAwait(false);
            ulong next = b & 0x7FUL;
            int shift = 7;
            while ((b & 0x80) != 0)
            {
                b = await ReadByteAsync().ConfigureAwait(false);
                next |= (b & 0x7FUL) << shift;
                shift += 7;
            }
            long value = (long)next;
            return (-(value & 0x01L)) ^ ((value >> 1) & 0x7fffffffffffffffL);
        }

        private static object ParseNull() => null;

        private static Task<object> ParseNullAsync()
            => Task.FromResult(ParseNull());

        private bool ParseBool() => ReadByte() != 0;

        private async Task<bool> ParseBoolAsync()
        {
            byte data = await ReadByteAsync().ConfigureAwait(false);
            return data != 0;
        }

        public long ParseLong() => ZigZag();

        public async Task<long> ParseLongAsync()
            => await ZigZagAsync().ConfigureAwait(false);

        private int ParseInt() => (int)ParseLong();

        private async Task<int> ParseIntAsync()
        {
            long data = await ParseLongAsync().ConfigureAwait(false);
            return (int)data;
        }

        private float ParseFloat() => BitConverter.ToSingle(ReadBytes(4), 0);

        private async Task<float> ParseFloatAsync()
        {
            byte[] data = await ReadBytesAsync(4).ConfigureAwait(false);
            return BitConverter.ToSingle(data, 0);
        }

        private double ParseDouble() => BitConverter.ToDouble(ReadBytes(8), 0);

        private async Task<double> ParseDoubleAsync()
        {
            byte[] data = await ReadBytesAsync(8).ConfigureAwait(false);
            return BitConverter.ToDouble(data, 0);
        }

        public byte[] ParseBytes() => ReadBytes(ParseInt());

        public async Task<byte[]> ParseBytesAsync()
        {
            int length = await ParseIntAsync().ConfigureAwait(false);
            return await ReadBytesAsync(length).ConfigureAwait(false);
        }

        public string ParseString()
        {
            int length = ParseInt();
            byte[] bytes = ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        public async Task<string> ParseStringAsync()
        {
            int length = await ParseIntAsync().ConfigureAwait(false);
            byte[] bytes = await ReadBytesAsync(length).ConfigureAwait(false);
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
                                return (Parser)(p => ParseNull());
                            case "boolean":
                                return (Parser)(p => p.ParseBool());
                            case "int":
                                return (Parser)(p => p.ParseInt());
                            case "long":
                                return (Parser)(p => p.ParseLong());
                            case "float":
                                return (Parser)(p => p.ParseFloat());
                            case "double":
                                return (Parser)(p => p.ParseDouble());
                            case "bytes":
                                return (Parser)(p => p.ParseBytes());
                            case "string":
                                return (Parser)(p => p.ParseString());
                            default:
                                throw new InvalidOperationException($"Unexpected Avro type {type} in {schema}");
                        }
                    }
                // Union types
                case JsonValueKind.Array:
                    {
                        List<Parser> parsers = SelectArray(schema, BuildParser);
                        return (Parser)(p => parsers[p.ParseInt()](p));
                    }
                // Everything else
                case JsonValueKind.Object:
                    {
                        string type = schema.GetProperty("type").GetString();
                        switch (type)
                        {
                            // Primitives can be defined as strings or objects
                            case "null":
                                return (Parser)(p => ParseNull());
                            case "boolean":
                                return (Parser)(p => p.ParseBool());
                            case "int":
                                return (Parser)(p => p.ParseInt());
                            case "long":
                                return (Parser)(p => p.ParseLong());
                            case "float":
                                return (Parser)(p => p.ParseFloat());
                            case "double":
                                return (Parser)(p => p.ParseDouble());
                            case "bytes":
                                return (Parser)(p => p.ParseBytes());
                            case "string":
                                return (Parser)(p => p.ParseString());
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
                                    return (Parser)(p =>
                                    {
                                        Dictionary<string, object> record = new Dictionary<string, object>();
                                        record["$schemaName"] = name;
                                        foreach (KeyValuePair<string, Parser> field in fields)
                                        {
                                            record[field.Key] = field.Value(p);
                                        }
                                        return record;
                                    });
                                }
                            case "enum":
                                {
                                    if (schema.TryGetProperty("aliases", out var _))
                                        throw new InvalidOperationException($"Unexpected aliases on {schema}");
                                    List<string> symbols = SelectArray(schema.GetProperty("symbols"), s => s.GetString());
                                    return (Parser)(p => symbols[p.ParseInt()]);
                                }
                            case "map":
                                {
                                    Parser values = BuildParser(schema.GetProperty("values"));
                                    return (Parser)(p => p.ParseMap(values));
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

        public AsyncParser BuildAsyncParser(JsonElement schema)
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
                                return (AsyncParser)(async (p) => await ParseNullAsync().ConfigureAwait(false));
                            case "boolean":
                                return (AsyncParser)(async (p) => await p.ParseBoolAsync().ConfigureAwait(false));
                            case "int":
                                return (AsyncParser)(async (p) => await p.ParseIntAsync().ConfigureAwait(false));
                            case "long":
                                return (AsyncParser)(async (p) => await p.ParseLongAsync().ConfigureAwait(false));
                            case "float":
                                return (AsyncParser)(async (p) => await p.ParseFloatAsync().ConfigureAwait(false));
                            case "double":
                                return (AsyncParser)(async (p) => await p.ParseDoubleAsync().ConfigureAwait(false));
                            case "bytes":
                                return (AsyncParser)(async (p) => await p.ParseBytesAsync().ConfigureAwait(false));
                            case "string":
                                return (AsyncParser)(async (p) => await p.ParseStringAsync().ConfigureAwait(false));
                            default:
                                throw new InvalidOperationException($"Unexpected Avro type {type} in {schema}");
                        }
                    }
                // Union types
                case JsonValueKind.Array:
                    {
                        List<Parser> parsers = SelectArray(schema, BuildParser);
                        return (AsyncParser)(async (p) => parsers[await p.ParseIntAsync().ConfigureAwait(false)](p));
                    }
                // Everything else
                case JsonValueKind.Object:
                    {
                        string type = schema.GetProperty("type").GetString();
                        switch (type)
                        {
                            // Primitives can be defined as strings or objects
                            case "null":
                                return (AsyncParser)(async (p) => await ParseNullAsync().ConfigureAwait(false));
                            case "boolean":
                                return (AsyncParser)(async (p) => await p.ParseBoolAsync().ConfigureAwait(false));
                            case "int":
                                return (AsyncParser)(async (p) => await p.ParseIntAsync().ConfigureAwait(false));
                            case "long":
                                return (AsyncParser)(async (p) => await p.ParseLongAsync().ConfigureAwait(false));
                            case "float":
                                return (AsyncParser)(async (p) => await p.ParseFloatAsync().ConfigureAwait(false));
                            case "double":
                                return (AsyncParser)(async (p) => await p.ParseDoubleAsync().ConfigureAwait(false));
                            case "bytes":
                                return (AsyncParser)(async (p) => await p.ParseBytesAsync().ConfigureAwait(false));
                            case "string":
                                return (AsyncParser)(async (p) => await p.ParseStringAsync().ConfigureAwait(false));
                            case "record":
                                {
                                    if (schema.TryGetProperty("aliases", out var _))
                                        throw new InvalidOperationException($"Unexpected aliases on {schema}");
                                    var fields = SelectArray(
                                        schema.GetProperty("fields"),
                                        f => new KeyValuePair<string, Parser>(
                                            f.GetProperty("name").GetString(),
                                            BuildParser(f.GetProperty("type")))).ToDictionary(p => p.Key, p => p.Value);
                                    return (AsyncParser)(p =>
                                    {
                                        Dictionary<string, object> record = new Dictionary<string, object>();
                                        foreach (KeyValuePair<string, Parser> field in fields)
                                        {
                                            record[field.Key] = field.Value(p);
                                        }
                                        return Task.FromResult((object)record);
                                    });
                                }
                            case "enum":
                                {
                                    if (schema.TryGetProperty("aliases", out var _))
                                        throw new InvalidOperationException($"Unexpected aliases on {schema}");
                                    List<string> symbols = SelectArray(schema.GetProperty("symbols"), s => s.GetString());
                                    return (AsyncParser)(async (p) => symbols[await p.ParseIntAsync().ConfigureAwait(false)]);
                                }
                            case "map":
                                {
                                    AsyncParser values = BuildAsyncParser(schema.GetProperty("values"));
                                    return (AsyncParser)(async (p) => await p.ParseMapAsync(values).ConfigureAwait(false));
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
