// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Storage.Internal.Avro
{
    internal class BinaryReader
    {
        private Stream _stream;

        public BinaryReader(Stream stream)
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
            for (int read = 0; read < length; read = _stream.Read(data, read, length))
            { }
            return data;
        }

        public async Task<byte[]> ReadBytesAsync(int length)
        {
            byte[] data = new byte[length];
            for (int read = 0; read < length; read = await _stream.ReadAsync(data, read, length).ConfigureAwait(false))
            { }
            return data;
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

        private object ParseNull() => null;

        private Task<object> ParseNullAsync()
            => Task.FromResult(ParseNull());

        private bool ParseBool() => ReadByte() != 0;

        private async Task<bool> ParseBoolAsync()
        {
            byte data = await ReadByteAsync().ConfigureAwait(false);
            return data != 0;
        }

        private long ParseLong() => ZigZag();

        private async Task<long> ParseLongAsync()
            => await ZigZagAsync().ConfigureAwait(false);

        private int ParseInt() => (int)ParseLong();

        private async Task<int> ParseIntAsync()
        {
            long data = await ParseLongAsync().ConfigureAwait(false);
            return (int)data;
        }

        private float ParseFloat() => BitConverter.ToSingle(ReadBytes(4), 0);

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

        public string ParseString() => Encoding.UTF8.GetString(ParseBytes());

        public async Task<string> ParseStringAsync()
        {
            byte[] bytes = await ParseBytesAsync().ConfigureAwait(false);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
