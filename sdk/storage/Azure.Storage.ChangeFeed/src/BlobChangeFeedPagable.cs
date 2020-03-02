// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Azure.Storage.Blobs;
using Azure.Storage.ChangeFeed.Models;

namespace Azure.Storage.ChangeFeed
{
    /// <summary>
    /// BlobChangeFeedPagable.
    /// </summary>
    public class BlobChangeFeedPagable : Pageable<BlobChangeFeedEvent>
    {
        private ChangeFeed _changeFeed;

        internal BlobChangeFeedPagable(BlobServiceClient serviceClient)
        {
            _changeFeed = new ChangeFeed(serviceClient);
        }

        /// <summary>
        /// InitalizeAsyncPagable.
        /// </summary>
        public void InitalizeAsyncPagable()
        {
            _changeFeed.InitalizeChangeFeed(async: false).EnsureCompleted();
        }

        /// <summary>
        /// AsPages.
        /// </summary>
        /// <param name="continuationToken"></param>
        /// <param name="pageSizeHint"></param>
        /// <returns></returns>
        public override IEnumerable<Page<BlobChangeFeedEvent>> AsPages(string continuationToken = null, int? pageSizeHint = null)
        {
            throw new NotImplementedException();
        }
    }
}
