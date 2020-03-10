namespace Azure.Storage.Blobs.Specialized
{
    public partial class ClientsideEncryptionOptions
    {
        public ClientsideEncryptionOptions() { }
        public string EncryptionKeyWrapAlgorithm { [System.Runtime.CompilerServices.CompilerGeneratedAttribute] get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute] set { } }
        public Azure.Core.Cryptography.IKeyEncryptionKey KeyEncryptionKey { [System.Runtime.CompilerServices.CompilerGeneratedAttribute] get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute] set { } }
        public Azure.Core.Cryptography.IKeyEncryptionKeyResolver KeyResolver { [System.Runtime.CompilerServices.CompilerGeneratedAttribute] get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute] set { } }
    }
    public partial class EncryptedBlobClient : Azure.Storage.Blobs.BlobClient
    {
        protected EncryptedBlobClient() { }
        public EncryptedBlobClient(string connectionString, string blobContainerName, string blobName, Azure.Storage.Blobs.Specialized.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Blobs.BlobClientOptions options = null) { }
        public EncryptedBlobClient(System.Uri blobUri, Azure.Core.TokenCredential credential, Azure.Storage.Blobs.Specialized.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Blobs.BlobClientOptions options = null) { }
        public EncryptedBlobClient(System.Uri blobUri, Azure.Storage.Blobs.Specialized.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Blobs.BlobClientOptions options = null) { }
        public EncryptedBlobClient(System.Uri blobUri, Azure.Storage.StorageSharedKeyCredential credential, Azure.Storage.Blobs.Specialized.ClientsideEncryptionOptions encryptionOptions, Azure.Storage.Blobs.BlobClientOptions options = null) { }
        protected override Azure.Storage.Blobs.Models.BlobUploadContent TransformUploadContent(Azure.Storage.Blobs.Models.BlobUploadContent content, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        protected override System.Threading.Tasks.Task<Azure.Storage.Blobs.Models.BlobUploadContent> TransformUploadContentAsync(Azure.Storage.Blobs.Models.BlobUploadContent content, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
    }
}
namespace Azure.Storage.Blobs.Specialized.Models
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public readonly partial struct ClientSideEncryptionAlgorithm
    {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
        public ClientSideEncryptionAlgorithm(string value) { throw null; }
        public static Azure.Storage.Blobs.Specialized.Models.ClientSideEncryptionAlgorithm AesCbc256 { [System.Runtime.CompilerServices.CompilerGeneratedAttribute] get { throw null; } }
        public bool Equals(Azure.Storage.Blobs.Specialized.Models.ClientSideEncryptionAlgorithm other) { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public override bool Equals(object obj) { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public override int GetHashCode() { throw null; }
        public static bool operator ==(Azure.Storage.Blobs.Specialized.Models.ClientSideEncryptionAlgorithm left, Azure.Storage.Blobs.Specialized.Models.ClientSideEncryptionAlgorithm right) { throw null; }
        public static implicit operator Azure.Storage.Blobs.Specialized.Models.ClientSideEncryptionAlgorithm (string value) { throw null; }
        public static bool operator !=(Azure.Storage.Blobs.Specialized.Models.ClientSideEncryptionAlgorithm left, Azure.Storage.Blobs.Specialized.Models.ClientSideEncryptionAlgorithm right) { throw null; }
        public override string ToString() { throw null; }
    }
    public partial class EncryptionAgent
    {
        public EncryptionAgent() { }
        public Azure.Storage.Blobs.Specialized.Models.ClientSideEncryptionAlgorithm EncryptionAlgorithm { [System.Runtime.CompilerServices.CompilerGeneratedAttribute] get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute] set { } }
        public string Protocol { [System.Runtime.CompilerServices.CompilerGeneratedAttribute] get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute] set { } }
    }
    public partial class WrappedKey
    {
        public WrappedKey() { }
        public string Algorithm { [System.Runtime.CompilerServices.CompilerGeneratedAttribute] get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute] set { } }
        public byte[] EncryptedKey { [System.Runtime.CompilerServices.CompilerGeneratedAttribute] get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute] set { } }
        public string KeyId { [System.Runtime.CompilerServices.CompilerGeneratedAttribute] get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute] set { } }
    }
}
