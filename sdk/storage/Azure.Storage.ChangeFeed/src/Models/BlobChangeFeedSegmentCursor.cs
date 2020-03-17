// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Storage.ChangeFeed.Models
{
    /// <summary>
    /// Segment Cursor.
    /// </summary>
    public class BlobChangeFeedSegmentCursor
    {
        /// <summary>
        /// Shard Cursors.
        /// </summary>
        public List<BlobChangeFeedShardCursor> ShardCursors { get; private set; }

        /// <summary>
        /// Index of the current Shard.
        /// </summary>
        public int ShardIndex { get; private set; }

        /// <summary>
        /// The DateTimeOffset of the Segment.
        /// </summary>
        public DateTimeOffset SegmentTime { get; private set; }

        internal BlobChangeFeedSegmentCursor(
            DateTimeOffset segmentDateTime,
            List<BlobChangeFeedShardCursor> shardCursors,
            int shardIndex)
        {
            SegmentTime = segmentDateTime;
            ShardCursors = shardCursors;
            ShardIndex = shardIndex;
        }

        internal BlobChangeFeedSegmentCursor() { }
    }
}
