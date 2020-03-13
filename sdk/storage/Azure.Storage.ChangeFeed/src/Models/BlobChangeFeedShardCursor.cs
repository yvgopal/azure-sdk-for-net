// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Storage.ChangeFeed.Models
{
    /// <summary>
    /// Shard Cursor.
    /// </summary>
    public class BlobChangeFeedShardCursor
    {
        /// <summary>
        /// Index of the current Chunk.
        /// </summary>
        public long ChunkIndex { get; private set; }

        /// <summary>
        /// Index of the current Event.
        /// </summary>
        public long EventIndex { get; private set; }

        internal BlobChangeFeedShardCursor(
            long chunkIndex,
            long eventIndex)
        {
            ChunkIndex = chunkIndex;
            EventIndex = eventIndex;
        }

        internal BlobChangeFeedShardCursor() { }
    }
}
