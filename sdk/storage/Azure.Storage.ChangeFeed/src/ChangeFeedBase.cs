// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Azure.Storage.ChangeFeed
{
    internal class ChangeFeedBase
    {
        /// <summary>
        /// Builds a DateTimeOffset from a segment path.
        /// </summary>
        internal static DateTimeOffset SegmentPathToDateTimeOffset(string segmentPath)
        {
            string[] splitPath = segmentPath.Split('/');
            return new DateTimeOffset(
                year: int.Parse(splitPath[2], CultureInfo.InvariantCulture),
                month: int.Parse(splitPath[3], CultureInfo.InvariantCulture),
                day: int.Parse(splitPath[4], CultureInfo.InvariantCulture),
                hour: int.Parse(splitPath[5], CultureInfo.InvariantCulture),
                minute: 0,
                second: 0,
                offset: TimeSpan.Zero);
        }
    }
}
