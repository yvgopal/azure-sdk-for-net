// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Cryptography;
using Azure.Core.Pipeline;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Common;
using Azure.Storage.Common.Cryptography.Models;
using Azure.Storage.Common.Cryptography;

using Metadata = System.Collections.Generic.IDictionary<string, string>;
using static Azure.Storage.Common.Cryptography.Utility;
using Azure.Storage.Blobs.Specialized.Models;

namespace Azure.Storage.Blobs.Specialized
{
    /// <summary>
    /// The <see cref="EncryptedBlobClient"/> allows you to manipulate
    /// Azure Storage block blobs with client-side encryption. See
    /// <see cref="BlobClient"/> for more details.
    /// </summary>
    public class EncryptedBlobClient : BlobClient
    {

        /// <summary>
        /// The wrapper used to wrap the content encryption key.
        /// </summary>
        private IKeyEncryptionKey KeyWrapper { get; }

        /// <summary>
        /// The key resolver used to select the correct key for decrypting existing blobs.
        /// </summary>
        private IKeyEncryptionKeyResolver KeyResolver { get; }

        /// <summary>
        /// The algorithm identifier to use with the <see cref="KeyWrapper"/>. Value to pass into
        /// <see cref="IKeyEncryptionKey.WrapKey(string, ReadOnlyMemory{byte}, CancellationToken)"/>
        /// and it's async counterpart.
        /// </summary>
        private string KeyWrapAlgorithm { get; }

        #region ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedBlobClient"/>
        /// class for mocking.
        /// </summary>
        protected EncryptedBlobClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedBlobClient"/>
        /// class.
        /// </summary>
        /// <param name="connectionString">
        /// A connection string includes the authentication information
        /// required for your application to access data in an Azure Storage
        /// account at runtime.
        ///
        /// For more information, <see href="https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string"/>.
        /// </param>
        /// <param name="blobContainerName">
        /// The name of the container containing this encrypted block blob.
        /// </param>
        /// <param name="blobName">
        /// The name of this encrypted block blob.
        /// </param>
        /// <param name="encryptionOptions">
        /// Clientside encryption options to provide encryption and/or
        /// decryption implementations to the client.
        /// </param>
        /// <param name="options">
        /// Optional client options that define the transport pipeline
        /// policies for authentication, retries, etc., that are applied to
        /// every request.
        /// </param>
        public EncryptedBlobClient(
            string connectionString,
            string blobContainerName,
            string blobName,
            ClientsideEncryptionOptions encryptionOptions,
            BlobClientOptions options = default)
            : base(
                  connectionString,
                  blobContainerName,
                  blobName,
                  options)
        {
            KeyWrapper = encryptionOptions.KeyEncryptionKey;
            KeyWrapAlgorithm = encryptionOptions.EncryptionKeyWrapAlgorithm;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockBlobClient"/>
        /// class.
        /// </summary>
        /// <param name="blobUri">
        /// A <see cref="Uri"/> referencing the blob that includes the
        /// name of the account, the name of the container, and the name of
        /// the blob.
        /// </param>
        /// <param name="encryptionOptions">
        /// Clientside encryption options to provide encryption and/or
        /// decryption implementations to the client.
        /// every request.
        /// </param>
        /// <param name="options">
        /// Optional client options that define the transport pipeline
        /// policies for authentication, retries, etc., that are applied to
        /// every request.
        /// </param>
        public EncryptedBlobClient(
            Uri blobUri,
            ClientsideEncryptionOptions encryptionOptions,
            BlobClientOptions options = default)
            : base(
                  blobUri,
                  options)
        {
            KeyWrapper = encryptionOptions.KeyEncryptionKey;
            KeyWrapAlgorithm = encryptionOptions.EncryptionKeyWrapAlgorithm;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockBlobClient"/>
        /// class.
        /// </summary>
        /// <param name="blobUri">
        /// A <see cref="Uri"/> referencing the blob that includes the
        /// name of the account, the name of the container, and the name of
        /// the blob.
        /// </param>
        /// <param name="credential">
        /// The shared key credential used to sign requests.
        /// </param>
        /// <param name="encryptionOptions">
        /// Clientside encryption options to provide encryption and/or
        /// decryption implementations to the client.
        /// every request.
        /// </param>
        /// <param name="options">
        /// Optional client options that define the transport pipeline
        /// policies for authentication, retries, etc., that are applied to
        /// every request.
        /// </param>
        public EncryptedBlobClient(
            Uri blobUri,
            StorageSharedKeyCredential credential,
            ClientsideEncryptionOptions encryptionOptions,
            BlobClientOptions options = default)
            : base(
                  blobUri,
                  credential,
                  options)
        {
            KeyWrapper = encryptionOptions.KeyEncryptionKey;
            KeyWrapAlgorithm = encryptionOptions.EncryptionKeyWrapAlgorithm;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockBlobClient"/>
        /// class.
        /// </summary>
        /// <param name="blobUri">
        /// A <see cref="Uri"/> referencing the blob that includes the
        /// name of the account, the name of the container, and the name of
        /// the blob.
        /// </param>
        /// <param name="credential">
        /// The token credential used to sign requests.
        /// </param>
        /// <param name="encryptionOptions">
        /// Clientside encryption options to provide encryption and/or
        /// decryption implementations to the client.
        /// every request.
        /// </param>
        /// <param name="options">
        /// Optional client options that define the transport pipeline
        /// policies for authentication, retries, etc., that are applied to
        /// every request.
        /// </param>
        public EncryptedBlobClient(
            Uri blobUri,
            TokenCredential credential,
            ClientsideEncryptionOptions encryptionOptions,
            BlobClientOptions options = default)
            : base(
                  blobUri,
                  credential,
                  options)
        {
            KeyWrapper = encryptionOptions.KeyEncryptionKey;
            KeyWrapAlgorithm = encryptionOptions.EncryptionKeyWrapAlgorithm;
        }

        private EncryptedBlobClient(
            Uri blobUri,
            ClientsideEncryptionOptions encryptionOptions,
            HttpPipelinePolicy authentication,
            BlobClientOptions options)
            : base(blobUri, authentication, options)
        {
            KeyWrapper = encryptionOptions.KeyEncryptionKey;
            KeyWrapAlgorithm = encryptionOptions.EncryptionKeyWrapAlgorithm;
        }

        /// <summary>
        /// This behaves like a constructor. It has a conflicting signature with another public construtor, but
        /// has different behavior. The necessary extra behavior happens in this method and then invokes a private
        /// constructor with a now-unique signature.
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="blobName"></param>
        /// <param name="encryptionOptions"></param>
        /// <returns></returns>
        internal static EncryptedBlobClient EncryptedBlobClientFromContainerClient(
            BlobContainerClient containerClient,
            string blobName,
            ClientsideEncryptionOptions encryptionOptions)
        {
            (BlobClientOptions options, HttpPipelinePolicy authPolicy) = GetContainerPipelineInfo(containerClient);

            return new EncryptedBlobClient(
                containerClient.Uri.AppendToPath(blobName),
                encryptionOptions,
                authPolicy,
                options);
        }
        #endregion ctors

        #region Transform Upload
        /// <summary>
        /// Encrypts the upload stream.
        /// </summary>
        /// <param name="content">Blob content to encrypt.</param>
        /// <param name="cancellationToken">
        /// Optional <see cref="CancellationToken"/> to propagate
        /// notifications that the operation should be cancelled.
        /// </param>
        /// <returns>Transformed content stream.</returns>
        protected override BlobContent TransformUploadContent(BlobContent content, CancellationToken cancellationToken = default)
            => TransformUploadContentInternal(content, false, cancellationToken).EnsureCompleted();

        /// <summary>
        /// Encrypts the upload stream.
        /// </summary>
        /// <param name="content">Blob content to encrypt.</param>
        /// <param name="cancellationToken">
        /// Optional <see cref="CancellationToken"/> to propagate
        /// notifications that the operation should be cancelled.
        /// </param>
        /// <returns>Transformed content stream.</returns>
        protected override async Task<BlobContent> TransformUploadContentAsync(BlobContent content, CancellationToken cancellationToken = default)
            => await TransformUploadContentInternal(content, true, cancellationToken).ConfigureAwait(false);

        private async Task<BlobContent> TransformUploadContentInternal(BlobContent content, bool async, CancellationToken cancellationToken)
        {
            (Stream nonSeekableCiphertext, EncryptionData encryptionData) = await EncryptInternal(
                content.Content,
                KeyWrapper,
                KeyWrapAlgorithm,
                async: async,
                cancellationToken).ConfigureAwait(false);

            var updatedMetadata = new Dictionary<string, string>(content.Metadata ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase)
            {
                { EncryptionConstants.EncryptionDataKey, encryptionData.Serialize() }
            };

            return new BlobContent
            {
                Content = new RollingBufferStream(
                    nonSeekableCiphertext,
                    EncryptionConstants.DefaultRollingBufferSize,
                    content.Content.Length + (EncryptionConstants.EncryptionBlockSize - content.Content.Length % EncryptionConstants.EncryptionBlockSize)),
                Metadata = updatedMetadata
            };

            //var generatedKey = CreateKey(EncryptionConstants.EncryptionKeySizeBits);

            //using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider() { Key = generatedKey })
            //{

            //    var encryptionData = async
            //        ? await EncryptionData.CreateInternal(aesProvider.IV, KeyWrapAlgorithm, generatedKey, KeyWrapper, true, cancellationToken).ConfigureAwait(false)
            //        : EncryptionData.CreateInternal(aesProvider.IV, KeyWrapAlgorithm, generatedKey, KeyWrapper, false, cancellationToken).EnsureCompleted();

            //    var encryptedContent = new RollingBufferStream(
            //        new CryptoStream(plaintext, aesProvider.CreateEncryptor(), CryptoStreamMode.Read),
            //        EncryptionConstants.DefaultRollingBufferSize,
            //        plaintext.Length + (EncryptionConstants.EncryptionBlockSize - plaintext.Length % EncryptionConstants.EncryptionBlockSize));
            //    return (encryptedContent, encryptionData);
            //}
        }
        #endregion

        #region TransformDownload
        /// <summary>
        ///
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        protected override HttpRange TransformDownloadSliceRange(HttpRange range)
        {
            return new EncryptedBlobRange(range).AdjustedRange;
        }

        /// <inheritdoc/>
        protected override BlobContent TransformDownloadSliceContent(
            BlobContent content,
            HttpRange originalRange,
            string receivedContentRange,
            CancellationToken cancellationToken = default)
            => TransformDownloadSliceContentInternal(
                content,
                originalRange,
                receivedContentRange,
                false,
                cancellationToken).EnsureCompleted();

        /// <inheritdoc/>
        protected override async Task<BlobContent> TransformDownloadSliceContentAsync(
            BlobContent content,
            HttpRange originalRange,
            string receivedContentRange,
            CancellationToken cancellationToken = default)
            => await TransformDownloadSliceContentInternal(
                content,
                originalRange,
                receivedContentRange,
                true,
                cancellationToken).ConfigureAwait(false);

        private async Task<BlobContent> TransformDownloadSliceContentInternal(
            BlobContent content,
            HttpRange originalRange,
            string receivedContentRange,
            bool async,
            CancellationToken cancellationToken)
        {
            AssertKeyAccessPresent();

            ContentRange? contentRange = string.IsNullOrWhiteSpace(receivedContentRange)
                ? default
                : ContentRange.Parse(receivedContentRange);

            EncryptionData encryptionData = GetAndValidateEncryptionData(content.Metadata);
            if (encryptionData == default)
            {
                return content; // TODO readjust range
            }

            bool ivInStream = originalRange.Offset >= 16; // TODO should this check originalRange? tests seem to pass

            var plaintext = await Utility.DecryptInternal(
                content.Content,
                encryptionData,
                ivInStream,
                KeyResolver,
                KeyWrapper,
                CanIgnorePadding(contentRange),
                async).ConfigureAwait(false);

            // retrim start of stream to original requested location
            // keeping in mind whether we already pulled the IV out of the stream as well
            int gap = (int)(originalRange.Offset - (contentRange?.Start ?? 0))
                - (ivInStream ? EncryptionConstants.EncryptionBlockSize : 0);
            if (gap > 0)
            {
                // throw away initial bytes we want to trim off; stream cannot seek into future
                if (async)
                {
                    await plaintext.ReadAsync(new byte[gap], 0, gap, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    plaintext.Read(new byte[gap], 0, gap);
                }
            }

            return new BlobContent
            {
                Content = new LengthLimitingStream(plaintext, originalRange.Length),
                Metadata = content.Metadata
            };
        }

        private void AssertKeyAccessPresent()
        {
            if (KeyWrapper == default && KeyResolver == default)
            {
                throw EncryptionErrors.NoKeyAccessor();
            }
        }

        internal static EncryptionData GetAndValidateEncryptionData(Metadata metadata)
        {
            if (metadata == default)
            {
                return default;
            }
            if (!metadata.TryGetValue(EncryptionConstants.EncryptionDataKey, out string encryptedDataString))
            {
                return default;
            }

            EncryptionData encryptionData = EncryptionData.Deserialize(encryptedDataString);

            _ = encryptionData.ContentEncryptionIV ?? throw EncryptionErrors.MissingEncryptionMetadata(
                nameof(EncryptionData.ContentEncryptionIV));
            _ = encryptionData.WrappedContentKey.EncryptedKey ?? throw EncryptionErrors.MissingEncryptionMetadata(
                nameof(EncryptionData.WrappedContentKey.EncryptedKey));

            // Throw if the encryption protocol on the message doesn't match the version that this client library
            // understands and is able to decrypt.
            if (EncryptionConstants.EncryptionProtocolV1 != encryptionData.EncryptionAgent.Protocol)
            {
                throw EncryptionErrors.BadEncryptionAgent(encryptionData.EncryptionAgent.Protocol);
            }

            return encryptionData;
        }

        /// <summary>
        /// Gets whether to ignore padding options for decryption.
        /// </summary>
        /// <param name="contentRange">Downloaded content range.</param>
        /// <returns>True if we should ignore padding.</returns>
        /// <remarks>
        /// If the last cipher block of the blob was returned, we need the padding. Otherwise, we can ignore it.
        /// </remarks>
        private static bool CanIgnorePadding(ContentRange? contentRange)
        {
            // if Content-Range not present, we requested the whole blob
            if (!contentRange.HasValue)
            {
                return false;
            }

            // if range is wildcard, we requested the whole blob
            if (!contentRange.Value.End.HasValue)
            {
                return false;
            }

            // blob storage will always return ContentRange.Size
            // we don't have to worry about the impossible decision of what to do if it doesn't

            // did we request the last block?
            // end is inclusive/0-index, so end = n and size = n+1 means we requested the last block
            if (contentRange.Value.Size - contentRange.Value.End == 1)
            {
                return false;
            }

            return true;
        }
        #endregion
    }

    //TODO uncomment upon Azure.Core.ClientOptions "clone with modifications" support
    /// <summary>
    /// Add easy to discover methods to <see cref="BlobContainerClient"/> for
    /// creating <see cref="EncryptedBlobClient"/> instances.
    /// </summary>
#pragma warning disable SA1402 // File may only contain a single type
    public static partial class SpecializedBlobExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        /// Create a new <see cref="EncryptedBlobClient"/> object by
        /// concatenating <paramref name="blobName"/> to
        /// the end of the <paramref name="containerClient"/>'s
        /// <see cref="BlobContainerClient.Uri"/>.
        /// </summary>
        /// <param name="containerClient">The <see cref="BlobContainerClient"/>.</param>
        /// <param name="blobName">The name of the encrypted block blob.</param>
        /// <param name="encryptionOptions">
        /// Clientside encryption options to provide encryption and/or
        /// decryption implementations to the client.
        /// every request.
        /// </param>
        /// <returns>A new <see cref="EncryptedBlobClient"/> instance.</returns>
        public static EncryptedBlobClient GetEncryptedBlobClient(
            this BlobContainerClient containerClient,
            string blobName,
            ClientsideEncryptionOptions encryptionOptions)
            => EncryptedBlobClient.EncryptedBlobClientFromContainerClient(containerClient, blobName, encryptionOptions);
    }
}
