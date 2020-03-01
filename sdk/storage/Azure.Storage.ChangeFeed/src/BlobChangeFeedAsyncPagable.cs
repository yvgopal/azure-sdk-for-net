// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avro.File;
using Avro.Generic;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.ChangeFeed.Models;

namespace Azure.Storage.ChangeFeed
{
    /// <summary>
    /// BlobChangeFeedPagableAsync.
    /// </summary>
    public class BlobChangeFeedAsyncPagable : AsyncPageable<BlobChangeFeedEvent>
    {
        private BlobContainerClient _containerClient;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal BlobChangeFeedAsyncPagable(BlobServiceClient serviceClient)
        {
            _containerClient = serviceClient.GetBlobContainerClient(Constants.ChangeFeed.ChangeFeedContainerName);
        }

        /// <summary>
        /// AsPages.
        /// </summary>
        /// <param name="continuationToken"></param>
        /// <param name="pageSizeHint"></param>
        /// <returns></returns>
        public override async IAsyncEnumerable<Page<BlobChangeFeedEvent>> AsPages(
            string continuationToken = null,
            int? pageSizeHint = null)
        {
            // Get all the segments
            await foreach (BlobHierarchyItem segment in _containerClient.GetBlobsByHierarchyAsync(
                prefix: Constants.ChangeFeed.SegmentPrefix).ConfigureAwait(false))
            {
                if (segment.IsPrefix || segment.Blob.Name.Contains("/0000/"))
                    continue;

                // Get the segment's JSON
                using MemoryStream stream = new MemoryStream();
                await _containerClient.GetBlobClient(segment.Blob.Name).DownloadToAsync(stream).ConfigureAwait(false);
                stream.Seek(0, SeekOrigin.Begin);
                using JsonDocument json = JsonDocument.Parse(stream);

                // Get the chunks in the segment
                foreach (JsonElement chunk in json.RootElement.GetProperty("chunkFilePaths").EnumerateArray())
                {
                    // Get all the logs in the chunk
                    string logPrefix = chunk.GetString().Substring("$blobchangefeed/".Length);
                    await foreach (BlobHierarchyItem log
                        in _containerClient.GetBlobsByHierarchyAsync(prefix: logPrefix).ConfigureAwait(false))
                    {
                        if (segment.IsPrefix)
                            continue;

                        // Download and parse the Avro
                        Page<BlobChangeFeedEvent> page = null;
                        using (MemoryStream avroStream = new MemoryStream())
                        {
                            Response raw = await _containerClient.GetBlobClient(log.Blob.Name)
                                .DownloadToAsync(avroStream).ConfigureAwait(false);
                            avroStream.Seek(0, SeekOrigin.Begin);
                            IFileReader<GenericRecord> avroReader
                                = DataFileReader<GenericRecord>.OpenReader(avroStream);
                            page = new BlobChangeFeedEventPage(raw, avroReader.NextEntries.ToList());
                        }
                        yield return page;
                    }
                }
            }
        }
    }
}
