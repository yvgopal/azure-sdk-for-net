// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text;
using System.Threading;
using Azure.Core;
using Azure.Core.Cryptography;
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
        #endregion

        /// <inheritdoc/>
        protected override string TransformMessageUpload(string messageToUpload, CancellationToken cancellationToken)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(messageToUpload);
            (byte[] ciphertext, EncryptionData encryptionData) = BufferedEncryptInternal(
                new MemoryStream(bytesToEncrypt),
                KeyWrapper,
                KeyWrapAlgorithm,
                async: false,
                cancellationToken).EnsureCompleted();

            return EncryptedMessageSerializer.Serialize(new EncryptedMessage
            {
                EncryptedMessageContents = Convert.ToBase64String(ciphertext),
                EncryptionData = encryptionData
            });
        }

        /// <inheritdoc/>
        protected override string TransformMessageDownload(string downloadedMessage, CancellationToken cancellationToken)
        {
            var encryptedMessage = EncryptedMessageSerializer.Deserialize(downloadedMessage);
            var decryptedMessageStream = DecryptInternal(
                new MemoryStream(Convert.FromBase64String(encryptedMessage.EncryptedMessageContents)),
                encryptedMessage.EncryptionData,
                ivInStream: false,
                KeyResolver,
                KeyWrapper,
                noPadding: false,
                async: false).EnsureCompleted();

            return new StreamReader(decryptedMessageStream, Encoding.UTF8).ReadToEnd();
        }
    }
}
