// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Storage.Blobs.Models
{
    /// <summary>
    /// Algorithms available for hashing blob data.
    /// </summary>
    public enum BlobHashAlgorithm
    {
        /// <summary>
        /// Azure Storage custom CRC64 algorithm.
        /// </summary>
        Crc64 = 0,

        /// <summary>
        /// MD5 algorithm.
        /// </summary>
        Md5 = 1
    }
}
