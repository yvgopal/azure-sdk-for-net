// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Azure.Core;
using Azure.Core.Testing;
using Azure.Storage.Blobs;
using Azure.Storage.Test.Shared;

namespace Azure.Storage.ChangeFeed.Tests
{
    public class ChangeFeedTestBase : StorageTestBase
    {

        public ChangeFeedTestBase(bool async) : this(async, null) { }

        public ChangeFeedTestBase(bool async, RecordedTestMode? mode = null)
            : base(async, RecordedTestMode.Live)
        {
        }

        public BlobServiceClient GetServiceClient_SharedKey()
            => InstrumentClient(
                new BlobServiceClient(
                    new Uri(TestConfigDefault.BlobServiceEndpoint),
                    new StorageSharedKeyCredential(
                        TestConfigDefault.AccountName,
                        TestConfigDefault.AccountKey),
                    GetOptions()));

        public BlobClientOptions GetOptions()
        {
            var options = new BlobClientOptions
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
                options.AddPolicy(new RecordedClientRequestIdPolicy(Recording), HttpPipelinePosition.PerCall);
            }

            return Recording.InstrumentClientOptions(options);
        }
    }
}
