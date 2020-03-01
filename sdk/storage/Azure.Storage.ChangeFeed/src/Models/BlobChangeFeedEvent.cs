// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Avro.Generic;

namespace Azure.Storage.ChangeFeed.Models
{
    /// <summary>
    /// BlobChangeFeedEvent.
    /// </summary>
    public class BlobChangeFeedEvent
    {
        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal BlobChangeFeedEvent(GenericRecord record)
        {
            Topic = (string)record["topic"];
            Subject = (string)record["subject"];
            EventType = ToBlobChangeFeedEventType((GenericEnum)record["eventType"]);
            EventTime = DateTimeOffset.Parse((string)record["eventTime"], CultureInfo.InvariantCulture);
            Id = Guid.Parse((string)record["id"]);
            EventData = new BlobChangeFeedEventData((GenericRecord)record["data"]);
            record.TryGetValue("dataVersion", out object dataVersion);
            DataVersion = (long?)dataVersion;
            record.TryGetValue("metadataVersion", out object metadataVersion);
            MetadataVersion = (string)metadataVersion;
        }

        internal BlobChangeFeedEvent() { }

        /// <summary>
        /// Full resource path to the event source. This field is not writeable. Event Grid provides this value.
        /// </summary>
        public string Topic { get; internal set; }

        /// <summary>
        /// Publisher-defined path to the event subject.
        /// </summary>
        public string Subject { get; internal set; }

        /// <summary>
        /// One of the registered event types for this event source.
        /// </summary>
        public BlobChangeFeedEventType EventType { get; internal set; }

        /// <summary>
        /// The time the event is generated based on the provider's UTC time.
        /// </summary>
        public DateTimeOffset EventTime { get; internal set; }

        /// <summary>
        /// Unique identifier for the event.
        /// </summary>
        public Guid Id { get; internal set; }

        /// <summary>
        /// Blob storage event data.
        /// </summary>
        public BlobChangeFeedEventData EventData { get; internal set; }

        /// <summary>
        /// The schema version of the data object. The publisher defines the schema version.
        /// </summary>
        public long? DataVersion { get; internal set; }

        /// <summary>
        /// The schema version of the event metadata. Event Grid defines the schema of the top-level properties.
        /// Event Grid provides this value.
        /// </summary>
        public string MetadataVersion { get; internal set; }

        private static BlobChangeFeedEventType ToBlobChangeFeedEventType(GenericEnum s)
        {
            switch (s.Value)
            {
                case "BlobCreated":
                    return BlobChangeFeedEventType.BlobCreated;
                case "BlobDeleted":
                    return BlobChangeFeedEventType.BlobDeleted;
                default:
                    return default;
            }
        }
    }
}
