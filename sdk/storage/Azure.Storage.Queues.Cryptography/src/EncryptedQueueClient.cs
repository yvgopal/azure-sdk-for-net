// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Cryptography;
using Azure.Core.Pipeline;
using Azure.Storage.Common.Cryptography;
using Azure.Storage.Common.Cryptography.Models;
using Azure.Storage.Queues.Specialized.Models;
using static Azure.Storage.Common.Cryptography.Utility;

namespace Azure.Storage.Queues.Specialized
{
    /// <summary>
    /// The <see cref="EncryptedQueueClient"/> allows you to manipulate
    /// Azure Storage queues with client-side encryption. See
    /// <see cref="QueueClient"/> for more details.
    /// Note that encrypting messages may make their content slightly
    /// longer, and encryption metadata will also extent that length.
    /// Impact is estimated to be no more than 0.5 KB towards the maximum
    /// message size, and so impact should be manageable.
    /// </summary>
    public class EncryptedQueueClient : QueueClient
    {
        /// <summary>
        /// The wrapper used to wrap the content encryption key.
        /// </summary>
        private IKeyEncryptionKey KeyWrapper { get; }

        /// <summary>
        /// Resolver to get the correct <see cref="IKeyEncryptionKey"/>
        /// for unwrapping a message's content encryption key.
        /// </summary>
        private IKeyEncryptionKeyResolver KeyResolver { get;  }

        /// <summary>
        /// The algorithm identifier to use with the <see cref="KeyWrapper"/>. Value to pass into
        /// <see cref="IKeyEncryptionKey.WrapKey(string, ReadOnlyMemory{byte}, CancellationToken)"/>
        /// and it's async counterpart.
        /// </summary>
        private string KeyWrapAlgorithm { get; }

        #region ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedQueueClient"/>
        /// class for mocking.
        /// </summary>
        protected EncryptedQueueClient()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedQueueClient"/>
        /// class.
        /// </summary>
        /// <param name="connectionString">
        /// A connection string includes the authentication information
        /// required for your application to access data in an Azure Storage
        /// account at runtime.
        ///
        /// For more information, <see href="https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string"/>.
        /// </param>
        /// <param name="queueName">
        /// The name of the queue in the storage account to reference.
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
        public EncryptedQueueClient(
            string connectionString,
            string queueName,
            ClientsideEncryptionOptions encryptionOptions,
            QueueClientOptions options = default)
            : base(connectionString, queueName, options)
        {
            KeyWrapper = encryptionOptions.KeyEncryptionKey;
            KeyWrapAlgorithm = encryptionOptions.EncryptionKeyWrapAlgorithm;
            KeyResolver = encryptionOptions.KeyResolver;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedQueueClient"/>
        /// class.
        /// </summary>
        /// <param name="queueUri">
        /// A <see cref="Uri"/> referencing the queue that includes the
        /// name of the account, and the name of the queue.
        /// This is likely to be similar to "https://{account_name}.queue.core.windows.net/{queue_name}".
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
        public EncryptedQueueClient(
            Uri queueUri,
            ClientsideEncryptionOptions encryptionOptions,
            QueueClientOptions options = default)
            : base(queueUri, options)
        {
            KeyWrapper = encryptionOptions.KeyEncryptionKey;
            KeyWrapAlgorithm = encryptionOptions.EncryptionKeyWrapAlgorithm;
            KeyResolver = encryptionOptions.KeyResolver;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedQueueClient"/>
        /// class.
        /// </summary>
        /// <param name="queueUri">
        /// A <see cref="Uri"/> referencing the queue that includes the
        /// name of the account, and the name of the queue.
        /// This is likely to be similar to "https://{account_name}.queue.core.windows.net/{queue_name}".
        /// </param>
        /// <param name="credential">
        /// The shared key credential used to sign requests.
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
        public EncryptedQueueClient(
            Uri queueUri,
            StorageSharedKeyCredential credential,
            ClientsideEncryptionOptions encryptionOptions,
            QueueClientOptions options = default)
            : base(queueUri, credential, options)
        {
            KeyWrapper = encryptionOptions.KeyEncryptionKey;
            KeyWrapAlgorithm = encryptionOptions.EncryptionKeyWrapAlgorithm;
            KeyResolver = encryptionOptions.KeyResolver;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedQueueClient"/>
        /// class.
        /// </summary>
        /// <param name="queueUri">
        /// A <see cref="Uri"/> referencing the queue that includes the
        /// name of the account, and the name of the queue.
        /// This is likely to be similar to "https://{account_name}.queue.core.windows.net/{queue_name}".
        /// </param>
        /// <param name="credential">
        /// The token credential used to sign requests.
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
        public EncryptedQueueClient(
            Uri queueUri,
            TokenCredential credential,
            ClientsideEncryptionOptions encryptionOptions,
            QueueClientOptions options = default)
            : base(queueUri, credential, options)
        {
            KeyWrapper = encryptionOptions.KeyEncryptionKey;
            KeyWrapAlgorithm = encryptionOptions.EncryptionKeyWrapAlgorithm;
            KeyResolver = encryptionOptions.KeyResolver;
        }

        private EncryptedQueueClient(
            Uri queueUri,
            ClientsideEncryptionOptions encryptionOptions,
            HttpPipelinePolicy authentication,
            QueueClientOptions options)
            : base(queueUri, authentication, options)
        {
            KeyWrapper = encryptionOptions.KeyEncryptionKey;
            KeyWrapAlgorithm = encryptionOptions.EncryptionKeyWrapAlgorithm;
            KeyResolver = encryptionOptions.KeyResolver;
        }

        /// <summary>
        /// This behaves like a constructor. It has a conflicting signature with another public construtor, but
        /// has different behavior. The necessary extra behavior happens in this method and then invokes a private
        /// constructor with a now-unique signature.
        /// </summary>
        /// <param name="serviceClient"></param>
        /// <param name="queueName"></param>
        /// <param name="encryptionOptions"></param>
        /// <returns></returns>
        internal static EncryptedQueueClient EncryptedQueueClientFromServiceClient(
            QueueServiceClient serviceClient,
            string queueName,
            ClientsideEncryptionOptions encryptionOptions)
        {
            (var options, var authPolicy) = GetQueueServiceClientPipelineInfo(serviceClient);

            return new EncryptedQueueClient(
                serviceClient.Uri.AppendToPath(queueName),
                encryptionOptions,
                authPolicy,
                options);
        }
        #endregion

        /// <inheritdoc/>
        protected override string TransformMessageUpload(string messageToUpload, CancellationToken cancellationToken)
            => TransformMessageUploadInternal(messageToUpload, false, cancellationToken).EnsureCompleted();

        /// <inheritdoc/>
        protected override async Task<string> TransformMessageUploadAsync(string messageToUpload, CancellationToken cancellationToken)
            => await TransformMessageUploadInternal(messageToUpload, true, cancellationToken).ConfigureAwait(false);

        private async Task<string> TransformMessageUploadInternal(string messageToUpload, bool async, CancellationToken cancellationToken)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(messageToUpload);
            (byte[] ciphertext, EncryptionData encryptionData) = await BufferedEncryptInternal(
                new MemoryStream(bytesToEncrypt),
                KeyWrapper,
                KeyWrapAlgorithm,
                async: async,
                cancellationToken).ConfigureAwait(false);

            return EncryptedMessageSerializer.Serialize(new EncryptedMessage
            {
                EncryptedMessageContents = Convert.ToBase64String(ciphertext),
                EncryptionData = encryptionData
            });
        }

        /// <inheritdoc/>
        protected override string TransformMessageDownload(string downloadedMessage, CancellationToken cancellationToken)
            => TransformMessageDownloadInternal(downloadedMessage, false, cancellationToken).EnsureCompleted();

        /// <inheritdoc/>
        protected override async Task<string> TransformMessageDownloadAsync(string downloadedMessage, CancellationToken cancellationToken)
            => await TransformMessageDownloadInternal(downloadedMessage, true, cancellationToken).ConfigureAwait(false);

        private async Task<string> TransformMessageDownloadInternal(string downloadedMessage, bool async, CancellationToken cancellationToken)
        {
            if (!EncryptedMessageSerializer.TryDeserialize(downloadedMessage, out var encryptedMessage))
            {
                return downloadedMessage; // not recognized as client-side encrypted message
            }
            var decryptedMessageStream = await DecryptInternal(
                new MemoryStream(Convert.FromBase64String(encryptedMessage.EncryptedMessageContents)),
                encryptedMessage.EncryptionData,
                ivInStream: false,
                KeyResolver,
                KeyWrapper,
                noPadding: false,
                async: async,
                cancellationToken).ConfigureAwait(false);

            return new StreamReader(decryptedMessageStream, Encoding.UTF8).ReadToEnd();
        }
    }

    /// <summary>
    /// Add easy to discover methods to <see cref="QueueServiceClient"/> for
    /// creating <see cref="EncryptedQueueClient"/> instances.
    /// </summary>
#pragma warning disable SA1402 // File may only contain a single type
    public static partial class SpecializedQueueExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceClient"></param>
        /// <param name="queueName"></param>
        /// <param name="encryptionOptions"></param>
        /// <returns></returns>
        public static EncryptedQueueClient GetEncryptedQueueClient(
            this QueueServiceClient serviceClient,
            string queueName,
            ClientsideEncryptionOptions encryptionOptions)
            => EncryptedQueueClient.EncryptedQueueClientFromServiceClient(serviceClient, queueName, encryptionOptions);
    }
}
