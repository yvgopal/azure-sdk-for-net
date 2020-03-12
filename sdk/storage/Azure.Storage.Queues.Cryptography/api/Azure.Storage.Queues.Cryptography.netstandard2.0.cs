namespace Azure.Storage.Queues.Specialized
{
    public partial class EncryptedQueueClient : Azure.Storage.Queues.QueueClient
    {
        protected EncryptedQueueClient() { }
        public EncryptedQueueClient(string connectionString, string queueName, Azure.Storage.Common.Cryptography.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Queues.QueueClientOptions options = null) { }
        public EncryptedQueueClient(System.Uri queueUri, Azure.Core.TokenCredential credential, Azure.Storage.Common.Cryptography.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Queues.QueueClientOptions options = null) { }
        public EncryptedQueueClient(System.Uri queueUri, Azure.Storage.Common.Cryptography.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Queues.QueueClientOptions options = null) { }
        public EncryptedQueueClient(System.Uri queueUri, Azure.Storage.StorageSharedKeyCredential credential, Azure.Storage.Common.Cryptography.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Queues.QueueClientOptions options = null) { }
        protected override string TransformMessageDownload(string downloadedMessage, System.Threading.CancellationToken cancellationToken) { throw null; }
        protected override string TransformMessageUpload(string messageToUpload, System.Threading.CancellationToken cancellationToken) { throw null; }
    }
    public static partial class SpecializedQueueExtensions
    {
        public static Azure.Storage.Queues.Specialized.EncryptedQueueClient GetEncryptedQueueClient(this Azure.Storage.Queues.QueueServiceClient serviceClient, string queueName, Azure.Storage.Common.Cryptography.ClientsideEncryptionOptions encryptionOptions) { throw null; }
    }
}
