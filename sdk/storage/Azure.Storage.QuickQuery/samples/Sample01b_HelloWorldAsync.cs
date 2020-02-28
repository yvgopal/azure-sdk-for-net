// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using NUnit.Framework;

namespace Azure.Storage.QuickQuery.Samples
{
    /// <summary>
    /// Basic Azure Storage Quick Query samples.
    /// </summary>
    public class Sample01b_HelloWorldAsync : QuickQuerySample
    {
        [Test]
        public async Task QueryAsync()
        {
            // Get a connection string to our Azure Storage account.
            string connectionString = ConnectionString;

            // Get a reference to a container named "sample-container" and then create it
            BlobContainerClient container = new BlobContainerClient(connectionString, Randomize("sample-container"));
            await container.CreateAsync();

            try
            {
                // Get a reference to a blob named "sample-file"
                BlockBlobClient blockBlob = container.GetBlockBlobClient(Randomize("sample-file"));

                // Get CSV stream
                Stream csvStream = CreateCsvStream(length: 1024);

                // Upload CSV stream
                await blockBlob.UploadAsync(csvStream);

                // Create BlobQuickQueryClient
                BlobQuickQueryClient queryClient = blockBlob.GetQuickQueryClient();

                // Create query
                string query = @"SELECT _2 from BlobStorage WHERE _1 > 250;";

                // Query the service
                Response<BlobDownloadInfo> response = await queryClient.QueryAsync(query);

                // Read the resulting Stream
                using StreamReader streamReader = new StreamReader(response.Value.Content);
                string s = await streamReader.ReadToEndAsync();
            }
            finally
            {
                // Clean up after the test when we're finished
                await container.DeleteAsync();
            }
        }
    }
}
