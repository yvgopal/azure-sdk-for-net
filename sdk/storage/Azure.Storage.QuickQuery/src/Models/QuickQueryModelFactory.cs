// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// QuickQueryModelFactory provides utilities for mocking.
    /// </summary>
    public static partial class QuickQueryModelFactory
    {
        #region BlobQueryError
        /// <summary>
        /// Creates a new <see cref="BlobQueryError"/> instance for mocking.
        /// </summary>
        public static BlobQueryError BlobQueryError(
            string name,
            string description,
            bool isFatal,
            long position)
            => new BlobQueryError()
            {
                Name = name,
                Description = description,
                IsFatal = isFatal,
                Position = position
            };
        #endregion BlobQueryError
    }
}
