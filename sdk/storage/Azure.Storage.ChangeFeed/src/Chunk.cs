// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avro.File;
using Avro.Generic;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.ChangeFeed.Models;

namespace Azure.Storage.ChangeFeed
{
    /// <summary>
    /// Chunk.
    /// </summary>
    internal class Chunk
    {
        /// <summary>
        /// Blob Client for downloading the Chunk.
        /// </summary>
        private readonly BlobClient _blobClient;

        /// <summary>
        /// The index of the event we are currently processing.
        /// </summary>
        //TODO this might not work if we don't download the entire chunk at a time.
        public long EventIndex { get; private set; }

        /// <summary>
        /// Avro Reader to parser the Events.
        /// </summary>
        private IFileReader<GenericRecord> _avroReader;

        /// <summary>
        /// If this Chunk has been initalized.
        /// </summary>
        private bool _isInitialized;

        public Chunk(
            BlobContainerClient containerClient,
            string chunkPath,
            long? eventIndex = default)
        {
            _blobClient = containerClient.GetBlobClient(chunkPath);
            _isInitialized = false;
            EventIndex = eventIndex ?? 0;
        }

        //TODO need to figure out how to not download the entire chunk
        //TODO figure out live streaming
        private async Task Initalize(
            bool async)
        {
            BlobDownloadInfo blobDownloadInfo;
            if (async)
            {
                blobDownloadInfo = await _blobClient.DownloadAsync().ConfigureAwait(false);
            }
            else
            {
                blobDownloadInfo = _blobClient.Download();
            }
            _isInitialized = true;
            _avroReader = DataFileReader<GenericRecord>.OpenReader(blobDownloadInfo.Content);

            // Fast forward to next event.
            //TODO this won't work if we decide to only download part of the Chunck.
            if (EventIndex > 0)
            {
                for (int i = 0; i < EventIndex; i++)
                {
                    //TODO add async version of this.
                    _avroReader.Next();
                }
            }
        }

        //TODO what if the Segment isn't Finalized??
        public async Task<bool> HasNext(bool async)
        {
            if (!_isInitialized)
            {
                return true;
            }

            if (async)
            {
                //TODO someday this will be real async
                return await Task.FromResult(_avroReader.HasNext()).ConfigureAwait(false);
            }
            else
            {
                return _avroReader.HasNext();
            }
        }

        public async Task<BlobChangeFeedEvent> Next(bool async)
        {
            if (!_isInitialized)
            {
                if (async)
                {
                    await Initalize(async: true).ConfigureAwait(false);
                }
                else
                {
                    Initalize(async: false).EnsureCompleted();
                }
            }

            GenericRecord genericRecord;

            bool hasNext;
            if (async)
            {
                hasNext = await HasNext(async: true).ConfigureAwait(false);
            }
            else
            {
                hasNext = HasNext(async: false).EnsureCompleted();
            }

            if (!hasNext)
            {
                return null;
            }


            if (async)
            {
                //TODO someday this will be real async
                genericRecord = await Task.FromResult(_avroReader.Next()).ConfigureAwait(false);
            }
            else
            {
                genericRecord = _avroReader.Next();
            }

            EventIndex++;
            return new BlobChangeFeedEvent(genericRecord);
        }
    }
}
