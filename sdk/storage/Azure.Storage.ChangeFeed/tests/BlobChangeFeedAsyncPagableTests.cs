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

        //TODO better cursor tests
        //TODO start and end time tests
        //TODO page size tests
        //TODO tests for BlobChangeFeedExtensions

        [Test]
        public async Task Test()
        {
            BlobServiceClient service = GetServiceClient_SharedKey();
            BlobChangeFeedClient blobChangeFeedClient = service.GetChangeFeedClient();
            BlobChangeFeedAsyncPagable blobChangeFeedAsyncPagable
                = blobChangeFeedClient.GetChangesAsync();
            IList<BlobChangeFeedEvent> list = await blobChangeFeedAsyncPagable.ToListAsync();
            foreach (BlobChangeFeedEvent e in list)
            {
                Console.WriteLine(e.Id);
            }
        }

        [Test]
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

            BlobChangeFeedCursor cursor = blobChangeFeedAsyncPagable.GetCursor();

            BlobChangeFeedAsyncPagable cursorBlobChangeFeedAsyncPagable
                = blobChangeFeedClient.GetChangesAsync(cursor);

            IList<BlobChangeFeedEvent> list = await cursorBlobChangeFeedAsyncPagable.ToListAsync();
            foreach (BlobChangeFeedEvent e in list)
            {
                Console.WriteLine(e.Id);
            }

        }
    }
}
