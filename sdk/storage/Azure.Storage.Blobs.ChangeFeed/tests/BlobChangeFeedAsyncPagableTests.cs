// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.ChangeFeed.Models;
using NUnit.Framework;

namespace Azure.Storage.Blobs.ChangeFeed.Tests
{
    public class BlobChangeFeedAsyncPagableTests : ChangeFeedTestBase
    {
        public BlobChangeFeedAsyncPagableTests(bool async)
            : base(async, null /* RecordedTestMode.Record /* to re-record */)
        {
        }

        //TODO better cursor tests
        //TODO start and end time tests
        //TODO page size tests

        [Test]
        [Ignore("Can't record avro")]
        public async Task Test()
        {
            BlobServiceClient service = GetServiceClient_SharedKey();
            BlobChangeFeedClient blobChangeFeedClient = service.GetChangeFeedClient();
            BlobChangeFeedAsyncPagable blobChangeFeedAsyncPagable
                = blobChangeFeedClient.GetChangesAsync();
            IList<BlobChangeFeedEvent> list = await blobChangeFeedAsyncPagable.ToListAsync();
            foreach (BlobChangeFeedEvent e in list)
            {
                Console.WriteLine(e);
            }
        }

        [Test]
        [Ignore("Can't record avro")]
        public async Task CursorTest()
        {
            BlobServiceClient service = GetServiceClient_SharedKey();
            BlobChangeFeedClient blobChangeFeedClient = service.GetChangeFeedClient();
            BlobChangeFeedAsyncPagable blobChangeFeedAsyncPagable
                = blobChangeFeedClient.GetChangesAsync();
            IAsyncEnumerable<Page<BlobChangeFeedEvent>> asyncEnumerable = blobChangeFeedAsyncPagable.AsPages(pageSizeHint: 10);
            Page<BlobChangeFeedEvent> page = await asyncEnumerable.FirstAsync();
            foreach (BlobChangeFeedEvent changeFeedEvent in page.Values)
            {
                Console.WriteLine(changeFeedEvent.Id);
            }

            Console.WriteLine("break");

            string continuation = page.ContinuationToken;

            BlobChangeFeedAsyncPagable cursorBlobChangeFeedAsyncPagable
                = blobChangeFeedClient.GetChangesAsync(continuation);

            IList<BlobChangeFeedEvent> list = await cursorBlobChangeFeedAsyncPagable.ToListAsync();
            foreach (BlobChangeFeedEvent e in list)
            {
                Console.WriteLine(e.Id);
            }

        }
    }
}
