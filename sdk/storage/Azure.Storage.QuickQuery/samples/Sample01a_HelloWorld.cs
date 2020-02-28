// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using NUnit.Framework;

namespace Azure.Storage.QuickQuery.Samples
{
    /// <summary>
    /// Basic Azure QuickQuery Storage samples.
    /// </summary>
    public class Sample01a_HelloWorld : QuickQuerySample
    {
        [Test]
        public void Query()
        {
            // Get a connection string to our Azure Storage account.
            string connectionString = ConnectionString;

            // Get a reference to a container named "sample-container" and then create it
            BlobContainerClient container = new BlobContainerClient(connectionString, Randomize("sample-container"));
            container.Create();

            try
            {
                // Get a reference to a blob named "sample-file"
                BlockBlobClient blockBlob = container.GetBlockBlobClient(Randomize("sample-file"));

                // Get CSV stream
                Stream csvStream = CreateCsvStream(length: 1024);

                // Upload CSV stream
                blockBlob.Upload(csvStream);

                // Create BlobQuickQueryClient
                BlobQuickQueryClient queryClient = blockBlob.GetQuickQueryClient();

                // Create query
                string query = @"SELECT _2 from BlobStorage WHERE _1 > 250;";

                // Query the service
                Response<BlobDownloadInfo> response = queryClient.Query(query);

                // Read the resulting Stream
                using StreamReader streamReader = new StreamReader(response.Value.Content);
                string s = streamReader.ReadToEnd();
            }
            finally
            {
                // Clean up after the test when we're finished
                container.Delete();
            }
        }
    }
}
