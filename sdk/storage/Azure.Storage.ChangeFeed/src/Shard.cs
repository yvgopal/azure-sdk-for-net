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
        private readonly Queue<Chunk> _chunks;
        private bool _isInitialized;

        public Shard(
            BlobContainerClient containerClient,
            string shardPath)
        {
            _containerClient = containerClient;
            _shardPath = shardPath;
            _chunks = new Queue<Chunk>();
            _isInitialized = false;
        }

        private async Task Initalize(bool async)
        {
            // Get Chunks
            if (async)
            {
                await foreach (BlobHierarchyItem blobHierarchyItem in _containerClient.GetBlobsByHierarchyAsync(
                    prefix: _shardPath).ConfigureAwait(false))
                {
                    if (blobHierarchyItem.IsPrefix)
                        continue;

                    Chunk chunk = new Chunk(_containerClient, blobHierarchyItem.Blob.Name);
                    _chunks.Enqueue(chunk);
                }
            }
            else
            {
                foreach (BlobHierarchyItem blobHierarchyItem in _containerClient.GetBlobsByHierarchy(
                    prefix: _shardPath))
                {
                    if (blobHierarchyItem.IsPrefix)
                        continue;

                    Chunk chunk = new Chunk(_containerClient, blobHierarchyItem.Blob.Name);
                    _chunks.Enqueue(chunk);
                }
            }
            _isInitialized = true;
        }

        public bool HasNext()
        {
            if (!_isInitialized)
            {
                return true;
            }

            return _chunks.Count > 0;
        }

        public async Task<BlobChangeFeedEvent> Next(bool async)
        {
            if (!_isInitialized)
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
                throw new InvalidOperationException("Shard doesn't have any more events");
            }

            Chunk currentChunk = _chunks.Peek();
            BlobChangeFeedEvent changeFeedEvent;

            if (async)
            {
                changeFeedEvent = await currentChunk.Next(async: true).ConfigureAwait(false);
            }
            else
            {
                changeFeedEvent = currentChunk.Next(async: false).EnsureCompleted();
            }

            // Remove currentChunk if it doesn't have another event.
            bool currentChunkHasNext;
            if (async)
            {
                currentChunkHasNext = await currentChunk.HasNext(async: true).ConfigureAwait(false);
            }
            else
            {
                currentChunkHasNext = currentChunk.HasNext(async: false).EnsureCompleted();
            }

            if (!currentChunkHasNext)
            {
                _chunks.Dequeue();
            }
            return changeFeedEvent;
        }
    }
}
