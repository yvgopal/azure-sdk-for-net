// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.ChangeFeed.Models;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Internal.Avro;

namespace Azure.Storage.Blobs.ChangeFeed
{
    /// <summary>
    /// Chunk.
    /// </summary>
    internal class Chunk : IDisposable
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
        //private IFileReader<GenericRecord> _avroReader;
        private AvroReader _avroReader;

        /// <summary>
        /// If this Chunk has been initalized.
        /// </summary>
        private bool _isInitialized;

        private MemoryStream _stream;

        public Chunk(
            BlobContainerClient containerClient,
            string chunkPath,
            long? eventIndex = default)
        {
            _blobClient = containerClient.GetBlobClient(chunkPath);
            _isInitialized = false;
            EventIndex = eventIndex ?? 0;
            _stream = new MemoryStream();
        }

        //TODO need to figure out how to not download the entire chunk
        //TODO figure out live streaming
        private async Task Initalize(
            bool async)
        {
            if (async)
            {
                 await _blobClient.DownloadToAsync(_stream).ConfigureAwait(false);
            }
            else
            {
                _blobClient.DownloadTo(_stream);
            }

            _stream.Position = 0;
            _isInitialized = true;
            _avroReader = new AvroReader(_stream);

            // Fast forward to next event.
            //TODO this won't work if we decide to only download part of the Chunck.
            if (EventIndex > 0)
            {
                for (int i = 0; i < EventIndex; i++)
                {
                    if (async)
                    {
                        await _avroReader.Next(async: true).ConfigureAwait(false);
                    }
                    else
                    {
                        _avroReader.Next(async: false).EnsureCompleted();
                    }
                }
            }
        }

        //TODO what if the Segment isn't Finalized??
        public bool HasNext()
        {
            if (!_isInitialized)
            {
                return true;
            }

            return _avroReader.HasNext();
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

            object result;

            if (!HasNext())
            {
                return null;
            }

            if (async)
            {
                result = await _avroReader.Next(async: true).ConfigureAwait(false);
            }
            else
            {
                result = _avroReader.Next(async: false).EnsureCompleted();
            }

            EventIndex++;
            return new BlobChangeFeedEvent((Dictionary<string, object>)result);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
