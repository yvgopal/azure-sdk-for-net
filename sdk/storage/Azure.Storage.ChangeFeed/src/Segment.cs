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
    internal class Segment
    {
        /// <summary>
        /// If this Segment is finalized.
        /// </summary>
        public bool Finalized { get; private set; }

        /// <summary>
        /// The time (to the nearest hour) associated with this Segment.
        /// </summary>
        public DateTimeOffset DateTime { get; private set; }

        /// <summary>
        /// Container client for listing Shards.
        /// </summary>
        private readonly BlobContainerClient _containerClient;

        /// <summary>
        /// The path to the manifest for this Segment.
        /// </summary>
        private readonly string _manifestPath;

        /// <summary>
        /// The Shards associated with this Segment.
        /// </summary>
        private readonly List<Shard> _shards;

        /// <summary>
        /// The index of the Shard we will return the next event from.
        /// </summary>
        private int _shardIndex;

        /// <summary>
        /// If this Segement has been initalized.
        /// </summary>
        private bool _isInitalized;

        public Segment(
            BlobContainerClient containerClient,
            string manifestPath)
        {
            _containerClient = containerClient;
            _manifestPath = manifestPath;
            DateTime = manifestPath.ToDateTimeOffset();
            _shards = new List<Shard>();
            _shardIndex = 0;
        }

        private async Task Initalize(bool async)
        {
            // Download segment manifest
            BlobClient blobClient = _containerClient.GetBlobClient(_manifestPath);
            BlobDownloadInfo blobDownloadInfo;

            if (async)
            {
                blobDownloadInfo = await blobClient.DownloadAsync().ConfigureAwait(false);
            }
            else
            {
                blobDownloadInfo = blobClient.Download();
            }

            // Parse segment manifest
            JsonDocument jsonManifest;

            if (async)
            {
                jsonManifest = await JsonDocument.ParseAsync(blobDownloadInfo.Content).ConfigureAwait(false);
            }
            else
            {
                jsonManifest = JsonDocument.Parse(blobDownloadInfo.Content);
            }

            // Initalized Finalized field
            string statusString = jsonManifest.RootElement.GetProperty("status").GetString();
            Finalized = statusString == "Finalized";

            foreach (JsonElement shardJsonElement in jsonManifest.RootElement.GetProperty("chunkFilePaths").EnumerateArray())
            {
                //TODO cleanup this line
                string shardPath = shardJsonElement.ToString().Substring("$blobchangefeed/".Length);
                Shard shard = new Shard(_containerClient, shardPath);
                _shards.Add(shard);
            }
            _isInitalized = true;
        }

        public BlobChangeFeedSegmentCursor GetCursor()
        {
            List<BlobChangeFeedShardCursor> shardCursors = new List<BlobChangeFeedShardCursor>();
            foreach (Shard shard in _shards)
            {
                shardCursors.Add(shard.GetCursor());
            }
            return new BlobChangeFeedSegmentCursor(
                segmentDateTime: DateTime,
                shardCursors: shardCursors,
                shardIndex: _shardIndex);
        }

        public async Task<Page<BlobChangeFeedEvent>> GetPage(
            bool async,
            int? pageSize)
        {
            List<BlobChangeFeedEvent> changeFeedEventList = new List<BlobChangeFeedEvent>();

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
                throw new InvalidOperationException("Segment doesn't have any more events");
            }

            int i = 0;
            while (i < pageSize && _shards.Count > 0)
            {
                Shard currentShard = _shards[_shardIndex];

                BlobChangeFeedEvent changeFeedEvent;
                if (async)
                {
                    changeFeedEvent = await currentShard.Next(async: true).ConfigureAwait(false);
                }
                else
                {
                    changeFeedEvent = currentShard.Next(async: false).EnsureCompleted();
                }

                changeFeedEventList.Add(changeFeedEvent);

                // If the current shard is completed, remove it from _shards
                if (!currentShard.HasNext())
                {
                    _shards.RemoveAt(_shardIndex);
                }

                i++;
                _shardIndex++;
                if (_shardIndex >= _shards.Count)
                {
                    _shardIndex = 0;
                }
            }

            //TODO how to get raw response for page?  Does it matter?
            return new BlobChangeFeedEventPage(changeFeedEventList);
        }

        //TODO figure out if this is right.
        public bool HasNext()
        {
            if (!_isInitalized)
            {
                return true;
            }

            return _shards.Count > 0;
        }
    }
}
