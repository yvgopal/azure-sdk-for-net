// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Storage.Internal.Avro
{
    internal class Constants
    {
        public const int SyncMarkerSize = 16;
        public const int InitBytesLength = 4;
        public static byte[] InitBytes =
        {
            (byte)'O',
            (byte)'b',
            (byte)'j',
            (byte)1
        };
    }
}
