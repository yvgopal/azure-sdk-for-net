// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using Azure.Core;

namespace Azure.Iot.Hub.Service.Models
{
    public partial class CloudToDeviceMethod : IUtf8JsonSerializable
    {
        void IUtf8JsonSerializable.Write(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            if (MethodName != null)
            {
                writer.WritePropertyName("methodName");
                writer.WriteStringValue(MethodName);
            }
            if (Payload != null)
            {
                writer.WritePropertyName("payload");
                writer.WriteObjectValue(JsonDocument.Parse(Payload).RootElement);
            }
            if (ResponseTimeoutInSeconds != null)
            {
                writer.WritePropertyName("responseTimeoutInSeconds");
                writer.WriteNumberValue(ResponseTimeoutInSeconds.Value);
            }
            if (ConnectTimeoutInSeconds != null)
            {
                writer.WritePropertyName("connectTimeoutInSeconds");
                writer.WriteNumberValue(ConnectTimeoutInSeconds.Value);
            }
            writer.WriteEndObject();
        }

        internal static CloudToDeviceMethod DeserializeCloudToDeviceMethod(JsonElement element)
        {
            string methodName = default;
            string payload = default;
            int? responseTimeoutInSeconds = default;
            int? connectTimeoutInSeconds = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("methodName"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    methodName = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("payload"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    payload = property.Value.GetRawText();
                    continue;
                }
                if (property.NameEquals("responseTimeoutInSeconds"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    responseTimeoutInSeconds = property.Value.GetInt32();
                    continue;
                }
                if (property.NameEquals("connectTimeoutInSeconds"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    connectTimeoutInSeconds = property.Value.GetInt32();
                    continue;
                }
            }
            return new CloudToDeviceMethod(methodName, payload, responseTimeoutInSeconds, connectTimeoutInSeconds);
        }
    }
}
