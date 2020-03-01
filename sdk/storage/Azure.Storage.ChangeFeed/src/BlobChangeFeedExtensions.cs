// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Blobs;

namespace Azure.Storage.ChangeFeed
{
    /// <summary>
    /// BlobChangeFeedExtensions.
    /// </summary>
    public static class BlobChangeFeedExtensions
    {
        /// <summary>
        /// GetChangeFeedClient.
        /// </summary>
        /// <param name="serviceClient"></param>
        /// <returns><see cref="BlobChangeFeedClient"/>.</returns>
        public static BlobChangeFeedClient GetChangeFeedClient(this BlobServiceClient serviceClient)
        {
            return new BlobChangeFeedClient(serviceClient);
        }
    }
}
