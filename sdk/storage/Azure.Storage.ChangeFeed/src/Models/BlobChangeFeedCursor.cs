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
        public DateTimeOffset? EndTime { get; internal set; }

        /// <summary>
        /// The Segment Cursor for the current segment.
        /// </summary>
        public BlobChangeFeedSegmentCursor CurrentSegmentCursor { get; internal set; }

        internal BlobChangeFeedCursor(
            long urlHash,
            DateTimeOffset? endDateTime,
            BlobChangeFeedSegmentCursor currentSegmentCursor)
        {
            CursorVersion = 1;
            UrlHash = urlHash;
            EndTime = endDateTime;
            CurrentSegmentCursor = currentSegmentCursor;
        }

        internal BlobChangeFeedCursor() { }
    }
}
