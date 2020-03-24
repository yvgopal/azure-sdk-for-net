// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.QuickQuery.Models;

namespace Azure.Storage.QuickQuery
{
    /// <summary>
    ///  Blob Quick Query client.
    /// </summary>
    public class BlobQuickQueryClient
    {
        /// <summary>
        /// Gets the blob service's primary <see cref="Uri"/> endpoint.
        /// </summary>
        public virtual Uri Uri { get; }

        /// <summary>
        /// The <see cref="HttpPipeline"/> transport pipeline used to send
        /// every request.
        /// </summary>
        internal virtual HttpPipeline Pipeline { get; }

        /// <summary>
        /// The version of the service to use when sending requests.
        /// </summary>
        internal virtual BlobClientOptions.ServiceVersion Version { get; }

        /// <summary>
        /// The <see cref="ClientDiagnostics"/> instance used to create diagnostic scopes
        /// every request.
        /// </summary>
        internal virtual ClientDiagnostics ClientDiagnostics { get; }

        /// <summary>
        /// The <see cref="CustomerProvidedKey"/> to be used when sending requests.
        /// </summary>
        internal virtual CustomerProvidedKey? CustomerProvidedKey { get; }

        #region ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobQuickQueryClient"/>
        /// class for mocking.
        /// </summary>
        protected BlobQuickQueryClient() { }

        internal BlobQuickQueryClient(
            Uri uri,
            HttpPipeline pipeline,
            BlobClientOptions.ServiceVersion serviceVersion,
            ClientDiagnostics clientDiagnostics,
            CustomerProvidedKey? customerProvidedKey)
        {
            Uri = uri;
            Pipeline = pipeline;
            Version = serviceVersion;
            ClientDiagnostics = clientDiagnostics;
            CustomerProvidedKey = customerProvidedKey;
        }

        /// <summary>
        /// Helper to access protected static members of BlobServiceClient
        /// that should not be exposed directly to customers.
        /// </summary>
        internal class BlobServiceClientInternals : BlobServiceClient
        {
            /// <summary>
            /// Prevent instantiation.
            /// </summary>
            private BlobServiceClientInternals() { }

            /// <summary>
            /// Get a <see cref="BlobServiceClient"/>'s <see cref="HttpPipeline"/>
            /// for creating child clients.
            /// </summary>
            /// <param name="client">The BlobServiceClient.</param>
            /// <returns>The BlobServiceClient's HttpPipeline.</returns>
            public static new HttpPipeline GetHttpPipeline(BlobServiceClient client) =>
                BlobServiceClient.GetHttpPipeline(client);

            /// <summary>
            /// Get a <see cref="BlobServiceClient"/>'s authentication
            /// <see cref="HttpPipelinePolicy"/> for creating child clients.
            /// </summary>
            /// <param name="client">The BlobServiceClient.</param>
            /// <returns>The BlobServiceClient's authentication policy.</returns>
            public static new HttpPipelinePolicy GetAuthenticationPolicy(BlobServiceClient client) =>
                BlobServiceClient.GetAuthenticationPolicy(client);

            /// <summary>
            /// Get a <see cref="BlobServiceClient"/>'s <see cref="BlobClientOptions"/>
            /// for creating child clients.
            /// </summary>
            /// <param name="client">The BlobServiceClient.</param>
            /// <returns>The BlobServiceClient's BlobClientOptions.</returns>
            public static new BlobClientOptions GetClientOptions(BlobServiceClient client) =>
                BlobServiceClient.GetClientOptions(client);
        }
        #endregion ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobQuickQueryClient"/>
        /// class with an identical <see cref="Uri"/> source but the specified
        /// <paramref name="snapshot"/> timestamp.
        ///
        /// For more information, see <see href="https://docs.microsoft.com/en-us/rest/api/storageservices/creating-a-snapshot-of-a-blob" />.
        /// </summary>
        /// <param name="snapshot">The snapshot identifier.</param>
        /// <returns>A new <see cref="BlobQuickQueryClient"/> instance.</returns>
        /// <remarks>
        /// Pass null or empty string to remove the snapshot returning a URL
        /// to the base blob.
        /// </remarks>
        public BlobQuickQueryClient WithSnapshot(string snapshot)
        {
            var builder = new BlobUriBuilder(Uri) { Snapshot = snapshot };
            return new BlobQuickQueryClient(
                builder.ToUri(),
                Pipeline,
                Version,
                ClientDiagnostics,
                CustomerProvidedKey);
        }

        /// <summary>
        /// The <see cref="Query"/> API returns the
        /// result of a query against the blob.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="inputTextConfiguration">
        /// Optional input text configuration.
        /// </param>
        /// <param name="outputTextConfiguration">
        /// Optional output text configuration.
        /// </param>
        /// <param name="errorReceiver">
        /// Optional error receiver.
        /// </param>
        /// <param name="conditions">
        /// Optional request conditions.
        /// </param>
        /// <param name="progressReceiver">
        /// Optional progress receiver.
        /// </param>
        /// <param name="cancellationToken">
        /// Optional <see cref="CancellationToken"/> to propagate
        /// notifications that the operation should be cancelled.
        /// </param>
        /// <remarks>
        /// A <see cref="RequestFailedException"/> will be thrown if
        /// a failure occurs.
        /// </remarks>
        /// <returns>
        /// A <see cref="Response{BlobQueryInfo}"/>.
        /// </returns>
        public virtual Response<BlobDownloadInfo> Query(
            string query,
            BlobQueryTextConfiguration inputTextConfiguration = default,
            BlobQueryTextConfiguration outputTextConfiguration = default,
            IBlobQueryErrorReceiver errorReceiver = default,
            BlobRequestConditions conditions = default,
            IProgress<long> progressReceiver = default,
            CancellationToken cancellationToken = default) =>
            QueryInternal(
                query,
                inputTextConfiguration,
                outputTextConfiguration,
                errorReceiver,
                conditions,
                progressReceiver,
                async: false,
                cancellationToken)
            .EnsureCompleted();

        /// <summary>
        /// The <see cref="QueryAsync"/> API returns the
        /// result of a query against the blob.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="inputTextConfiguration">
        /// Optional input text configuration.
        /// </param>
        /// <param name="outputTextConfiguration">
        /// Optional output text configuration.
        /// </param>
        /// <param name="errorReceiver">
        /// Optional error receiver.
        /// </param>
        /// <param name="conditions">
        /// Optional request conditions.
        /// </param>
        /// <param name="progressReceiver">
        /// Optional progress receiver.
        /// </param>
        /// <param name="cancellationToken">
        /// Optional <see cref="CancellationToken"/> to propagate
        /// notifications that the operation should be cancelled.
        /// </param>
        /// <remarks>
        /// A <see cref="RequestFailedException"/> will be thrown if
        /// a failure occurs.
        /// </remarks>
        /// <returns>
        /// A <see cref="Response{BlobQueryInfo}"/>.
        /// </returns>
        public virtual async Task<Response<BlobDownloadInfo>> QueryAsync(
            string query,
            BlobQueryTextConfiguration inputTextConfiguration = default,
            BlobQueryTextConfiguration outputTextConfiguration = default,
            IBlobQueryErrorReceiver errorReceiver = default,
            BlobRequestConditions conditions = default,
            IProgress<long> progressReceiver = default,
            CancellationToken cancellationToken = default) =>
            await QueryInternal(
                query,
                inputTextConfiguration,
                outputTextConfiguration,
                errorReceiver,
                conditions,
                progressReceiver,
                async: true,
                cancellationToken)
            .ConfigureAwait(false);

        /// <summary>
        /// The <see cref="QueryInternal"/> API returns the
        /// result of a query against the blob.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="inputTextConfiguration">
        /// Optional input text configuration.
        /// </param>
        /// <param name="outputTextConfiguration">
        /// Optional output text configuration.
        /// </param>
        /// <param name="nonFatalErrorReceiver">
        /// Optional non-fatal error receiver.
        /// </param>
        /// <param name="conditions">
        /// Optional request conditions.
        /// </param>
        /// <param name="progressReceiver">
        /// Optional progress receiver.
        /// </param>
        /// <param name="async">
        /// Whether to invoke the operation asynchronously.
        /// </param>
        /// <param name="cancellationToken">
        /// Optional <see cref="CancellationToken"/> to propagate
        /// notifications that the operation should be cancelled.
        /// </param>
        /// <remarks>
        /// A <see cref="RequestFailedException"/> will be thrown if
        /// a failure occurs.
        /// </remarks>
        /// <returns>
        /// A <see cref="Response{BlobQueryInfo}"/>.
        /// </returns>
        private async Task<Response<BlobDownloadInfo>> QueryInternal(
            string query,
            BlobQueryTextConfiguration inputTextConfiguration,
            BlobQueryTextConfiguration outputTextConfiguration,
            IBlobQueryErrorReceiver nonFatalErrorReceiver,
            BlobRequestConditions conditions,
            IProgress<long> progressReceiver,
            bool async,
            CancellationToken cancellationToken)
        {
            using (Pipeline.BeginLoggingScope(nameof(BlobQuickQueryClient)))
            {
                Pipeline.LogMethodEnter(nameof(BlobQuickQueryClient), message: $"{nameof(Uri)}: {Uri}");

                try
                {
                    QueryRequest queryRequest = new QueryRequest()
                    {
                        QueryType = Constants.QuickQuery.SqlQueryType,
                        Expression = query,
                        InputSerialization = inputTextConfiguration.ToQuickQuerySerialization(),
                        OutputSerialization = outputTextConfiguration.ToQuickQuerySerialization()
                    };
                    Response<BlobQuickQueryResult> result = await BlobQuickQueryRestClient.Blob.QuickQueryAsync(
                        clientDiagnostics: ClientDiagnostics,
                        pipeline: Pipeline,
                        resourceUri: Uri,
                        version: Version.ToVersionString(),
                        queryRequest: queryRequest,
                        leaseId: conditions?.LeaseId,
                        encryptionKey: CustomerProvidedKey?.EncryptionKey,
                        encryptionKeySha256: CustomerProvidedKey?.EncryptionKeyHash,
                        encryptionAlgorithm: CustomerProvidedKey?.EncryptionAlgorithm.ToQuickQueryEncryptionAlgorithmType(),
                        ifModifiedSince: conditions?.IfModifiedSince,
                        ifUnmodifiedSince: conditions?.IfUnmodifiedSince,
                        ifMatch: conditions?.IfMatch,
                        ifNoneMatch: conditions?.IfNoneMatch,
                        async: async,
                        operationName: $"{nameof(BlobQuickQueryClient)}.{nameof(Query)}",
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    Stream parsedStream = new BlobQuickQueryStream(result.Value.Body, progressReceiver, nonFatalErrorReceiver);
                    result.Value.Body = parsedStream;


                    return Response.FromValue(result.Value.ToBlobDownloadInfo(), result.GetRawResponse());
                }
                catch (Exception ex)
                {
                    Pipeline.LogException(ex);
                    throw;
                }
                finally
                {
                    Pipeline.LogMethodExit(nameof(BlobQuickQueryClient));
                }
            }
        }
    }
}
