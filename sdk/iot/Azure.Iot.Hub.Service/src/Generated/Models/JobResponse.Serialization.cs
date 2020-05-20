// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Text.Json;
using Azure.Core;

namespace Azure.Iot.Hub.Service.Models
{
    public partial class JobResponse
    {
        internal static JobResponse DeserializeJobResponse(JsonElement element)
        {
            string jobId = default;
            string queryCondition = default;
            DateTimeOffset? createdTime = default;
            DateTimeOffset? startTime = default;
            DateTimeOffset? endTime = default;
            long? maxExecutionTimeInSeconds = default;
            JobResponseType? type = default;
            CloudToDeviceMethod cloudToDeviceMethod = default;
            TwinData updateTwin = default;
            JobResponseStatus? status = default;
            string failureReason = default;
            string statusMessage = default;
            DeviceJobStatistics deviceJobStatistics = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("jobId"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    jobId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("queryCondition"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    queryCondition = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("createdTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    createdTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("startTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    startTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("endTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    endTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("maxExecutionTimeInSeconds"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    maxExecutionTimeInSeconds = property.Value.GetInt64();
                    continue;
                }
                if (property.NameEquals("type"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    type = new JobResponseType(property.Value.GetString());
                    continue;
                }
                if (property.NameEquals("cloudToDeviceMethod"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    cloudToDeviceMethod = Models.CloudToDeviceMethod.DeserializeCloudToDeviceMethod(property.Value);
                    continue;
                }
                if (property.NameEquals("updateTwin"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    updateTwin = TwinData.DeserializeTwinData(property.Value);
                    continue;
                }
                if (property.NameEquals("status"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    status = new JobResponseStatus(property.Value.GetString());
                    continue;
                }
                if (property.NameEquals("failureReason"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    failureReason = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("statusMessage"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    statusMessage = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("deviceJobStatistics"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    deviceJobStatistics = DeviceJobStatistics.DeserializeDeviceJobStatistics(property.Value);
                    continue;
                }
            }
            return new JobResponse(jobId, queryCondition, createdTime, startTime, endTime, maxExecutionTimeInSeconds, type, cloudToDeviceMethod, updateTwin, status, failureReason, statusMessage, deviceJobStatistics);
        }
    }
}
