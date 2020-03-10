namespace Azure.Storage.Blobs.Specialized
{
    public partial class EncryptedBlobClient : Azure.Storage.Blobs.BlobClient
    {
        protected EncryptedBlobClient() { }
        public EncryptedBlobClient(string connectionString, string blobContainerName, string blobName, Azure.Storage.Common.Cryptography.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Blobs.BlobClientOptions options = null) { }
        public EncryptedBlobClient(System.Uri blobUri, Azure.Core.TokenCredential credential, Azure.Storage.Common.Cryptography.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Blobs.BlobClientOptions options = null) { }
        public EncryptedBlobClient(System.Uri blobUri, Azure.Storage.Common.Cryptography.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Blobs.BlobClientOptions options = null) { }
        public EncryptedBlobClient(System.Uri blobUri, Azure.Storage.StorageSharedKeyCredential credential, Azure.Storage.Common.Cryptography.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Blobs.BlobClientOptions options = null) { }
        protected override Azure.Storage.Blobs.Models.BlobContent TransformDownloadSliceContent(Azure.Storage.Blobs.Models.BlobContent content, Azure.HttpRange originalRange, string receivedContentRange, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        protected override System.Threading.Tasks.Task<Azure.Storage.Blobs.Models.BlobContent> TransformDownloadSliceContentAsync(Azure.Storage.Blobs.Models.BlobContent content, Azure.HttpRange originalRange, string receivedContentRange, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        protected override Azure.HttpRange TransformDownloadSliceRange(Azure.HttpRange range) { throw null; }
        protected override Azure.Storage.Blobs.Models.BlobContent TransformUploadContent(Azure.Storage.Blobs.Models.BlobContent content, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        protected override System.Threading.Tasks.Task<Azure.Storage.Blobs.Models.BlobContent> TransformUploadContentAsync(Azure.Storage.Blobs.Models.BlobContent content, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
    }
}
