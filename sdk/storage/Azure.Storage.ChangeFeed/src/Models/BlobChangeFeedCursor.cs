// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Storage.ChangeFeed.Models
{
    /// <summary>
    /// BlobChangeFeedCursor.
    /// </summary>
    public class BlobChangeFeedCursor
    {
        /// <summary>
        /// CursorVersion.
        /// </summary>
        public int CursorVersion { get; internal set; }

        /// <summary>
        /// UrlHash.
        /// </summary>
        public long UrlHash { get; internal set; }

        /// <summary>
        /// EndDateTime.
        /// </summary>
        public DateTimeOffset EndDateTime { get; internal set; }

        /// <summary>
        /// SegmentCursor.
        /// </summary>
        public int SegmentCursor { get; internal set; }

        /// <summary>
        /// ShardCursor.
        /// </summary>
        public int ShardCursor { get; internal set; }

        /// <summary>
        /// ChunkCursor.
        /// </summary>
        public int ChunkCursor { get; internal set; }
    }
}
