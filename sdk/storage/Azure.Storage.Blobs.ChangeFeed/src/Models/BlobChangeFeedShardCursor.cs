// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Storage.Blobs.ChangeFeed.Models
{
    internal class BlobChangeFeedShardCursor
    {
        /// <summary>
        /// Index of the current Chunk.
        /// </summary>
        public long ChunkIndex { get; set; }

        /// <summary>
        /// Index of the current Event.
        /// </summary>
        public long EventIndex { get; set; }

        internal BlobChangeFeedShardCursor(
            long chunkIndex,
            long eventIndex)
        {
            ChunkIndex = chunkIndex;
            EventIndex = eventIndex;
        }

        /// <summary>
        ///
        /// </summary>
        public BlobChangeFeedShardCursor() { }
    }
}
