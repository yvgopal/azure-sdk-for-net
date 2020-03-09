// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using Metadata = System.Collections.Generic.IDictionary<string, string>;

namespace Azure.Storage.Blobs.Models
{
    /// <summary>
    /// Blob content to be moved by a <see cref="Specialized.BlobBaseClient"/>.
    /// </summary>
    public class BlobContent
    {
        /// <summary>
        /// Blob content stream.
        /// </summary>
        public Stream Content { get; set; }

        /// <summary>
        /// Blob metadata.
        /// </summary>
        public Metadata Metadata { get; set; }
    }
}
