// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.ChangeFeed.Models;

namespace Azure.Storage.ChangeFeed
{
    internal class ChangeFeed
    {
        private readonly BlobContainerClient _containerClient;
        private readonly List<Segment> _segments;
        private int _segmentCursor;
        //TODO need to make mutable for live streaming events
        //private DateTimeOffset _lastConsumable;

        public ChangeFeed(BlobServiceClient blobServiceClient)
        {
            _containerClient = blobServiceClient.GetBlobContainerClient(Constants.ChangeFeed.ChangeFeedContainerName);
            _segments = new List<Segment>();
            _segmentCursor = 0;
        }

        public  async Task InitalizeChangeFeed(bool async)
        {
            // Check if Change Feed has been abled for this account.
            bool changeFeedContainerExists;

            if (async)
            {
                changeFeedContainerExists = await _containerClient.ExistsAsync().ConfigureAwait(false);
            }
            else
            {
                changeFeedContainerExists = _containerClient.Exists();
            }

            if (!changeFeedContainerExists)
            {
                //TODO improve this error message
                throw new ArgumentException("Change Feed hasn't been enabled on this account, or is current being enabled.");
            }

            //// Get last consumable
            //BlobClient blobClient = _containerClient.GetBlobClient(Constants.ChangeFeed.MetaSegmentsPath);
            //BlobDownloadInfo blobDownloadInfo;
            //if (async)
            //{
            //    blobDownloadInfo = await blobClient.DownloadAsync().ConfigureAwait(false);
            //}
            //else
            //{
            //    blobDownloadInfo = blobClient.Download();
            //}

            //JsonDocument jsonMetaSegment;
            //if (async)
            //{
            //    jsonMetaSegment = await JsonDocument.ParseAsync(blobDownloadInfo.Content).ConfigureAwait(false);
            //}
            //else
            //{
            //    jsonMetaSegment = JsonDocument.Parse(blobDownloadInfo.Content);
            //}

            //_lastConsumable = jsonMetaSegment.RootElement.GetProperty("lastConsumable").GetDateTimeOffset();

            // Get Segments
            if (async)
            {
                await foreach (BlobHierarchyItem blobHierarchyItem in _containerClient.GetBlobsByHierarchyAsync(
                    prefix: Constants.ChangeFeed.SegmentPrefix).ConfigureAwait(false))
                {
                    if (blobHierarchyItem.IsPrefix
                        || blobHierarchyItem.Blob.Name.Contains(Constants.ChangeFeed.InitalizationManifestPath))
                        continue;

                    Segment segment = new Segment(_containerClient, blobHierarchyItem.Blob.Name);
                    // TODO maybe we shoud lazially initalize the segments as they are needed?
                    await segment.InitalizeSegment(async: true).ConfigureAwait(false);
                    _segments.Add(segment);
                }
            }
            else
            {
                foreach (BlobHierarchyItem blobHierarchyItem in _containerClient.GetBlobsByHierarchy(
                    prefix: Constants.ChangeFeed.SegmentPrefix))
                {
                    if (blobHierarchyItem.IsPrefix
                        || blobHierarchyItem.Blob.Name.Contains(Constants.ChangeFeed.InitalizationManifestPath))
                        continue;

                    Segment segment = new Segment(_containerClient, blobHierarchyItem.Blob.Name);
                    // TODO maybe we shoud lazially initalize the segments as they are needed?
                    segment.InitalizeSegment(async: false).EnsureCompleted();
                    _segments.Add(segment);
                }
            }
        }

        public async Task<Page<BlobChangeFeedEvent>> GetPage(
            bool async,
            int pageSize = 512)
        {
            if (!HasNext())
            {
                return null;
            }

            Segment currentSegment = _segments[_segmentCursor];

            // Get next page
            Page<BlobChangeFeedEvent> page;
            if (async)
            {
                page = await currentSegment.GetPage(async: true, pageSize).ConfigureAwait(false);
            }
            else
            {
                page = currentSegment.GetPage(async: false, pageSize).EnsureCompleted();
            }

            // If the current segment is completed, remove it
            if (!currentSegment.HasNext())
            {
                _segments.RemoveAt(_segmentCursor);
            }

            return page;
        }

        public bool HasNext()
        {
            return _segments.Count > 0;
        }

        //public DateTimeOffset LastConsumable()
        //{
        //    return _lastConsumable;
        //}
    }
}
