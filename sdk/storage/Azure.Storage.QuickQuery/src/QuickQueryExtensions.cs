// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Blobs.Specialized;

namespace Azure.Storage.QuickQuery
{
    /// <summary>
    /// Quick Query extensions.
    /// </summary>
    public static class QuickQueryExtensions
    {
        /// <summary>
        /// Gets a <see cref="BlobQuickQueryClient"/>.
        /// </summary>
        /// <param name="blockBlobClient"><see cref="BlobQuickQueryClient"/> associated with the
        /// block blob.</param>
        /// <returns>A <see cref="BlobQuickQueryClient"/>.</returns>
        public static BlobQuickQueryClient ToQuickQueryClient(this BlockBlobClient blockBlobClient)
        {
            return new BlobQuickQueryClient();
        }
    }
}
