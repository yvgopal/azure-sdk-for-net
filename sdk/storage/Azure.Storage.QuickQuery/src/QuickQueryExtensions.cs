// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;
using Azure.Core.Pipeline;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.QuickQuery.Models;

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
        public static BlobQuickQueryClient GetQuickQueryClient(this BlockBlobClient blockBlobClient)
        {
            BlobClientOptions options = BlobClientInternals.GetClientOptions(blockBlobClient);
            return new BlobQuickQueryClient(
                uri: blockBlobClient.Uri,
                pipeline: BlobClientInternals.GetHttpPipeline(blockBlobClient),
                serviceVersion: options.Version,
                clientDiagnostics: new ClientDiagnostics(options),
                customerProvidedKey: BlobClientInternals.GetCustomerProvidedKey(blockBlobClient));
        }

        internal static QuickQuerySerialization ToQuickQuerySerialization(
            this BlobQueryTextConfiguration textConfiguration)
        {
            if (textConfiguration == default)
            {
                return default;
            }

            QuickQuerySerialization serialization = new QuickQuerySerialization
            {
                Format = new QuickQueryFormat()
            };

            serialization.Format.DelimitedTextConfiguration = default;
            serialization.Format.JsonTextConfiguration = default;

            if (textConfiguration.GetType() == typeof(CsvTextConfiguration))
            {
                CsvTextConfiguration cvsTextConfiguration = textConfiguration as CsvTextConfiguration;
                serialization.Format.Type = QuickQueryFormatType.Delimited;
                serialization.Format.DelimitedTextConfiguration = new DelimitedTextConfigurationInternal
                {
                    ColumnSeparator = cvsTextConfiguration.ColumnSeparator?.ToString(CultureInfo.InvariantCulture),
                    FieldQuote = cvsTextConfiguration.FieldQuote?.ToString(CultureInfo.InvariantCulture),
                    RecordSeparator = cvsTextConfiguration.RecordSeparator?.ToString(CultureInfo.InvariantCulture),
                    EscapeChar = cvsTextConfiguration.EscapeCharacter?.ToString(CultureInfo.InvariantCulture),
                    HasHeaders = cvsTextConfiguration.HasHeaders
                };
            }
            else if (textConfiguration.GetType() == typeof(JsonTextConfiguration))
            {
                JsonTextConfiguration jsonTextConfiguration = textConfiguration as JsonTextConfiguration;
                serialization.Format.Type = QuickQueryFormatType.Json;
                serialization.Format.JsonTextConfiguration = new JsonTextConfigurationInternal
                {
                    RecordSeparator = jsonTextConfiguration.RecordSeparator?.ToString(CultureInfo.InvariantCulture)
                };
            }
            else
            {
                throw new ArgumentException(Constants.QuickQuery.Errors.InvalidTextConfigurationType);
            }

            return serialization;
        }

        internal static Models.EncryptionAlgorithmType ToQuickQueryEncryptionAlgorithmType(this     Blobs.Models.EncryptionAlgorithmType blobEncryptionAlgorithmType)
            => Models.EncryptionAlgorithmType.Aes256;

        internal static BlobDownloadInfo ToBlobDownloadInfo(this BlobQuickQueryResult quickQueryResult)
            => BlobsModelFactory.BlobDownloadInfo(
                lastModified: quickQueryResult.LastModified,
                blobSequenceNumber: quickQueryResult.BlobSequenceNumber,
                blobType: (Blobs.Models.BlobType)quickQueryResult.BlobType,
                contentCrc64: quickQueryResult.ContentCrc64,
                contentLanguage: quickQueryResult.ContentLanguage,
                copyStatusDescription: quickQueryResult.CopyStatusDescription,
                copyId: quickQueryResult.CopyId,
                copyProgress: quickQueryResult.CopyProgress,
                copySource: quickQueryResult.CopySource != default ? new Uri(quickQueryResult.CopySource) : default,
                copyStatus: (Blobs.Models.CopyStatus)quickQueryResult.CopyStatus,
                contentDisposition: quickQueryResult.ContentDisposition,
                leaseDuration: (Blobs.Models.LeaseDurationType)quickQueryResult.LeaseDuration,
                cacheControl: quickQueryResult.CacheControl,
                leaseState: (Blobs.Models.LeaseState)quickQueryResult.LeaseState,
                contentEncoding: quickQueryResult.ContentEncoding,
                leaseStatus: (Blobs.Models.LeaseStatus)quickQueryResult.LeaseStatus,
                //TODO this might be wrong
                contentHash: quickQueryResult.ContentMD5,
                acceptRanges: quickQueryResult.AcceptRanges,
                eTag: quickQueryResult.ETag,
                blobCommittedBlockCount: quickQueryResult.BlobCommittedBlockCount,
                contentRange: quickQueryResult.ContentRange,
                isServerEncrypted: quickQueryResult.IsServerEncrypted,
                contentType: quickQueryResult.ContentType,
                encryptionKeySha256: quickQueryResult.EncryptionKeySha256,
                encryptionScope: quickQueryResult.EncryptionScope,
                contentLength: quickQueryResult.ContentLength,
                //TODO this one might be wrong
                blobContentHash: quickQueryResult.BlobContentMD5,
                metadata: quickQueryResult.Metadata,
                content: quickQueryResult.Body,
                copyCompletionTime: quickQueryResult.CopyCompletionTime);
    }
}
