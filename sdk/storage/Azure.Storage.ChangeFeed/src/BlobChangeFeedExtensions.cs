// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Azure.Storage.Blobs;

namespace Azure.Storage.ChangeFeed
{
    /// <summary>
    /// BlobChangeFeedExtensions.
    /// </summary>
    public static class BlobChangeFeedExtensions
    {
        /// <summary>
        /// GetChangeFeedClient.
        /// </summary>
        /// <param name="serviceClient"></param>
        /// <returns><see cref="BlobChangeFeedClient"/>.</returns>
        public static BlobChangeFeedClient GetChangeFeedClient(this BlobServiceClient serviceClient)
        {
            return new BlobChangeFeedClient(serviceClient);
        }

        /// <summary>
        /// Builds a DateTimeOffset from a segment path.
        /// </summary>
        internal static DateTimeOffset ToDateTimeOffset(this string segmentPath)
        {
            if (segmentPath == null)
            {
                return default;
            }

            string[] splitPath = segmentPath.Split('/');
            return new DateTimeOffset(
                year: int.Parse(splitPath[2], CultureInfo.InvariantCulture),
                month: int.Parse(splitPath[3], CultureInfo.InvariantCulture),
                day: int.Parse(splitPath[4], CultureInfo.InvariantCulture),
                hour: int.Parse(splitPath[5], CultureInfo.InvariantCulture) / 100,
                minute: 0,
                second: 0,
                offset: TimeSpan.Zero);
        }
    }
}
