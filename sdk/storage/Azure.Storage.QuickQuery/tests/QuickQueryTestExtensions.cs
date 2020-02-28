// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.


using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Blobs.Models;

namespace Azure.Storage.QuickQuery.Tests
{
    public static class QuickQueryTestExtensions
    {
        /// <summary>
        /// Convert a base RequestConditions to BlobRequestConditions.
        /// </summary>
        /// <param name="conditions">The <see cref="RequestConditions"/>.</param>
        /// <returns>The <see cref="BlobRequestConditions"/>.</returns>
        public static BlobRequestConditions ToBlobRequestConditions(this RequestConditions conditions) =>
            conditions == null ?
                null :
                new BlobRequestConditions
                {
                    IfMatch = conditions.IfMatch,
                    IfNoneMatch = conditions.IfNoneMatch,
                    IfModifiedSince = conditions.IfModifiedSince,
                    IfUnmodifiedSince = conditions.IfUnmodifiedSince
                };
    }
}
