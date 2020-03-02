// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.ChangeFeed.Models;

namespace Azure.Storage.ChangeFeed
{
    internal class Shard
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string _shardPath;
        private readonly List<Chunk> _chunks;

        public Shard(
            BlobContainerClient containerClient,
            string shardPath)
        {
            _containerClient = containerClient;
            _shardPath = shardPath;
            _chunks = new List<Chunk>();
        }

        public async Task InitalizeShard(bool async)
        {
            // Get Chunks
            if (async)
            {
                await foreach (BlobHierarchyItem blobHierarchyItem in _containerClient.GetBlobsByHierarchyAsync(
                    prefix: _shardPath).ConfigureAwait(false))
                {
                    if (blobHierarchyItem.IsPrefix
                        || blobHierarchyItem.Blob.Name.Contains(Constants.ChangeFeed.InitalizationManifestPath))
                        continue;

                    Chunk chunk = new Chunk(_containerClient, blobHierarchyItem.Blob.Name);
                    // TODO maybe we shoud lazially initalize the segments as they are needed?
                    await chunk.InitalizeChunk(async: true).ConfigureAwait(false);
                    _chunks.Add(chunk);
                }
            }
            else
            {
                foreach (BlobHierarchyItem blobHierarchyItem in _containerClient.GetBlobsByHierarchy(
                    prefix: _shardPath))
                {
                    if (blobHierarchyItem.IsPrefix
                        || blobHierarchyItem.Blob.Name.Contains(Constants.ChangeFeed.InitalizationManifestPath))
                        continue;

                    Chunk chunk = new Chunk(_containerClient, blobHierarchyItem.Blob.Name);
                    // TODO maybe we shoud lazially initalize the segments as they are needed?
                    chunk.InitalizeChunk(async: false).EnsureCompleted();
                    _chunks.Add(chunk);
                }
            }
        }

        public bool HasNext()
        {
            return _chunks.Count > 0;
        }

        public async Task<BlobChangeFeedEvent> Next(bool async)
        {
            if (!HasNext())
            {
                return null;
            }

            BlobChangeFeedEvent changeFeedEvent = null;
            Chunk currentChunk = _chunks[0];

            if (async)
            {
                changeFeedEvent = await currentChunk.Next(async: true).ConfigureAwait(false);
            }
            else
            {
                changeFeedEvent = currentChunk.Next(async: false).EnsureCompleted();
            }

            bool hasNext;
            if (async)
            {
                hasNext = await currentChunk.HasNext(async: true).ConfigureAwait(false);
            }
            else
            {
                hasNext = currentChunk.HasNext(async: false).EnsureCompleted();
            }

            if (!hasNext)
            {
                _chunks.RemoveAt(0);
            }
            return changeFeedEvent;
        }
    }
}
