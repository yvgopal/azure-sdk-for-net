// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avro.Util;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.ChangeFeed.Models;

namespace Azure.Storage.ChangeFeed
{
    internal class ChangeFeed : ChangeFeedBase
    {
        /// <summary>
        /// BlobContainerClient for making List Blob requests and creating Segments.
        /// </summary>
        private readonly BlobContainerClient _containerClient;
        private readonly List<Segment> _segments;
        private DateTimeOffset _segmentCursor;
        //TODO need to make mutable for live streaming events
        private DateTimeOffset _lastConsumable;
        private DateTimeOffset? _startTime;
        private DateTimeOffset? _endTime;
        private string _segmentsPathsContinutationToken;

        /// <summary>
        /// If this ChangeFeed has been initalized.
        /// </summary>
        private bool _isInitalized;

        // Start time will be rounded down to the nearest hour.
        public ChangeFeed(
            BlobServiceClient blobServiceClient,
            DateTimeOffset? startTime = default,
            DateTimeOffset? endTime = default)
        {
            _containerClient = blobServiceClient.GetBlobContainerClient(Constants.ChangeFeed.ChangeFeedContainerName);
            _segments = new List<Segment>();
            _isInitalized = false;
            _startTime = RoundHourDown(startTime);
            _endTime = RoundHourUp(endTime);
        }

        private async Task Initalize(bool async)
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

            // Get last consumable
            BlobClient blobClient = _containerClient.GetBlobClient(Constants.ChangeFeed.MetaSegmentsPath);
            BlobDownloadInfo blobDownloadInfo;
            if (async)
            {
                blobDownloadInfo = await blobClient.DownloadAsync().ConfigureAwait(false);
            }
            else
            {
                blobDownloadInfo = blobClient.Download();
            }

            JsonDocument jsonMetaSegment;
            if (async)
            {
                jsonMetaSegment = await JsonDocument.ParseAsync(blobDownloadInfo.Content).ConfigureAwait(false);
            }
            else
            {
                jsonMetaSegment = JsonDocument.Parse(blobDownloadInfo.Content);
            }

            _lastConsumable = jsonMetaSegment.RootElement.GetProperty("lastConsumable").GetDateTimeOffset();

            // Get Segments
            //if (async)
            //{
            //    await foreach (BlobHierarchyItem blobHierarchyItem in _containerClient.GetBlobsByHierarchyAsync(
            //        prefix: Constants.ChangeFeed.SegmentPrefix).ConfigureAwait(false))
            //    {
            //        if (blobHierarchyItem.IsPrefix
            //            || blobHierarchyItem.Blob.Name.Contains(Constants.ChangeFeed.InitalizationManifestPath))
            //            continue;

            //        Segment segment = new Segment(_containerClient, blobHierarchyItem.Blob.Name);
            //        _segments.Add(segment);
            //    }
            //}
            //else
            //{
            //    foreach (BlobHierarchyItem blobHierarchyItem in _containerClient.GetBlobsByHierarchy(
            //        prefix: Constants.ChangeFeed.SegmentPrefix))
            //    {
            //        if (blobHierarchyItem.IsPrefix
            //            || blobHierarchyItem.Blob.Name.Contains(Constants.ChangeFeed.InitalizationManifestPath))
            //            continue;

            //        Segment segment = new Segment(_containerClient, blobHierarchyItem.Blob.Name);
            //        _segments.Add(segment);
            //    }
            //}

            // Get ~first month of segments

            List<string> firstSegmentPaths;

            if (async)
            {
                firstSegmentPaths = await GetFirstSegments(async: true).ConfigureAwait(false);
            }
            else
            {
                firstSegmentPaths = GetFirstSegments(async: false).EnsureCompleted();
            }

            foreach (string segmentPath in firstSegmentPaths)
            {
                Segment segment = new Segment(_containerClient, segmentPath);
                _segments.Add(segment);
            }

            _segmentCursor = _segments[0].DateTime;
            _isInitalized = true;
        }

        //TODO current round robin strategy doesn't work for live streaming!
        // The last segment may still be adding chunks.
        public async Task<Page<BlobChangeFeedEvent>> GetPage(
            bool async,
            int pageSize = 512)
        {
            if (!_isInitalized)
            {
                if (async)
                {
                    await Initalize(async: true).ConfigureAwait(false);
                }
                else
                {
                    Initalize(async: false).EnsureCompleted();
                }
            }

            if (!HasNext())
            {
                return null;
            }

            // Get next page
            Page<BlobChangeFeedEvent> page;

            Segment currentSegment = _segments[0];

            //TODO what should we return here?  Also do we really need to check this on every page?
            if (currentSegment.DateTime > _endTime)
            {
                return null;
            }

            //TODO what should we return here?  Also do we really need to check this on every page?
            if (currentSegment.DateTime > _lastConsumable)
            {
                return new BlobChangeFeedEventPage();
            }

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
                _segments.RemoveAt(0);
            }

            // If _segments is empty, refill it
            if (_segments.Count == 0)
            {
                _
            }

            return page;
        }

        public bool HasNext()
        {
            if (!_isInitalized)
            {
                return true;
            }
            if (_endTime < LastConsumable())
            {
                return _segmentCursor <= _endTime;
            }
            else
            {
                return _segmentCursor <= LastConsumable();
            }
        }

        //TODO how do update this?
        public DateTimeOffset LastConsumable()
        {
            return _lastConsumable;
        }

        //TODO what if we are live streaming and LastConsumable is advancing?
        private async Task<List<string>> GetFirstSegments(bool async)
        {
            // Determine Change Feed start time.
            string firstSegmentPath;

            if (async)
            {
                firstSegmentPath = await GetFirstSegmentPath(async: true).ConfigureAwait(false);
            }
            else
            {
                firstSegmentPath = GetFirstSegmentPath(async: false).EnsureCompleted();
            }

            DateTimeOffset changeFeedStartTime = SegmentPathToDateTimeOffset(firstSegmentPath);
            DateTimeOffset startTime = changeFeedStartTime;

            if (_startTime.HasValue && changeFeedStartTime < _startTime)
            {
                startTime = _startTime.Value;
            }

            List<string> firstMonthOfSegments;

            if (async)
            {
                firstMonthOfSegments = await GetFirstMonthOfSegmentPaths(
                    async: true,
                    startTime).ConfigureAwait(false);
            }
            else
            {
                firstMonthOfSegments = GetFirstMonthOfSegmentPaths(
                    async: false,
                    startTime)
                    .EnsureCompleted();
            }

            return firstMonthOfSegments;
        }

        /// <summary>
        /// Gets the first calendar month of segment paths that occured after startTime.
        /// This will return a maximum of ~720 (24 hours * 30 days) segment paths.
        /// </summary>
        private async Task<Page<string>> GetFirstMonthOfSegmentPaths(
            bool async,
            DateTimeOffset startTime,
            int pageSize = 10)
        {
            // determine year
            int year = startTime.Year;

            while (year <= _lastConsumable.Year)
            {
                string yearPath = BuildSegmentPath(year);
                bool yearExists;
                if (async)
                {
                    yearExists = await SegmentsExists(
                        async: true,
                        path: yearPath).ConfigureAwait(false);
                }
                else
                {
                    yearExists = SegmentsExists(
                        async: false,
                        path: yearPath).EnsureCompleted();
                }

                if (yearExists)
                {
                    break;
                }
                year++;
            }

            // determine month
            int month = startTime.Month;

            while (month <= _lastConsumable.Month)
            {
                string monthPath = BuildSegmentPath(year, month);
                bool monthExists;
                if (async)
                {
                    monthExists = await SegmentsExists(
                        async: true,
                        path: monthPath).ConfigureAwait(false);
                }
                else
                {
                    monthExists = SegmentsExists(
                        async: false,
                        path: monthPath).EnsureCompleted();
                }

                if (monthExists)
                {
                    break;
                }
                month++;
            }

            // Download first page of segment paths in the month
            List<string> segmentPaths = new List<string>();
            string continuationToken = null;
            if (async)
            {
                IAsyncEnumerator<Page<BlobHierarchyItem>> asyncEnumerator= GetMonthSegmentPathAsyncEnumerator(
                    year,
                    month,
                    pageSize);

            }
            else
            {
                IEnumerator<Page<BlobHierarchyItem>> asyncEnumerator = GetMonthSegmentPathEnumerator(
                    year,
                    month,
                    pageSize);
            }

            return new SegmentPathPage(segmentPaths, continuationToken);
        }

        private IAsyncEnumerator<Page<BlobHierarchyItem>> GetMonthSegmentPathAsyncEnumerator(
            int year,
            int month,
            int pageSize)
        {
            string path = $"{Constants.ChangeFeed.SegmentPrefix}{year}/{month}";
            return _containerClient.GetBlobsByHierarchyAsync(
                    prefix: path)
                .AsPages(pageSizeHint: 10)
                .GetAsyncEnumerator();
        }

        private IEnumerator<Page<BlobHierarchyItem>> GetMonthSegmentPathEnumerator(
            int year,
            int month,
            int pageSize)
        {
            string path = $"{Constants.ChangeFeed.SegmentPrefix}{year}/{month}";
            return _containerClient.GetBlobsByHierarchy(
                    prefix: path)
                .AsPages(pageSizeHint: 10)
                .GetEnumerator();
        }


        private static string BuildSegmentPath(
            int year,
            int? month = null,
            int? day = null,
            int? hour = null)
        {
            StringBuilder stringBuilder = new StringBuilder(Constants.ChangeFeed.SegmentPrefix);

            stringBuilder.Append(year + "/");

            if (month.HasValue)
            {
                stringBuilder.Append(month.Value + "/");
            }

            if (day.HasValue)
            {
                stringBuilder.Append(day.Value + "/");
            }

            if (hour.HasValue)
            {
                stringBuilder.Append(hour.Value + "/");
            }

            return stringBuilder.ToString();
        }

        private async Task<string> GetFirstSegmentPath(bool async)
        {
            BlobHierarchyItem firstSegmentItem;

            if (async)
            {
                IAsyncEnumerable<Page<BlobHierarchyItem>> enumerable = _containerClient.GetBlobsByHierarchyAsync(
                    prefix: Constants.ChangeFeed.SegmentPrefix).AsPages(pageSizeHint: 2);
                IAsyncEnumerator<Page<BlobHierarchyItem>> enumerator = enumerable.GetAsyncEnumerator();
                await enumerator.MoveNextAsync().ConfigureAwait(false);
                firstSegmentItem = enumerator.Current.Values[1];
            }
            else
            {
                IEnumerable<Page<BlobHierarchyItem>> enumerable = _containerClient.GetBlobsByHierarchy(
                    prefix: Constants.ChangeFeed.SegmentPrefix).AsPages(pageSizeHint: 2);
                IEnumerator<Page<BlobHierarchyItem>> enumerator = enumerable.GetEnumerator();
                enumerator.MoveNext();
                firstSegmentItem = enumerator.Current.Values[1];
            }

            return firstSegmentItem.Blob.Name;
        }

        private async Task<bool> SegmentsExists(
            bool async,
            string path)
        {
            if (async)
            {
                IAsyncEnumerable<Page<BlobHierarchyItem>> enumerable = _containerClient.GetBlobsByHierarchyAsync(
                    prefix: path).AsPages(pageSizeHint: 1);
                IAsyncEnumerator<Page<BlobHierarchyItem>> enumerator = enumerable.GetAsyncEnumerator();
                await enumerator.MoveNextAsync().ConfigureAwait(false);
                return enumerator.Current.Values.Count > 0;
            }
            else
            {
                IEnumerable<Page<BlobHierarchyItem>> enumerable = _containerClient.GetBlobsByHierarchy(
                    prefix: path).AsPages(pageSizeHint: 1);
                IEnumerator<Page<BlobHierarchyItem>> enumerator = enumerable.GetEnumerator();
                enumerator.MoveNext();
                return enumerator.Current.Values.Count > 0;
            }
        }

        /// <summary>
        /// Rounds a DateTimeOffset down to the nearest hour.
        /// </summary>
        private static DateTimeOffset? RoundHourDown(DateTimeOffset? dateTimeOffset)
        {
            if (dateTimeOffset == null)
            {
                return null;
            }

            return new DateTimeOffset(
                year: dateTimeOffset.Value.Year,
                month: dateTimeOffset.Value.Month,
                day: dateTimeOffset.Value.Day,
                hour: dateTimeOffset.Value.Hour,
                minute: 0,
                second: 0,
                offset: dateTimeOffset.Value.Offset);
        }

        /// <summary>
        /// Rounds a DateTimeOffset up to the nearest hour.
        /// </summary>
        private static DateTimeOffset? RoundHourUp(DateTimeOffset? dateTimeOffset)
        {
            if (dateTimeOffset == null)
            {
                return null;
            }

            DateTimeOffset? newDateTimeOffest = RoundHourDown(dateTimeOffset.Value);

            return newDateTimeOffest.Value.AddHours(1);
        }
    }
}
