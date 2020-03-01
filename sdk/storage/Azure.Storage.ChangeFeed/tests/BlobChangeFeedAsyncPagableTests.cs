// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avro.Generic;
using Azure.Storage.Blobs;
using Azure.Storage.ChangeFeed.Models;
using NUnit.Framework;

namespace Azure.Storage.ChangeFeed.Tests
{
    public class BlobChangeFeedAsyncPagableTests : ChangeFeedTestBase
    {
        public BlobChangeFeedAsyncPagableTests(bool async)
            : base(async, null /* RecordedTestMode.Record /* to re-record */)
        {
        }

        [Test]
        public async Task Test()
        {
            BlobServiceClient service = GetServiceClient_SharedKey();
            BlobChangeFeedClient blobChangeFeedClient = service.GetChangeFeedClient();
            BlobChangeFeedAsyncPagable blobChangeFeedAsyncPagable = blobChangeFeedClient.GetChangesAsync();
            IList<BlobChangeFeedEvent> list = await blobChangeFeedAsyncPagable.ToListAsync();
            Console.WriteLine("sdfsdf");
        }
    }
}
