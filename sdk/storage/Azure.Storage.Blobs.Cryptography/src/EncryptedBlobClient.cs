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
                  options.WithPolicy(new ClientSideDecryptionPolicy(
                      encryptionOptions.KeyResolver,
                      encryptionOptions.KeyEncryptionKey)))
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
                  options.WithPolicy(new ClientSideDecryptionPolicy(
                      encryptionOptions.KeyResolver,
                      encryptionOptions.KeyEncryptionKey)))
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
                  options.WithPolicy(new ClientSideDecryptionPolicy(
                      encryptionOptions.KeyResolver,
                      encryptionOptions.KeyEncryptionKey)))
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
                  options.WithPolicy(new ClientSideDecryptionPolicy(
                      encryptionOptions.KeyResolver,
                      encryptionOptions.KeyEncryptionKey)))
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

        //TODO uncomment upon Azure.Core.ClientOptions "clone with modifications" support
        ///// <summary>
        ///// This behaves like a constructor. It has a conflicting signature with another public construtor, but
        ///// has different behavior. The necessary extra behavior happens in this method and then invokes a private
        ///// constructor with a now-unique signature.
        ///// </summary>
        ///// <param name="containerClient"></param>
        ///// <param name="blobName"></param>
        ///// <param name="encryptionOptions"></param>
        ///// <returns></returns>
        //internal static EncryptedBlobClient EncryptedBlobClientFromContainerClient(
        //    BlobContainerClient containerClient,
        //    string blobName,
        //    ClientsideEncryptionOptions encryptionOptions)
        //{
        //    (var options, var authPolicy) = GetContainerPipelineInfo(containerClient);

        //    var editedOptions = new BlobClientOptions(options);
        //    editedOptions.AddPolicy(
        //        new ClientSideDecryptionPolicy(encryptionOptions.KeyResolver, encryptionOptions.KeyEncryptionKey),
        //        HttpPipelinePosition.PerCall);

        //    return new EncryptedBlobClient(
        //        containerClient.Uri.AppendToPath(blobName),
        //        encryptionOptions,
        //        authPolicy,
        //        editedOptions);
        //}
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

        /// <summary>
        /// Transforms the content of an individual REST download, not an overall multipart download.
        /// </summary>
        /// <param name="content">Content of this download slice.</param>
        /// <param name="originalRange">Orignially requested range of the slice.</param>
        /// <param name="adjustedRange">Adjusted range of the slice, as determined by <see cref="TransformDownloadSliceRange"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Transformed content.</returns>
        protected override BlobContent TransformDownloadSliceContent(
            BlobContent content,
            HttpRange originalRange,
            HttpRange adjustedRange,
            CancellationToken cancellationToken = default)
        {
            return content; // no-op
        }

        /// <summary>
        /// Transforms the content of an individual REST download asyncronously, not an overall multipart download.
        /// </summary>
        /// <param name="content">Content of this download slice.</param>
        /// <param name="originalRange">Orignially requested range of the slice.</param>
        /// <param name="adjustedRange">Adjusted range of the slice, as determined by <see cref="TransformDownloadSliceRange"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Transformed content.</returns>
        protected override Task<BlobContent> TransformDownloadSliceContentAsync(
            BlobContent content,
            HttpRange originalRange,
            HttpRange adjustedRange,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(content); // no-op
        }

        private async Task<BlobContent> TransformDownloadSliceContentInternal(
            BlobContent content,
            HttpRange originalRange,
            HttpRange adjustedRange,
            bool async,
            CancellationToken cancellationToken)
        {
            AssertKeyAccessPresent();

            EncryptionData encryptionData = GetAndValidateEncryptionData(content.Metadata);
            if (encryptionData == default)
            {
                return content; // TODO readjust range
            }

            bool ivInStream = adjustedRange.Offset != 0; //TODO should this check originalRange? tests seem to pass

            var plaintext = await Utility.DecryptInternal(content.Content, encryptionData, ivInStream, KeyResolver, KeyWrapper, CanIgnorePadding(), async).ConfigureAwait(false);

            // retrim start of stream to original requested location
            // keeping in mind whether we already pulled the IV out of the stream as well
            int gap = (int)(originalRange.Offset - adjustedRange.Offset)
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
        /// <param name="headers">Response headers for the download.</param>
        /// <returns>True if we should ignore padding.</returns>
        /// <remarks>
        /// If the last cipher block of the blob was returned, we need the padding. Otherwise, we can ignore it.
        /// </remarks>
        private static bool CanIgnorePadding(ResponseHeaders headers)
        {
            // if Content-Range not present, we requested the whole blob
            if (!headers.TryGetValue(Constants.HeaderNames.ContentRange, out string contentRange))
            {
                return false;
            }

            // parse header value (e.g. "bytes <start>-<end>/<blobSize>")
            // end is the inclusive last byte; e.g. header "bytes 0-7/8" is the entire 8-byte blob
            var tokens = contentRange.Split(new char[] { ' ', '-', '/' }); // ["bytes", "<start>", "<end>", "<blobSize>"]
            if (tokens.Length < 4)
            {
                throw Errors.ParsingHttpRangeFailed();
            }

            // did we request the last block?
            if (long.Parse(tokens[3], System.Globalization.CultureInfo.InvariantCulture) -
                long.Parse(tokens[2], System.Globalization.CultureInfo.InvariantCulture) < EncryptionConstants.EncryptionBlockSize)
            {
                return false;
            }

            return true;
        }
        #endregion
    }

    //TODO uncomment upon Azure.Core.ClientOptions "clone with modifications" support
    //    /// <summary>
    //    /// Add easy to discover methods to <see cref="BlobContainerClient"/> for
    //    /// creating <see cref="EncryptedBlobClient"/> instances.
    //    /// </summary>
    //#pragma warning disable SA1402 // File may only contain a single type
    //    public static partial class SpecializedBlobExtensions
    //#pragma warning restore SA1402 // File may only contain a single type
    //    {
    //        /// <summary>
    //        /// Create a new <see cref="EncryptedBlobClient"/> object by
    //        /// concatenating <paramref name="blobName"/> to
    //        /// the end of the <paramref name="containerClient"/>'s
    //        /// <see cref="BlobContainerClient.Uri"/>.
    //        /// </summary>
    //        /// <param name="containerClient">The <see cref="BlobContainerClient"/>.</param>
    //        /// <param name="blobName">The name of the encrypted block blob.</param>
    //        /// <param name="encryptionOptions">
    //        /// Clientside encryption options to provide encryption and/or
    //        /// decryption implementations to the client.
    //        /// every request.
    //        /// </param>
    //        /// <returns>A new <see cref="EncryptedBlobClient"/> instance.</returns>
    //        public static EncryptedBlobClient GetEncryptedBlobClient(
    //            this BlobContainerClient containerClient,
    //            string blobName,
    //            ClientsideEncryptionOptions encryptionOptions)
    //            /*
    //             * Extension methods have to be in their own static class, but the logic for this method needs a protected
    //             * static method in BlobBaseClient. So this extension method just passes the arguments on to a place with
    //             * access to that method.
    //             */
    //            => EncryptedBlobClient.EncryptedBlobClientFromContainerClient(
    //                containerClient,
    //                blobName,
    //                encryptionOptions);
    //    }
}
