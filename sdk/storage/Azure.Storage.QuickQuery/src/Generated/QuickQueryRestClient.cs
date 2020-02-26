// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// This file was automatically generated.  Do not edit.

#pragma warning disable IDE0016 // Null check can be simplified
#pragma warning disable IDE0017 // Variable declaration can be inlined
#pragma warning disable IDE0018 // Object initialization can be simplified
#pragma warning disable SA1402  // File may only contain a single type

#region Service
namespace Azure.Storage.QuickQuery
{
    /// <summary>
    /// Azure Blob Storage
    /// </summary>
    internal static partial class QuickQueryRestClient
    {
        #region Blob operations
        /// <summary>
        /// Blob operations for Azure Blob Storage
        /// </summary>
        public static partial class Blob
        {
            #region Blob.QuickQueryAsync
            /// <summary>
            /// The QuickQuery operation enables users to select/project on blob data by providing simple query expressions.
            /// </summary>
            /// <param name="clientDiagnostics">The ClientDiagnostics instance used for operation reporting.</param>
            /// <param name="pipeline">The pipeline used for sending requests.</param>
            /// <param name="resourceUri">The URL of the service account, container, or blob that is the targe of the desired operation.</param>
            /// <param name="version">Specifies the version of the operation to use for this request.</param>
            /// <param name="queryRequest">the query request</param>
            /// <param name="snapshot">The snapshot parameter is an opaque DateTime value that, when present, specifies the blob snapshot to retrieve. For more information on working with blob snapshots, see <a href="https://docs.microsoft.com/en-us/rest/api/storageservices/fileservices/creating-a-snapshot-of-a-blob">Creating a Snapshot of a Blob.</a></param>
            /// <param name="timeout">The timeout parameter is expressed in seconds. For more information, see <a href="https://docs.microsoft.com/en-us/rest/api/storageservices/fileservices/setting-timeouts-for-blob-service-operations">Setting Timeouts for Blob Service Operations.</a></param>
            /// <param name="leaseId">If specified, the operation only succeeds if the resource's lease is active and matches this ID.</param>
            /// <param name="encryptionKey">Optional. Specifies the encryption key to use to encrypt the data provided in the request. If not specified, encryption is performed with the root account encryption key.  For more information, see Encryption at Rest for Azure Storage Services.</param>
            /// <param name="encryptionKeySha256">The SHA-256 hash of the provided encryption key. Must be provided if the x-ms-encryption-key header is provided.</param>
            /// <param name="encryptionAlgorithm">The algorithm used to produce the encryption key hash. Currently, the only accepted value is "AES256". Must be provided if the x-ms-encryption-key header is provided.</param>
            /// <param name="ifModifiedSince">Specify this header value to operate only on a blob if it has been modified since the specified date/time.</param>
            /// <param name="ifUnmodifiedSince">Specify this header value to operate only on a blob if it has not been modified since the specified date/time.</param>
            /// <param name="ifMatch">Specify an ETag value to operate only on blobs with a matching value.</param>
            /// <param name="ifNoneMatch">Specify an ETag value to operate only on blobs without a matching value.</param>
            /// <param name="requestId">Provides a client-generated, opaque value with a 1 KB character limit that is recorded in the analytics logs when storage analytics logging is enabled.</param>
            /// <param name="async">Whether to invoke the operation asynchronously.  The default value is true.</param>
            /// <param name="operationName">Operation name.</param>
            /// <param name="cancellationToken">Cancellation token.</param>
            /// <returns>Azure.Response{Azure.Storage.QuickQuery.Models.BlobQuickQueryResult}</returns>
            public static async System.Threading.Tasks.ValueTask<Azure.Response<Azure.Storage.QuickQuery.Models.BlobQuickQueryResult>> QuickQueryAsync(
                Azure.Core.Pipeline.ClientDiagnostics clientDiagnostics,
                Azure.Core.Pipeline.HttpPipeline pipeline,
                System.Uri resourceUri,
                string version,
                Azure.Storage.QuickQuery.Models.QueryRequest queryRequest = default,
                string snapshot = default,
                int? timeout = default,
                string leaseId = default,
                string encryptionKey = default,
                string encryptionKeySha256 = default,
                Azure.Storage.QuickQuery.Models.EncryptionAlgorithmType? encryptionAlgorithm = default,
                System.DateTimeOffset? ifModifiedSince = default,
                System.DateTimeOffset? ifUnmodifiedSince = default,
                Azure.ETag? ifMatch = default,
                Azure.ETag? ifNoneMatch = default,
                string requestId = default,
                bool async = true,
                string operationName = "BlobClient.QuickQuery",
                System.Threading.CancellationToken cancellationToken = default)
            {
                Azure.Core.Pipeline.DiagnosticScope _scope = clientDiagnostics.CreateScope(operationName);
                try
                {
                    _scope.AddAttribute("url", resourceUri);
                    _scope.Start();
                    using (Azure.Core.HttpMessage _message = QuickQueryAsync_CreateMessage(
                        pipeline,
                        resourceUri,
                        version,
                        queryRequest,
                        snapshot,
                        timeout,
                        leaseId,
                        encryptionKey,
                        encryptionKeySha256,
                        encryptionAlgorithm,
                        ifModifiedSince,
                        ifUnmodifiedSince,
                        ifMatch,
                        ifNoneMatch,
                        requestId))
                    {
                        if (async)
                        {
                            // Send the request asynchronously if we're being called via an async path
                            await pipeline.SendAsync(_message, cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            // Send the request synchronously through the API that blocks if we're being called via a sync path
                            // (this is safe because the Task will complete before the user can call Wait)
                            pipeline.Send(_message, cancellationToken);
                        }
                        Azure.Response _response = _message.Response;
                        cancellationToken.ThrowIfCancellationRequested();
                        return QuickQueryAsync_CreateResponse(clientDiagnostics, _response);
                    }
                }
                catch (System.Exception ex)
                {
                    _scope.Failed(ex);
                    throw;
                }
                finally
                {
                    _scope.Dispose();
                }
            }

            /// <summary>
            /// Create the Blob.QuickQueryAsync request.
            /// </summary>
            /// <param name="pipeline">The pipeline used for sending requests.</param>
            /// <param name="resourceUri">The URL of the service account, container, or blob that is the targe of the desired operation.</param>
            /// <param name="version">Specifies the version of the operation to use for this request.</param>
            /// <param name="queryRequest">the query request</param>
            /// <param name="snapshot">The snapshot parameter is an opaque DateTime value that, when present, specifies the blob snapshot to retrieve. For more information on working with blob snapshots, see <a href="https://docs.microsoft.com/en-us/rest/api/storageservices/fileservices/creating-a-snapshot-of-a-blob">Creating a Snapshot of a Blob.</a></param>
            /// <param name="timeout">The timeout parameter is expressed in seconds. For more information, see <a href="https://docs.microsoft.com/en-us/rest/api/storageservices/fileservices/setting-timeouts-for-blob-service-operations">Setting Timeouts for Blob Service Operations.</a></param>
            /// <param name="leaseId">If specified, the operation only succeeds if the resource's lease is active and matches this ID.</param>
            /// <param name="encryptionKey">Optional. Specifies the encryption key to use to encrypt the data provided in the request. If not specified, encryption is performed with the root account encryption key.  For more information, see Encryption at Rest for Azure Storage Services.</param>
            /// <param name="encryptionKeySha256">The SHA-256 hash of the provided encryption key. Must be provided if the x-ms-encryption-key header is provided.</param>
            /// <param name="encryptionAlgorithm">The algorithm used to produce the encryption key hash. Currently, the only accepted value is "AES256". Must be provided if the x-ms-encryption-key header is provided.</param>
            /// <param name="ifModifiedSince">Specify this header value to operate only on a blob if it has been modified since the specified date/time.</param>
            /// <param name="ifUnmodifiedSince">Specify this header value to operate only on a blob if it has not been modified since the specified date/time.</param>
            /// <param name="ifMatch">Specify an ETag value to operate only on blobs with a matching value.</param>
            /// <param name="ifNoneMatch">Specify an ETag value to operate only on blobs without a matching value.</param>
            /// <param name="requestId">Provides a client-generated, opaque value with a 1 KB character limit that is recorded in the analytics logs when storage analytics logging is enabled.</param>
            /// <returns>The Blob.QuickQueryAsync Message.</returns>
            internal static Azure.Core.HttpMessage QuickQueryAsync_CreateMessage(
                Azure.Core.Pipeline.HttpPipeline pipeline,
                System.Uri resourceUri,
                string version,
                Azure.Storage.QuickQuery.Models.QueryRequest queryRequest = default,
                string snapshot = default,
                int? timeout = default,
                string leaseId = default,
                string encryptionKey = default,
                string encryptionKeySha256 = default,
                Azure.Storage.QuickQuery.Models.EncryptionAlgorithmType? encryptionAlgorithm = default,
                System.DateTimeOffset? ifModifiedSince = default,
                System.DateTimeOffset? ifUnmodifiedSince = default,
                Azure.ETag? ifMatch = default,
                Azure.ETag? ifNoneMatch = default,
                string requestId = default)
            {
                // Validation
                if (resourceUri == null)
                {
                    throw new System.ArgumentNullException(nameof(resourceUri));
                }
                if (version == null)
                {
                    throw new System.ArgumentNullException(nameof(version));
                }

                // Create the request
                Azure.Core.HttpMessage _message = pipeline.CreateMessage();
                Azure.Core.Request _request = _message.Request;

                // Set the endpoint
                _request.Method = Azure.Core.RequestMethod.Post;
                _request.Uri.Reset(resourceUri);
                _request.Uri.AppendQuery("comp", "query", escapeValue: false);
                if (snapshot != null) { _request.Uri.AppendQuery("snapshot", snapshot); }
                if (timeout != null) { _request.Uri.AppendQuery("timeout", timeout.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)); }

                // Add request headers
                _request.Headers.SetValue("x-ms-version", version);
                if (leaseId != null) { _request.Headers.SetValue("x-ms-lease-id", leaseId); }
                if (encryptionKey != null) { _request.Headers.SetValue("x-ms-encryption-key", encryptionKey); }
                if (encryptionKeySha256 != null) { _request.Headers.SetValue("x-ms-encryption-key-sha256", encryptionKeySha256); }
                if (encryptionAlgorithm != null) { _request.Headers.SetValue("x-ms-encryption-algorithm", Azure.Storage.QuickQuery.QuickQueryRestClient.Serialization.ToString(encryptionAlgorithm.Value)); }
                if (ifModifiedSince != null) { _request.Headers.SetValue("If-Modified-Since", ifModifiedSince.Value.ToString("R", System.Globalization.CultureInfo.InvariantCulture)); }
                if (ifUnmodifiedSince != null) { _request.Headers.SetValue("If-Unmodified-Since", ifUnmodifiedSince.Value.ToString("R", System.Globalization.CultureInfo.InvariantCulture)); }
                if (ifMatch != null) { _request.Headers.SetValue("If-Match", ifMatch.Value.ToString()); }
                if (ifNoneMatch != null) { _request.Headers.SetValue("If-None-Match", ifNoneMatch.Value.ToString()); }
                if (requestId != null) { _request.Headers.SetValue("x-ms-client-request-id", requestId); }

                // Create the body
                if (queryRequest != null)
                {
                    System.Xml.Linq.XElement _body = Azure.Storage.QuickQuery.Models.QueryRequest.ToXml(queryRequest, "QueryRequest", "");
                    string _text = _body.ToString(System.Xml.Linq.SaveOptions.DisableFormatting);
                    _request.Headers.SetValue("Content-Type", "application/xml");
                    _request.Headers.SetValue("Content-Length", _text.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    _request.Content = Azure.Core.RequestContent.Create(System.Text.Encoding.UTF8.GetBytes(_text));
                }

                return _message;
            }

            /// <summary>
            /// Create the Blob.QuickQueryAsync response or throw a failure exception.
            /// </summary>
            /// <param name="clientDiagnostics">The ClientDiagnostics instance to use.</param>
            /// <param name="response">The raw Response.</param>
            /// <returns>The Blob.QuickQueryAsync Azure.Response{Azure.Storage.QuickQuery.Models.BlobQuickQueryResult}.</returns>
            internal static Azure.Response<Azure.Storage.QuickQuery.Models.BlobQuickQueryResult> QuickQueryAsync_CreateResponse(
                Azure.Core.Pipeline.ClientDiagnostics clientDiagnostics,
                Azure.Response response)
            {
                // Process the response
                switch (response.Status)
                {
                    case 200:
                    {
                        // Create the result
                        Azure.Storage.QuickQuery.Models.BlobQuickQueryResult _value = new Azure.Storage.QuickQuery.Models.BlobQuickQueryResult();
                        _value.Body = response.ContentStream; // You should manually wrap with RetriableStream!

                        // Get response headers
                        string _header;
                        if (response.Headers.TryGetValue("Last-Modified", out _header))
                        {
                            _value.LastModified = System.DateTimeOffset.Parse(_header, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        _value.Metadata = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
                        foreach (Azure.Core.HttpHeader _headerPair in response.Headers)
                        {
                            if (_headerPair.Name.StartsWith("x-ms-meta-", System.StringComparison.InvariantCulture))
                            {
                                _value.Metadata[_headerPair.Name.Substring(10)] = _headerPair.Value;
                            }
                        }
                        if (response.Headers.TryGetValue("Content-Length", out _header))
                        {
                            _value.ContentLength = long.Parse(_header, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        if (response.Headers.TryGetValue("Content-Type", out _header))
                        {
                            _value.ContentType = _header;
                        }
                        if (response.Headers.TryGetValue("Content-Range", out _header))
                        {
                            _value.ContentRange = _header;
                        }
                        if (response.Headers.TryGetValue("ETag", out _header))
                        {
                            _value.ETag = new Azure.ETag(_header);
                        }
                        if (response.Headers.TryGetValue("Content-MD5", out _header))
                        {
                            _value.ContentMD5 = System.Convert.FromBase64String(_header);
                        }
                        if (response.Headers.TryGetValue("Content-Encoding", out _header))
                        {
                            _value.ContentEncoding = _header;
                        }
                        if (response.Headers.TryGetValue("Cache-Control", out _header))
                        {
                            _value.CacheControl = _header;
                        }
                        if (response.Headers.TryGetValue("Content-Disposition", out _header))
                        {
                            _value.ContentDisposition = _header;
                        }
                        if (response.Headers.TryGetValue("Content-Language", out _header))
                        {
                            _value.ContentLanguage = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-blob-sequence-number", out _header))
                        {
                            _value.BlobSequenceNumber = long.Parse(_header, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        if (response.Headers.TryGetValue("x-ms-blob-type", out _header))
                        {
                            _value.BlobType = (Azure.Storage.QuickQuery.Models.BlobType)System.Enum.Parse(typeof(Azure.Storage.QuickQuery.Models.BlobType), _header, false);
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-completion-time", out _header))
                        {
                            _value.CopyCompletionTime = System.DateTimeOffset.Parse(_header, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-status-description", out _header))
                        {
                            _value.CopyStatusDescription = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-id", out _header))
                        {
                            _value.CopyId = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-progress", out _header))
                        {
                            _value.CopyProgress = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-source", out _header))
                        {
                            _value.CopySource = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-status", out _header))
                        {
                            _value.CopyStatus = Azure.Storage.QuickQuery.QuickQueryRestClient.Serialization.ParseCopyStatusType(_header);
                        }
                        if (response.Headers.TryGetValue("x-ms-lease-duration", out _header))
                        {
                            _value.LeaseDuration = Azure.Storage.QuickQuery.QuickQueryRestClient.Serialization.ParseLeaseDurationType(_header);
                        }
                        if (response.Headers.TryGetValue("x-ms-lease-state", out _header))
                        {
                            _value.LeaseState = Azure.Storage.QuickQuery.QuickQueryRestClient.Serialization.ParseLeaseStateType(_header);
                        }
                        if (response.Headers.TryGetValue("x-ms-lease-status", out _header))
                        {
                            _value.LeaseStatus = Azure.Storage.QuickQuery.QuickQueryRestClient.Serialization.ParseLeaseStatusType(_header);
                        }
                        if (response.Headers.TryGetValue("Accept-Ranges", out _header))
                        {
                            _value.AcceptRanges = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-blob-committed-block-count", out _header))
                        {
                            _value.BlobCommittedBlockCount = int.Parse(_header, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        if (response.Headers.TryGetValue("x-ms-server-encrypted", out _header))
                        {
                            _value.IsServerEncrypted = bool.Parse(_header);
                        }
                        if (response.Headers.TryGetValue("x-ms-encryption-key-sha256", out _header))
                        {
                            _value.EncryptionKeySha256 = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-encryption-scope", out _header))
                        {
                            _value.EncryptionScope = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-blob-content-md5", out _header))
                        {
                            _value.BlobContentMD5 = System.Convert.FromBase64String(_header);
                        }

                        // Create the response
                        return Response.FromValue(_value, response);
                    }
                    case 206:
                    {
                        // Create the result
                        Azure.Storage.QuickQuery.Models.BlobQuickQueryResult _value = new Azure.Storage.QuickQuery.Models.BlobQuickQueryResult();
                        _value.Body = response.ContentStream; // You should manually wrap with RetriableStream!

                        // Get response headers
                        string _header;
                        if (response.Headers.TryGetValue("Last-Modified", out _header))
                        {
                            _value.LastModified = System.DateTimeOffset.Parse(_header, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        _value.Metadata = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
                        foreach (Azure.Core.HttpHeader _headerPair in response.Headers)
                        {
                            if (_headerPair.Name.StartsWith("x-ms-meta-", System.StringComparison.InvariantCulture))
                            {
                                _value.Metadata[_headerPair.Name.Substring(10)] = _headerPair.Value;
                            }
                        }
                        if (response.Headers.TryGetValue("Content-Length", out _header))
                        {
                            _value.ContentLength = long.Parse(_header, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        if (response.Headers.TryGetValue("Content-Type", out _header))
                        {
                            _value.ContentType = _header;
                        }
                        if (response.Headers.TryGetValue("Content-Range", out _header))
                        {
                            _value.ContentRange = _header;
                        }
                        if (response.Headers.TryGetValue("ETag", out _header))
                        {
                            _value.ETag = new Azure.ETag(_header);
                        }
                        if (response.Headers.TryGetValue("Content-MD5", out _header))
                        {
                            _value.ContentMD5 = System.Convert.FromBase64String(_header);
                        }
                        if (response.Headers.TryGetValue("Content-Encoding", out _header))
                        {
                            _value.ContentEncoding = _header;
                        }
                        if (response.Headers.TryGetValue("Cache-Control", out _header))
                        {
                            _value.CacheControl = _header;
                        }
                        if (response.Headers.TryGetValue("Content-Disposition", out _header))
                        {
                            _value.ContentDisposition = _header;
                        }
                        if (response.Headers.TryGetValue("Content-Language", out _header))
                        {
                            _value.ContentLanguage = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-blob-sequence-number", out _header))
                        {
                            _value.BlobSequenceNumber = long.Parse(_header, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        if (response.Headers.TryGetValue("x-ms-blob-type", out _header))
                        {
                            _value.BlobType = (Azure.Storage.QuickQuery.Models.BlobType)System.Enum.Parse(typeof(Azure.Storage.QuickQuery.Models.BlobType), _header, false);
                        }
                        if (response.Headers.TryGetValue("x-ms-content-crc64", out _header))
                        {
                            _value.ContentCrc64 = System.Convert.FromBase64String(_header);
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-completion-time", out _header))
                        {
                            _value.CopyCompletionTime = System.DateTimeOffset.Parse(_header, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-status-description", out _header))
                        {
                            _value.CopyStatusDescription = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-id", out _header))
                        {
                            _value.CopyId = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-progress", out _header))
                        {
                            _value.CopyProgress = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-source", out _header))
                        {
                            _value.CopySource = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-copy-status", out _header))
                        {
                            _value.CopyStatus = Azure.Storage.QuickQuery.QuickQueryRestClient.Serialization.ParseCopyStatusType(_header);
                        }
                        if (response.Headers.TryGetValue("x-ms-lease-duration", out _header))
                        {
                            _value.LeaseDuration = Azure.Storage.QuickQuery.QuickQueryRestClient.Serialization.ParseLeaseDurationType(_header);
                        }
                        if (response.Headers.TryGetValue("x-ms-lease-state", out _header))
                        {
                            _value.LeaseState = Azure.Storage.QuickQuery.QuickQueryRestClient.Serialization.ParseLeaseStateType(_header);
                        }
                        if (response.Headers.TryGetValue("x-ms-lease-status", out _header))
                        {
                            _value.LeaseStatus = Azure.Storage.QuickQuery.QuickQueryRestClient.Serialization.ParseLeaseStatusType(_header);
                        }
                        if (response.Headers.TryGetValue("Accept-Ranges", out _header))
                        {
                            _value.AcceptRanges = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-blob-committed-block-count", out _header))
                        {
                            _value.BlobCommittedBlockCount = int.Parse(_header, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        if (response.Headers.TryGetValue("x-ms-server-encrypted", out _header))
                        {
                            _value.IsServerEncrypted = bool.Parse(_header);
                        }
                        if (response.Headers.TryGetValue("x-ms-encryption-key-sha256", out _header))
                        {
                            _value.EncryptionKeySha256 = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-encryption-scope", out _header))
                        {
                            _value.EncryptionScope = _header;
                        }
                        if (response.Headers.TryGetValue("x-ms-blob-content-md5", out _header))
                        {
                            _value.BlobContentMD5 = System.Convert.FromBase64String(_header);
                        }

                        // Create the response
                        return Response.FromValue(_value, response);
                    }
                    default:
                    {
                        // Create the result
                        System.Xml.Linq.XDocument _xml = System.Xml.Linq.XDocument.Load(response.ContentStream, System.Xml.Linq.LoadOptions.PreserveWhitespace);
                        Azure.Storage.QuickQuery.Models.StorageError _value = Azure.Storage.QuickQuery.Models.StorageError.FromXml(_xml.Root);

                        throw _value.CreateException(clientDiagnostics, response);
                    }
                }
            }
            #endregion Blob.QuickQueryAsync
        }
        #endregion Blob operations
    }
}
#endregion Service

#region Models
#region class BlobQuickQueryResult
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// Blob QuickQueryResult
    /// </summary>
    internal partial class BlobQuickQueryResult
    {
        /// <summary>
        /// Returns the date and time the container was last modified. Any operation that modifies the blob, including an update of the blob's metadata or properties, changes the last-modified time of the blob.
        /// </summary>
        public System.DateTimeOffset LastModified { get; internal set; }

        /// <summary>
        /// x-ms-meta
        /// </summary>
        public System.Collections.Generic.IDictionary<string, string> Metadata { get; internal set; }

        /// <summary>
        /// The number of bytes present in the response body.
        /// </summary>
        public long ContentLength { get; internal set; }

        /// <summary>
        /// The media type of the body of the response. For Download Blob this is 'application/octet-stream'
        /// </summary>
        public string ContentType { get; internal set; }

        /// <summary>
        /// Indicates the range of bytes returned in the event that the client requested a subset of the blob by setting the 'Range' request header.
        /// </summary>
        public string ContentRange { get; internal set; }

        /// <summary>
        /// The ETag contains a value that you can use to perform operations conditionally. If the request version is 2011-08-18 or newer, the ETag value will be in quotes.
        /// </summary>
        public Azure.ETag ETag { get; internal set; }

        /// <summary>
        /// If the blob has an MD5 hash and this operation is to read the full blob, this response header is returned so that the client can check for message content integrity.
        /// </summary>
        #pragma warning disable CA1819 // Properties should not return arrays
        public byte[] ContentMD5 { get; internal set; }
        #pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// This header returns the value that was specified for the Content-Encoding request header
        /// </summary>
        public string ContentEncoding { get; internal set; }

        /// <summary>
        /// This header is returned if it was previously specified for the blob.
        /// </summary>
        public string CacheControl { get; internal set; }

        /// <summary>
        /// This header returns the value that was specified for the 'x-ms-blob-content-disposition' header. The Content-Disposition response header field conveys additional information about how to process the response payload, and also can be used to attach additional metadata. For example, if set to attachment, it indicates that the user-agent should not display the response, but instead show a Save As dialog with a filename other than the blob name specified.
        /// </summary>
        public string ContentDisposition { get; internal set; }

        /// <summary>
        /// This header returns the value that was specified for the Content-Language request header.
        /// </summary>
        public string ContentLanguage { get; internal set; }

        /// <summary>
        /// The current sequence number for a page blob. This header is not returned for block blobs or append blobs
        /// </summary>
        public long BlobSequenceNumber { get; internal set; }

        /// <summary>
        /// The blob's type.
        /// </summary>
        public Azure.Storage.QuickQuery.Models.BlobType BlobType { get; internal set; }

        /// <summary>
        /// If the request is to read a specified range and the x-ms-range-get-content-crc64 is set to true, then the request returns a crc64 for the range, as long as the range size is less than or equal to 4 MB. If both x-ms-range-get-content-crc64 and x-ms-range-get-content-md5 is specified in the same request, it will fail with 400(Bad Request)
        /// </summary>
        #pragma warning disable CA1819 // Properties should not return arrays
        public byte[] ContentCrc64 { get; internal set; }
        #pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Conclusion time of the last attempted Copy Blob operation where this blob was the destination blob. This value can specify the time of a completed, aborted, or failed copy attempt. This header does not appear if a copy is pending, if this blob has never been the destination in a Copy Blob operation, or if this blob has been modified after a concluded Copy Blob operation using Set Blob Properties, Put Blob, or Put Block List.
        /// </summary>
        public System.DateTimeOffset CopyCompletionTime { get; internal set; }

        /// <summary>
        /// Only appears when x-ms-copy-status is failed or pending. Describes the cause of the last fatal or non-fatal copy operation failure. This header does not appear if this blob has never been the destination in a Copy Blob operation, or if this blob has been modified after a concluded Copy Blob operation using Set Blob Properties, Put Blob, or Put Block List
        /// </summary>
        public string CopyStatusDescription { get; internal set; }

        /// <summary>
        /// String identifier for this copy operation. Use with Get Blob Properties to check the status of this copy operation, or pass to Abort Copy Blob to abort a pending copy.
        /// </summary>
        public string CopyId { get; internal set; }

        /// <summary>
        /// Contains the number of bytes copied and the total bytes in the source in the last attempted Copy Blob operation where this blob was the destination blob. Can show between 0 and Content-Length bytes copied. This header does not appear if this blob has never been the destination in a Copy Blob operation, or if this blob has been modified after a concluded Copy Blob operation using Set Blob Properties, Put Blob, or Put Block List
        /// </summary>
        public string CopyProgress { get; internal set; }

        /// <summary>
        /// URL up to 2 KB in length that specifies the source blob or file used in the last attempted Copy Blob operation where this blob was the destination blob. This header does not appear if this blob has never been the destination in a Copy Blob operation, or if this blob has been modified after a concluded Copy Blob operation using Set Blob Properties, Put Blob, or Put Block List.
        /// </summary>
        public string CopySource { get; internal set; }

        /// <summary>
        /// State of the copy operation identified by x-ms-copy-id.
        /// </summary>
        public Azure.Storage.QuickQuery.Models.CopyStatusType CopyStatus { get; internal set; }

        /// <summary>
        /// When a blob is leased, specifies whether the lease is of infinite or fixed duration.
        /// </summary>
        public Azure.Storage.QuickQuery.Models.LeaseDurationType LeaseDuration { get; internal set; }

        /// <summary>
        /// Lease state of the blob.
        /// </summary>
        public Azure.Storage.QuickQuery.Models.LeaseStateType LeaseState { get; internal set; }

        /// <summary>
        /// The current lease status of the blob.
        /// </summary>
        public Azure.Storage.QuickQuery.Models.LeaseStatusType LeaseStatus { get; internal set; }

        /// <summary>
        /// Indicates that the service supports requests for partial blob content.
        /// </summary>
        public string AcceptRanges { get; internal set; }

        /// <summary>
        /// The number of committed blocks present in the blob. This header is returned only for append blobs.
        /// </summary>
        public int BlobCommittedBlockCount { get; internal set; }

        /// <summary>
        /// The value of this header is set to true if the blob data and application metadata are completely encrypted using the specified algorithm. Otherwise, the value is set to false (when the blob is unencrypted, or if only parts of the blob/application metadata are encrypted).
        /// </summary>
        public bool IsServerEncrypted { get; internal set; }

        /// <summary>
        /// The SHA-256 hash of the encryption key used to encrypt the blob. This header is only returned when the blob was encrypted with a customer-provided key.
        /// </summary>
        public string EncryptionKeySha256 { get; internal set; }

        /// <summary>
        /// Returns the name of the encryption scope used to encrypt the blob contents and application metadata.  Note that the absence of this header implies use of the default account encryption scope.
        /// </summary>
        public string EncryptionScope { get; internal set; }

        /// <summary>
        /// If the blob has a MD5 hash, and if request contains range header (Range or x-ms-range), this response header is returned with the value of the whole blob's MD5 value. This value may or may not be equal to the value returned in Content-MD5 header, with the latter calculated from the requested range
        /// </summary>
        #pragma warning disable CA1819 // Properties should not return arrays
        public byte[] BlobContentMD5 { get; internal set; }
        #pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Body
        /// </summary>
        public System.IO.Stream Body { get; internal set; }

        /// <summary>
        /// Creates a new BlobQuickQueryResult instance
        /// </summary>
        public BlobQuickQueryResult()
        {
            Metadata = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
        }
    }
}
#endregion class BlobQuickQueryResult

#region enum BlobType
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// The blob's type.
    /// </summary>
    public enum BlobType
    {
        /// <summary>
        /// BlockBlob
        /// </summary>
        BlockBlob,

        /// <summary>
        /// PageBlob
        /// </summary>
        PageBlob,

        /// <summary>
        /// AppendBlob
        /// </summary>
        AppendBlob
    }
}
#endregion enum BlobType

#region enum CopyStatusType
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// State of the copy operation identified by x-ms-copy-id.
    /// </summary>
    public enum CopyStatusType
    {
        /// <summary>
        /// pending
        /// </summary>
        Pending,

        /// <summary>
        /// success
        /// </summary>
        Success,

        /// <summary>
        /// aborted
        /// </summary>
        Aborted,

        /// <summary>
        /// failed
        /// </summary>
        Failed
    }
}

namespace Azure.Storage.QuickQuery
{
    internal static partial class QuickQueryRestClient
    {
        public static partial class Serialization
        {
            public static string ToString(Azure.Storage.QuickQuery.Models.CopyStatusType value)
            {
                return value switch
                {
                    Azure.Storage.QuickQuery.Models.CopyStatusType.Pending => "pending",
                    Azure.Storage.QuickQuery.Models.CopyStatusType.Success => "success",
                    Azure.Storage.QuickQuery.Models.CopyStatusType.Aborted => "aborted",
                    Azure.Storage.QuickQuery.Models.CopyStatusType.Failed => "failed",
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.CopyStatusType value.")
                };
            }

            public static Azure.Storage.QuickQuery.Models.CopyStatusType ParseCopyStatusType(string value)
            {
                return value switch
                {
                    "pending" => Azure.Storage.QuickQuery.Models.CopyStatusType.Pending,
                    "success" => Azure.Storage.QuickQuery.Models.CopyStatusType.Success,
                    "aborted" => Azure.Storage.QuickQuery.Models.CopyStatusType.Aborted,
                    "failed" => Azure.Storage.QuickQuery.Models.CopyStatusType.Failed,
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.CopyStatusType value.")
                };
            }
        }
    }
}
#endregion enum CopyStatusType

#region class DelimitedTextConfigurationInternal
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// delimited text configuration
    /// </summary>
    internal partial class DelimitedTextConfigurationInternal
    {
        /// <summary>
        /// column separator
        /// </summary>
        public string ColumnSeparator { get; set; }

        /// <summary>
        /// field quote
        /// </summary>
        public string FieldQuote { get; set; }

        /// <summary>
        /// record separator
        /// </summary>
        public string RecordSeparator { get; set; }

        /// <summary>
        /// escape char
        /// </summary>
        public string EscapeChar { get; set; }

        /// <summary>
        /// has headers
        /// </summary>
        public string HasHeaders { get; set; }

        /// <summary>
        /// Creates a new DelimitedTextConfigurationInternal instance
        /// </summary>
        public DelimitedTextConfigurationInternal() { }

        /// <summary>
        /// Serialize a DelimitedTextConfigurationInternal instance as XML.
        /// </summary>
        /// <param name="value">The DelimitedTextConfigurationInternal instance to serialize.</param>
        /// <param name="name">An optional name to use for the root element instead of "DelimitedTextConfiguration".</param>
        /// <param name="ns">An optional namespace to use for the root element instead of "".</param>
        /// <returns>The serialized XML element.</returns>
        internal static System.Xml.Linq.XElement ToXml(Azure.Storage.QuickQuery.Models.DelimitedTextConfigurationInternal value, string name = "DelimitedTextConfiguration", string ns = "")
        {
            System.Diagnostics.Debug.Assert(value != null);
            System.Xml.Linq.XElement _element = new System.Xml.Linq.XElement(System.Xml.Linq.XName.Get(name, ns));
            _element.Add(new System.Xml.Linq.XElement(
                System.Xml.Linq.XName.Get("ColumnSeparator", ""),
                value.ColumnSeparator));
            _element.Add(new System.Xml.Linq.XElement(
                System.Xml.Linq.XName.Get("FieldQuote", ""),
                value.FieldQuote));
            _element.Add(new System.Xml.Linq.XElement(
                System.Xml.Linq.XName.Get("RecordSeparator", ""),
                value.RecordSeparator));
            _element.Add(new System.Xml.Linq.XElement(
                System.Xml.Linq.XName.Get("EscapeChar", ""),
                value.EscapeChar));
            _element.Add(new System.Xml.Linq.XElement(
                System.Xml.Linq.XName.Get("HasHeaders", ""),
                value.HasHeaders));
            return _element;
        }
    }
}
#endregion class DelimitedTextConfigurationInternal

#region enum EncryptionAlgorithmType
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// The algorithm used to produce the encryption key hash. Currently, the only accepted value is "AES256". Must be provided if the x-ms-encryption-key header is provided.
    /// </summary>
    public enum EncryptionAlgorithmType
    {
        /// <summary>
        /// AES256
        /// </summary>
        Aes256
    }
}

namespace Azure.Storage.QuickQuery
{
    internal static partial class QuickQueryRestClient
    {
        public static partial class Serialization
        {
            public static string ToString(Azure.Storage.QuickQuery.Models.EncryptionAlgorithmType value)
            {
                return value switch
                {
                    Azure.Storage.QuickQuery.Models.EncryptionAlgorithmType.Aes256 => "AES256",
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.EncryptionAlgorithmType value.")
                };
            }

            public static Azure.Storage.QuickQuery.Models.EncryptionAlgorithmType ParseEncryptionAlgorithmType(string value)
            {
                return value switch
                {
                    "AES256" => Azure.Storage.QuickQuery.Models.EncryptionAlgorithmType.Aes256,
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.EncryptionAlgorithmType value.")
                };
            }
        }
    }
}
#endregion enum EncryptionAlgorithmType

#region class JsonTextConfigurationInternal
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// json text configuration
    /// </summary>
    internal partial class JsonTextConfigurationInternal
    {
        /// <summary>
        /// record separator
        /// </summary>
        public string RecordSeparator { get; set; }

        /// <summary>
        /// Creates a new JsonTextConfigurationInternal instance
        /// </summary>
        public JsonTextConfigurationInternal() { }

        /// <summary>
        /// Serialize a JsonTextConfigurationInternal instance as XML.
        /// </summary>
        /// <param name="value">The JsonTextConfigurationInternal instance to serialize.</param>
        /// <param name="name">An optional name to use for the root element instead of "JsonTextConfiguration".</param>
        /// <param name="ns">An optional namespace to use for the root element instead of "".</param>
        /// <returns>The serialized XML element.</returns>
        internal static System.Xml.Linq.XElement ToXml(Azure.Storage.QuickQuery.Models.JsonTextConfigurationInternal value, string name = "JsonTextConfiguration", string ns = "")
        {
            System.Diagnostics.Debug.Assert(value != null);
            System.Xml.Linq.XElement _element = new System.Xml.Linq.XElement(System.Xml.Linq.XName.Get(name, ns));
            _element.Add(new System.Xml.Linq.XElement(
                System.Xml.Linq.XName.Get("RecordSeparator", ""),
                value.RecordSeparator));
            return _element;
        }
    }
}
#endregion class JsonTextConfigurationInternal

#region enum LeaseDurationType
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// When a blob is leased, specifies whether the lease is of infinite or fixed duration.
    /// </summary>
    public enum LeaseDurationType
    {
        /// <summary>
        /// infinite
        /// </summary>
        Infinite,

        /// <summary>
        /// fixed
        /// </summary>
        Fixed
    }
}

namespace Azure.Storage.QuickQuery
{
    internal static partial class QuickQueryRestClient
    {
        public static partial class Serialization
        {
            public static string ToString(Azure.Storage.QuickQuery.Models.LeaseDurationType value)
            {
                return value switch
                {
                    Azure.Storage.QuickQuery.Models.LeaseDurationType.Infinite => "infinite",
                    Azure.Storage.QuickQuery.Models.LeaseDurationType.Fixed => "fixed",
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.LeaseDurationType value.")
                };
            }

            public static Azure.Storage.QuickQuery.Models.LeaseDurationType ParseLeaseDurationType(string value)
            {
                return value switch
                {
                    "infinite" => Azure.Storage.QuickQuery.Models.LeaseDurationType.Infinite,
                    "fixed" => Azure.Storage.QuickQuery.Models.LeaseDurationType.Fixed,
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.LeaseDurationType value.")
                };
            }
        }
    }
}
#endregion enum LeaseDurationType

#region enum LeaseStateType
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// Lease state of the blob.
    /// </summary>
    public enum LeaseStateType
    {
        /// <summary>
        /// available
        /// </summary>
        Available,

        /// <summary>
        /// leased
        /// </summary>
        Leased,

        /// <summary>
        /// expired
        /// </summary>
        Expired,

        /// <summary>
        /// breaking
        /// </summary>
        Breaking,

        /// <summary>
        /// broken
        /// </summary>
        Broken
    }
}

namespace Azure.Storage.QuickQuery
{
    internal static partial class QuickQueryRestClient
    {
        public static partial class Serialization
        {
            public static string ToString(Azure.Storage.QuickQuery.Models.LeaseStateType value)
            {
                return value switch
                {
                    Azure.Storage.QuickQuery.Models.LeaseStateType.Available => "available",
                    Azure.Storage.QuickQuery.Models.LeaseStateType.Leased => "leased",
                    Azure.Storage.QuickQuery.Models.LeaseStateType.Expired => "expired",
                    Azure.Storage.QuickQuery.Models.LeaseStateType.Breaking => "breaking",
                    Azure.Storage.QuickQuery.Models.LeaseStateType.Broken => "broken",
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.LeaseStateType value.")
                };
            }

            public static Azure.Storage.QuickQuery.Models.LeaseStateType ParseLeaseStateType(string value)
            {
                return value switch
                {
                    "available" => Azure.Storage.QuickQuery.Models.LeaseStateType.Available,
                    "leased" => Azure.Storage.QuickQuery.Models.LeaseStateType.Leased,
                    "expired" => Azure.Storage.QuickQuery.Models.LeaseStateType.Expired,
                    "breaking" => Azure.Storage.QuickQuery.Models.LeaseStateType.Breaking,
                    "broken" => Azure.Storage.QuickQuery.Models.LeaseStateType.Broken,
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.LeaseStateType value.")
                };
            }
        }
    }
}
#endregion enum LeaseStateType

#region enum LeaseStatusType
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// The current lease status of the blob.
    /// </summary>
    public enum LeaseStatusType
    {
        /// <summary>
        /// locked
        /// </summary>
        Locked,

        /// <summary>
        /// unlocked
        /// </summary>
        Unlocked
    }
}

namespace Azure.Storage.QuickQuery
{
    internal static partial class QuickQueryRestClient
    {
        public static partial class Serialization
        {
            public static string ToString(Azure.Storage.QuickQuery.Models.LeaseStatusType value)
            {
                return value switch
                {
                    Azure.Storage.QuickQuery.Models.LeaseStatusType.Locked => "locked",
                    Azure.Storage.QuickQuery.Models.LeaseStatusType.Unlocked => "unlocked",
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.LeaseStatusType value.")
                };
            }

            public static Azure.Storage.QuickQuery.Models.LeaseStatusType ParseLeaseStatusType(string value)
            {
                return value switch
                {
                    "locked" => Azure.Storage.QuickQuery.Models.LeaseStatusType.Locked,
                    "unlocked" => Azure.Storage.QuickQuery.Models.LeaseStatusType.Unlocked,
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.LeaseStatusType value.")
                };
            }
        }
    }
}
#endregion enum LeaseStatusType

#region class QueryRequest
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// the quick query body
    /// </summary>
    internal partial class QueryRequest
    {
        /// <summary>
        /// the query type
        /// </summary>
        public string QueryType { get; set; }

        /// <summary>
        /// a query statement
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// InputSerialization
        /// </summary>
        public Azure.Storage.QuickQuery.Models.QuickQuerySerialization InputSerialization { get; set; }

        /// <summary>
        /// OutputSerialization
        /// </summary>
        public Azure.Storage.QuickQuery.Models.QuickQuerySerialization OutputSerialization { get; set; }

        /// <summary>
        /// Creates a new QueryRequest instance
        /// </summary>
        public QueryRequest()
        {
            InputSerialization = new Azure.Storage.QuickQuery.Models.QuickQuerySerialization();
            OutputSerialization = new Azure.Storage.QuickQuery.Models.QuickQuerySerialization();
        }

        /// <summary>
        /// Serialize a QueryRequest instance as XML.
        /// </summary>
        /// <param name="value">The QueryRequest instance to serialize.</param>
        /// <param name="name">An optional name to use for the root element instead of "QueryRequest".</param>
        /// <param name="ns">An optional namespace to use for the root element instead of "".</param>
        /// <returns>The serialized XML element.</returns>
        internal static System.Xml.Linq.XElement ToXml(Azure.Storage.QuickQuery.Models.QueryRequest value, string name = "QueryRequest", string ns = "")
        {
            System.Diagnostics.Debug.Assert(value != null);
            System.Xml.Linq.XElement _element = new System.Xml.Linq.XElement(System.Xml.Linq.XName.Get(name, ns));
            _element.Add(new System.Xml.Linq.XElement(
                System.Xml.Linq.XName.Get("QueryType", ""),
                value.QueryType));
            _element.Add(new System.Xml.Linq.XElement(
                System.Xml.Linq.XName.Get("Expression", ""),
                value.Expression));
            if (value.InputSerialization != null)
            {
                _element.Add(Azure.Storage.QuickQuery.Models.QuickQuerySerialization.ToXml(value.InputSerialization, "InputSerialization", ""));
            }
            if (value.OutputSerialization != null)
            {
                _element.Add(Azure.Storage.QuickQuery.Models.QuickQuerySerialization.ToXml(value.OutputSerialization, "OutputSerialization", ""));
            }
            return _element;
        }
    }
}
#endregion class QueryRequest

#region class QuickQueryFormat
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// QuickQueryFormat
    /// </summary>
    internal partial class QuickQueryFormat
    {
        /// <summary>
        /// the quick query type
        /// </summary>
        public Azure.Storage.QuickQuery.Models.QuickQueryType QuickQueryType { get; set; }

        /// <summary>
        /// delimited text configuration
        /// </summary>
        public Azure.Storage.QuickQuery.Models.DelimitedTextConfigurationInternal DelimitedTextConfiguration { get; set; }

        /// <summary>
        /// json text configuration
        /// </summary>
        public Azure.Storage.QuickQuery.Models.JsonTextConfigurationInternal JsonTextConfiguration { get; set; }

        /// <summary>
        /// Creates a new QuickQueryFormat instance
        /// </summary>
        public QuickQueryFormat()
        {
            DelimitedTextConfiguration = new Azure.Storage.QuickQuery.Models.DelimitedTextConfigurationInternal();
            JsonTextConfiguration = new Azure.Storage.QuickQuery.Models.JsonTextConfigurationInternal();
        }

        /// <summary>
        /// Serialize a QuickQueryFormat instance as XML.
        /// </summary>
        /// <param name="value">The QuickQueryFormat instance to serialize.</param>
        /// <param name="name">An optional name to use for the root element instead of "QuickQueryFormat".</param>
        /// <param name="ns">An optional namespace to use for the root element instead of "".</param>
        /// <returns>The serialized XML element.</returns>
        internal static System.Xml.Linq.XElement ToXml(Azure.Storage.QuickQuery.Models.QuickQueryFormat value, string name = "QuickQueryFormat", string ns = "")
        {
            System.Diagnostics.Debug.Assert(value != null);
            System.Xml.Linq.XElement _element = new System.Xml.Linq.XElement(System.Xml.Linq.XName.Get(name, ns));
            _element.Add(new System.Xml.Linq.XElement(
                System.Xml.Linq.XName.Get("QuickQueryType", ""),
                Azure.Storage.QuickQuery.QuickQueryRestClient.Serialization.ToString(value.QuickQueryType)));
            if (value.DelimitedTextConfiguration != null)
            {
                _element.Add(Azure.Storage.QuickQuery.Models.DelimitedTextConfigurationInternal.ToXml(value.DelimitedTextConfiguration, "DelimitedTextConfiguration", ""));
            }
            if (value.JsonTextConfiguration != null)
            {
                _element.Add(Azure.Storage.QuickQuery.Models.JsonTextConfigurationInternal.ToXml(value.JsonTextConfiguration, "JsonTextConfiguration", ""));
            }
            return _element;
        }
    }
}
#endregion class QuickQueryFormat

#region class QuickQuerySerialization
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// QuickQuerySerialization
    /// </summary>
    internal partial class QuickQuerySerialization
    {
        /// <summary>
        /// Format
        /// </summary>
        public Azure.Storage.QuickQuery.Models.QuickQueryFormat Format { get; set; }

        /// <summary>
        /// Creates a new QuickQuerySerialization instance
        /// </summary>
        public QuickQuerySerialization()
        {
            Format = new Azure.Storage.QuickQuery.Models.QuickQueryFormat();
        }

        /// <summary>
        /// Serialize a QuickQuerySerialization instance as XML.
        /// </summary>
        /// <param name="value">The QuickQuerySerialization instance to serialize.</param>
        /// <param name="name">An optional name to use for the root element instead of "QuickQuerySerialization".</param>
        /// <param name="ns">An optional namespace to use for the root element instead of "".</param>
        /// <returns>The serialized XML element.</returns>
        internal static System.Xml.Linq.XElement ToXml(Azure.Storage.QuickQuery.Models.QuickQuerySerialization value, string name = "QuickQuerySerialization", string ns = "")
        {
            System.Diagnostics.Debug.Assert(value != null);
            System.Xml.Linq.XElement _element = new System.Xml.Linq.XElement(System.Xml.Linq.XName.Get(name, ns));
            _element.Add(Azure.Storage.QuickQuery.Models.QuickQueryFormat.ToXml(value.Format, "Format", ""));
            return _element;
        }
    }
}
#endregion class QuickQuerySerialization

#region enum QuickQueryType
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// the quick query type
    /// </summary>
    public enum QuickQueryType
    {
        /// <summary>
        /// delimited
        /// </summary>
        Delimited,

        /// <summary>
        /// json
        /// </summary>
        Json
    }
}

namespace Azure.Storage.QuickQuery
{
    internal static partial class QuickQueryRestClient
    {
        public static partial class Serialization
        {
            public static string ToString(Azure.Storage.QuickQuery.Models.QuickQueryType value)
            {
                return value switch
                {
                    Azure.Storage.QuickQuery.Models.QuickQueryType.Delimited => "delimited",
                    Azure.Storage.QuickQuery.Models.QuickQueryType.Json => "json",
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.QuickQueryType value.")
                };
            }

            public static Azure.Storage.QuickQuery.Models.QuickQueryType ParseQuickQueryType(string value)
            {
                return value switch
                {
                    "delimited" => Azure.Storage.QuickQuery.Models.QuickQueryType.Delimited,
                    "json" => Azure.Storage.QuickQuery.Models.QuickQueryType.Json,
                    _ => throw new System.ArgumentOutOfRangeException(nameof(value), value, "Unknown Azure.Storage.QuickQuery.Models.QuickQueryType value.")
                };
            }
        }
    }
}
#endregion enum QuickQueryType

#region class StorageError
namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// StorageError
    /// </summary>
    internal partial class StorageError
    {
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; internal set; }

        /// <summary>
        /// Prevent direct instantiation of StorageError instances.
        /// You can use QuickQueryModelFactory.StorageError instead.
        /// </summary>
        internal StorageError() { }

        /// <summary>
        /// Deserializes XML into a new StorageError instance.
        /// </summary>
        /// <param name="element">The XML element to deserialize.</param>
        /// <returns>A deserialized StorageError instance.</returns>
        internal static Azure.Storage.QuickQuery.Models.StorageError FromXml(System.Xml.Linq.XElement element)
        {
            System.Diagnostics.Debug.Assert(element != null);
            System.Xml.Linq.XElement _child;
            Azure.Storage.QuickQuery.Models.StorageError _value = new Azure.Storage.QuickQuery.Models.StorageError();
            _child = element.Element(System.Xml.Linq.XName.Get("Message", ""));
            if (_child != null)
            {
                _value.Message = _child.Value;
            }
            _child = element.Element(System.Xml.Linq.XName.Get("Code", ""));
            if (_child != null)
            {
                _value.Code = _child.Value;
            }
            CustomizeFromXml(element, _value);
            return _value;
        }

        static partial void CustomizeFromXml(System.Xml.Linq.XElement element, Azure.Storage.QuickQuery.Models.StorageError value);
    }
}
#endregion class StorageError
#endregion Models

