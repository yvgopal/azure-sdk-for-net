// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Testing;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Test;
using Azure.Storage.Test.Shared;

namespace Azure.Storage.QuickQuery.Tests
{
    public abstract class QuickQueryTestBase : StorageTestBase
    {
        public readonly string ReceivedETag = "\"received\"";
        public readonly string GarbageETag = "\"garbage\"";
        public readonly string ReceivedLeaseId = "received";


        public QuickQueryTestBase(bool async) : this(async, null) { }

        public QuickQueryTestBase(bool async, RecordedTestMode? mode = null)
            : base(async, mode)
        {
        }

        public string GetNewContainerName() => $"test-container-{Recording.Random.NewGuid()}";
        public string GetNewBlobName() => $"test-blob-{Recording.Random.NewGuid()}";
        public string GetGarbageLeaseId() => Recording.Random.NewGuid().ToString();

        public BlobClientOptions GetOptions(bool parallelRange = false)
        {
            var options = new BlobClientOptions()
            {
                Diagnostics = { IsLoggingEnabled = true },
                Retry =
                {
                    Mode = RetryMode.Exponential,
                    MaxRetries = Constants.MaxReliabilityRetries,
                    Delay = TimeSpan.FromSeconds(Mode == RecordedTestMode.Playback ? 0.01 : 0.5),
                    MaxDelay = TimeSpan.FromSeconds(Mode == RecordedTestMode.Playback ? 0.1 : 10)
                },
                Transport = GetTransport()
            };
            if (Mode != RecordedTestMode.Live)
            {
                options.AddPolicy(new RecordedClientRequestIdPolicy(Recording, parallelRange), HttpPipelinePosition.PerCall);
            }

            return Recording.InstrumentClientOptions(options);
        }

        public BlobServiceClient GetServiceClient_SharedKey(BlobClientOptions options = default)
            => GetServiceClientFromSharedKeyConfig(TestConfigDefault, options);

        public BlobServiceClient GetServiceClient_OauthAccount() =>
            GetServiceClientFromOauthConfig(TestConfigOAuth);

        private BlobServiceClient GetServiceClientFromSharedKeyConfig(
            TenantConfiguration config, BlobClientOptions options = default)
            => InstrumentClient(
                new BlobServiceClient(
                    new Uri(config.BlobServiceEndpoint),
                    new StorageSharedKeyCredential(config.AccountName, config.AccountKey),
                    options ?? GetOptions()));

        private BlobServiceClient GetServiceClientFromOauthConfig(TenantConfiguration config)
            => InstrumentClient(
                new BlobServiceClient(
                    new Uri(config.BlobServiceEndpoint),
                    GetOAuthCredential(config),
                    GetOptions()));

        public async Task<DisposingContainer> GetTestContainerAsync(
            BlobServiceClient service = default,
            string containerName = default,
            IDictionary<string, string> metadata = default,
            PublicAccessType? publicAccessType = default,
            bool premium = default)
        {

            containerName ??= GetNewContainerName();
            service ??= GetServiceClient_SharedKey();

            if (publicAccessType == default)
            {
                publicAccessType = premium ? PublicAccessType.None : PublicAccessType.BlobContainer;
            }

            BlobContainerClient container = InstrumentClient(service.GetBlobContainerClient(containerName));
            await container.CreateAsync(metadata: metadata, publicAccessType: publicAccessType.Value);
            return new DisposingContainer(container);
        }

        //TODO consider removing this.
        public async Task<string> SetupBlobMatchCondition(BlockBlobClient blob, string match)
        {
            if (match == ReceivedETag)
            {
                Response<BlobProperties> headers = await blob.GetPropertiesAsync();
                return headers.Value.ETag.ToString();
            }
            else
            {
                return match;
            }
        }

        //TODO consider removing this.
        public async Task<string> SetupBlobLeaseCondition(BlockBlobClient blob, string leaseId, string garbageLeaseId)
        {
            BlobLease lease = null;
            if (leaseId == ReceivedLeaseId || leaseId == garbageLeaseId)
            {
                lease = await InstrumentClient(blob.GetBlobLeaseClient(Recording.Random.NewGuid().ToString())).AcquireAsync(BlobLeaseClient.InfiniteLeaseDuration);
            }
            return leaseId == ReceivedLeaseId ? lease.LeaseId : leaseId;
        }

        public class DisposingContainer : IAsyncDisposable
        {
            public BlobContainerClient Container;

            public DisposingContainer(BlobContainerClient client)
            {
                Container = client;
            }

            public async ValueTask DisposeAsync()
            {
                if (Container != null)
                {
                    try
                    {
                        await Container.DeleteAsync();
                        Container = null;
                    }
                    catch
                    {
                        // swallow the exception to avoid hiding another test failure
                    }
                }
            }
        }
    }
}
