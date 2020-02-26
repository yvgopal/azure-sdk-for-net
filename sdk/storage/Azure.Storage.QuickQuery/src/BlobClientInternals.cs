// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using Azure.Core.Pipeline;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace Azure.Storage.QuickQuery
{
    /// <summary>
    /// Helper to access protected static members of BlobServiceClient
    /// that should not be exposed directly to customers.
    /// </summary>
    internal class BlobClientInternals : BlobBaseClient
    {
        /// <summary>
        /// Prevent instantiation.
        /// </summary>
        private BlobClientInternals() { }

        /// <summary>
        /// Get a <see cref="BlobServiceClient"/>'s <see cref="HttpPipeline"/>
        /// for creating child clients.
        /// </summary>
        /// <param name="client">The BlobBaseClient.</param>
        /// <returns>The BlobServiceClient's HttpPipeline.</returns>
        public static new HttpPipeline GetHttpPipeline(BlobBaseClient client) =>
            BlobBaseClient.GetHttpPipeline(client);


        /// <summary>
        /// Get a <see cref="BlobBaseClient"/>'s <see cref="BlobClientOptions"/>
        /// for creating child clients.
        /// </summary>
        /// <param name="client">The BlobBaseClient.</param>
        /// <returns>The BlobBaseClient's BlobClientOptions.</returns>
        public static new BlobClientOptions GetClientOptions(BlobBaseClient client) =>
            BlobBaseClient.GetClientOptions(client);

        public static new CustomerProvidedKey? GetCustomerProvidedKey(BlobBaseClient client) =>
            BlobBaseClient.GetCustomerProvidedKey(client);
    }
}
