// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace Azure.ResourceManager.Compute.Models
{
    public partial class ResourceSku
    {
        internal static ResourceSku DeserializeResourceSku(JsonElement element)
        {
            Optional<string> resourceType = default;
            Optional<string> name = default;
            Optional<string> tier = default;
            Optional<string> size = default;
            Optional<string> family = default;
            Optional<string> kind = default;
            Optional<ResourceSkuCapacity> capacity = default;
            Optional<IReadOnlyList<string>> locations = default;
            Optional<IReadOnlyList<ResourceSkuLocationInfo>> locationInfo = default;
            Optional<IReadOnlyList<string>> apiVersions = default;
            Optional<IReadOnlyList<ResourceSkuCosts>> costs = default;
            Optional<IReadOnlyList<ResourceSkuCapabilities>> capabilities = default;
            Optional<IReadOnlyList<ResourceSkuRestrictions>> restrictions = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("resourceType"))
                {
                    resourceType = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("name"))
                {
                    name = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("tier"))
                {
                    tier = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("size"))
                {
                    size = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("family"))
                {
                    family = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("kind"))
                {
                    kind = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("capacity"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    capacity = ResourceSkuCapacity.DeserializeResourceSkuCapacity(property.Value);
                    continue;
                }
                if (property.NameEquals("locations"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<string> array = new List<string>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(item.GetString());
                    }
                    locations = array;
                    continue;
                }
                if (property.NameEquals("locationInfo"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<ResourceSkuLocationInfo> array = new List<ResourceSkuLocationInfo>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(ResourceSkuLocationInfo.DeserializeResourceSkuLocationInfo(item));
                    }
                    locationInfo = array;
                    continue;
                }
                if (property.NameEquals("apiVersions"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<string> array = new List<string>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(item.GetString());
                    }
                    apiVersions = array;
                    continue;
                }
                if (property.NameEquals("costs"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<ResourceSkuCosts> array = new List<ResourceSkuCosts>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(ResourceSkuCosts.DeserializeResourceSkuCosts(item));
                    }
                    costs = array;
                    continue;
                }
                if (property.NameEquals("capabilities"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<ResourceSkuCapabilities> array = new List<ResourceSkuCapabilities>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(ResourceSkuCapabilities.DeserializeResourceSkuCapabilities(item));
                    }
                    capabilities = array;
                    continue;
                }
                if (property.NameEquals("restrictions"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<ResourceSkuRestrictions> array = new List<ResourceSkuRestrictions>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(ResourceSkuRestrictions.DeserializeResourceSkuRestrictions(item));
                    }
                    restrictions = array;
                    continue;
                }
            }
            return new ResourceSku(resourceType.Value, name.Value, tier.Value, size.Value, family.Value, kind.Value, capacity.Value, Optional.ToList(locations), Optional.ToList(locationInfo), Optional.ToList(apiVersions), Optional.ToList(costs), Optional.ToList(capabilities), Optional.ToList(restrictions));
        }
    }
}
