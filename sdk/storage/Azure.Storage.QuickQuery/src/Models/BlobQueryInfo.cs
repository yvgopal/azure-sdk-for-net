// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Azure.Storage.Blobs.Models;

namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// BlobQueryInfo.
    /// </summary>
    public class BlobQueryInfo : IDisposable
    {
        /// <summary>
        /// Internal BlobQuickQueryResult
        /// </summary>
        internal BlobQuickQueryResult _quickQueryResult;

        /// <summary>
        /// The blob's type.
        /// </summary>
        public BlobType BlobType => _quickQueryResult.BlobType;

        /// <summary>
        /// The number of bytes present in the response body.
        /// </summary>
        public long ContentLength => _quickQueryResult.ContentLength;

        /// <summary>
        /// Content
        /// </summary>
        public Stream Content => _quickQueryResult.Body;

        /// <summary>
        /// The media type of the body of the response. For Download Blob this is 'application/octet-stream'
        /// </summary>
        public string ContentType => _quickQueryResult.ContentType;

        /// <summary>
        /// If the blob has an MD5 hash and this operation is to read the full blob, this response header is returned so that the client can check for message content integrity.
        /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] ContentHash => _quickQueryResult.BlobContentMD5;
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Disposes the BlobDownloadInfo by calling Dispose on the underlying Content stream.
        /// </summary>
        public void Dispose()
        {
            Content?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
