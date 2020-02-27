// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Parser = System.Func<Azure.Storage.QuickQuery.AvroParser, object>;

namespace Azure.Storage.QuickQuery
{
    internal class AvroParser
    {
        private Stream _stream;
        private AvroParser(Stream stream) => _stream = stream;
        public static List<object> Parse(Stream stream) => new AvroParser(stream).ParseObjectContainerFile();

        private byte ReadByte()
        {
            int data = _stream.ReadByte();
            if (data < 0)
                throw new InvalidOperationException("Unexpected end of input.");
            return (byte)data;
        }

        private byte[] ReadBytes(int length)
        {
            byte[] data = new byte[length];
            for (int read = 0; read < length; read = _stream.Read(data, read, length))
            { }
            return data;
        }

        // Stolen because the linked references in the Avro spec were subpar...
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
#pragma warning disable CA1822 // Can be marked as null.
        private object ParseNull() => null;
#pragma warning restore CA1822 // Can be marked as null.
        private bool ParseBool() => ReadByte() != 0;
        private long ParseLong() => ZigZag();
        private int ParseInt() => (int)ParseLong();
        private float ParseFloat() => BitConverter.ToSingle(ReadBytes(4), 0);
        private double ParseDouble() => BitConverter.ToDouble(ReadBytes(8), 0);
        private byte[] ParseBytes() => ReadBytes(ParseInt());
        private string ParseString() => Encoding.UTF8.GetString(ParseBytes());
        private Dictionary<string, T> ParseMap<T>(Func<AvroParser, T> itemParser) =>
            new Dictionary<string, T>(ParseArray(p => new KeyValuePair<string, T>(ParseString(), itemParser(p)))
                .ToDictionary(entry => entry.Key, entry => entry.Value));
        private IEnumerable<T> ParseArray<T>(Func<AvroParser, T> itemParser)
        {
            for (long length = ParseLong(); length != 0; length = ParseLong())
            {
                // Ignore block sizes because we're not skipping anything
                if (length < 0)
                { ParseLong(); length = -length; }
                while (length-- > 0)
                    yield return itemParser(this);
            }
        }

        private List<object> ParseObjectContainerFile()
        {
            // Four bytes, ASCII 'O', 'b', 'j', followed by 1.
            byte b;
            b = ReadByte();
            Debug.Assert(b == (byte)'O');
            b = ReadByte();
            Debug.Assert(b == (byte)'b');
            b = ReadByte();
            Debug.Assert(b == (byte)'j');
            b = ReadByte();
            Debug.Assert(b == (byte)1);

            // File metadata is written as if defined by the following map schema:
            // { "type": "map", "values": "bytes"}
            Dictionary<string, string> metadata = ParseMap(p => p.ParseString());
            Debug.Assert(metadata.ContainsKey("avro.codec") == false);

            // The 16-byte, randomly-generated sync marker for this file.
            byte[] syncMarker = ReadBytes(16);

            // Turn the schema into a parser
            using JsonDocument schema = JsonDocument.Parse(metadata["avro.schema"]);
            Parser parse = BuildParser(schema.RootElement);

            // File data blocks
            var data = new List<object>();
            while (_stream.Position < _stream.Length)
            {
                long length = ParseLong();
                ParseLong(); // Ignore the block size
                while (length-- > 0)
                { data.Add(parse(this)); }
                ReadBytes(16); // Ignore the sync check
            }
            return data;
        }

        private Parser BuildParser(JsonElement schema)
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
                                return (Parser)(p => p.ParseNull());
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
                                return (Parser)(p => p.ParseNull());
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
                                    var fields = new List<KeyValuePair<string, Parser>>(SelectArray(
                                        schema.GetProperty("fields"),
                                        f => new KeyValuePair<string, Parser>(
                                            f.GetProperty("name").GetString(),
                                            BuildParser(f.GetProperty("type")))));
                                    return (Parser)(p =>
                                    {
                                        Dictionary<string, object> record = new Dictionary<string, object>();
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
