// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Azure.Storage.Blobs.Models
{
    /// <summary>
    /// Client options for transactional hash validation.
    /// </summary>
    public class TransactionalHashOptions
    {
        /// <summary>
        /// Algorithm the client should use for hashing.
        /// </summary>
        public BlobHashAlgorithm Algorithm { get; }

        /// <summary>
        /// Whether the client should automatically generate/validate hashes or
        /// leave it to the user to supply/validate hashes on requests. Note that
        /// automation allows for transactional hashing in multipart uploads and
        /// downloads where the user otherwise couldn't supply the hash.
        /// </summary>
        public bool AutomateHashing { get; }

        /// <summary>
        /// Constructs a new <see cref="TransactionalHashOptions"/> instance.
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="automateHashing"></param>
        public TransactionalHashOptions(
            BlobHashAlgorithm algorithm = BlobHashAlgorithm.Crc64,
            bool automateHashing = true)
        {
            if (algorithm == BlobHashAlgorithm.Crc64 && automateHashing == false)
            {
                throw new ArgumentException("CRC64 not available for manual hashing.");
            }

            Algorithm = algorithm;
            AutomateHashing = automateHashing;
        }
    }
}
