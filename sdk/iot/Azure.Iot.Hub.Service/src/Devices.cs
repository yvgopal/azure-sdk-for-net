// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Iot.Hub.Service.Models;

namespace Azure.Iot.Hub.Service
{
    public class Devices
    {
        private RegistryManagerRestClient registryManagerClient;
        private TwinRestClient twinClient;
        private DeviceMethodRestClient deviceMethodClient;

        private const string ContinuationTokenHeader = "x-ms-continuation";

        /// <summary>
        /// Create a device.
        /// </summary>
        /// <param name="deviceIdentity">The device to create.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created device.</returns>
        public virtual async Task<Response<DeviceIdentity>> CreateIdentityAsync(DeviceIdentity deviceIdentity, CancellationToken cancellationToken = default)
        {
            return await registryManagerClient.CreateOrUpdateDeviceAsync(deviceIdentity.DeviceId, deviceIdentity, null, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Update a device.
        /// </summary>
        /// <param name="deviceIdentity">The device to update.</param>
        /// <param name="ifMatch">A string representing a weak ETag for this device, as per RFC7232. The update operation is performed
        /// only if this ETag matches the value maintained by the server, indicating that the device has not been modified since it was last retrieved.
        /// The current ETag can be retrieved from the device identity last retrieved from the service. To force an unconditional update, set If-Match to the wildcard character (*).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created or updated device.</returns>
        public virtual async Task<Response<DeviceIdentity>> UpdateIdentityAsync(DeviceIdentity deviceIdentity, string ifMatch = null, CancellationToken cancellationToken = default)
        {
            return await registryManagerClient.CreateOrUpdateDeviceAsync(deviceIdentity.DeviceId, deviceIdentity, ifMatch, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a single device.
        /// </summary>
        /// <param name="deviceId">The unique identifier of the device to get.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The retrieved device.</returns>
        public virtual async Task<Response<DeviceIdentity>> GetIdentityAsync(string deviceId, CancellationToken cancellationToken = default)
        {
            return await registryManagerClient.GetDeviceAsync(deviceId, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a single device.
        /// </summary>
        /// <param name="deviceId">The unique identifier of the device to delete.</param>
        /// <param name="ifMatch">A string representing a weak ETag for this device, as per RFC7232. The delete operation is performed
        /// only if this ETag matches the value maintained by the server, indicating that the device has not been modified since it was last retrieved.
	    /// The current ETag can be retrieved from the device identity last retrieved from the service. To force an unconditional delete, set If-Match to the wildcard character (*).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The http response.</returns>
        public virtual async Task<Response> DeleteIdentityAsync(string deviceId, string ifMatch = null, CancellationToken cancellationToken = default)
        {
            return await registryManagerClient.DeleteDeviceAsync(deviceId, ifMatch, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Create multiple devices with an initial twin. A maximum of 100 creations can be done per call, and each creation must have a unique device identity. For larger scale operations, consider using IoT Hub jobs (https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-identity-registry#import-and-export-device-identities).
        /// </summary>
        /// <param name="devices">The pairs of devices their twins that will be created. For fields such as deviceId
        /// where device and twin have a definition, the device value will override the twin value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of the bulk operation.</returns>
        public async Task<Response<BulkRegistryOperationResult>> CreateIdentitiesWithTwinAsync(IDictionary<DeviceIdentity, TwinData> devices, CancellationToken cancellationToken = default)
        {
            IEnumerable<ExportImportDevice> registryOperations = devices
                .ToList()
                .Select(x => new ExportImportDevice()
                {
                    Id = x.Key.DeviceId,
                    Authentication = x.Key.Authentication,
                    Capabilities = x.Key.Capabilities,
                    DeviceScope = x.Key.DeviceScope,
                    ParentScopes = x.Key.ParentScopes,
                    Status = String.Equals(ExportImportDeviceStatus.Disabled.ToString(), x.Key.Status?.ToString(), StringComparison.OrdinalIgnoreCase) ? ExportImportDeviceStatus.Disabled : ExportImportDeviceStatus.Enabled,
                    StatusReason = x.Key.StatusReason,
                    Tags = x.Value.Tags,
                    Properties = new PropertyContainer(x.Value.Properties?.Desired, x.Value.Properties?.Reported),
                    ImportMode = ExportImportDeviceImportMode.Create
                });

            return await registryManagerClient.BulkDeviceCrudAsync(registryOperations, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Create multiple devices. A maximum of 100 creations can be done per call, and each device identity must be unique. For larger scale operations, consider using IoT Hub jobs (https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-identity-registry#import-and-export-device-identities).
        /// </summary>
        /// <param name="deviceIdentities">The devices to create.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of the bulk operation.</returns>
        public virtual async Task<Response<BulkRegistryOperationResult>> CreateIdentitiesAsync(IEnumerable<DeviceIdentity> deviceIdentities, CancellationToken cancellationToken = default)
        {
            IEnumerable<ExportImportDevice> registryOperations = deviceIdentities
                .ToList()
                .Select(x => new ExportImportDevice()
                {
                    Id = x.DeviceId,
                    Authentication = x.Authentication,
                    Capabilities = x.Capabilities,
                    DeviceScope = x.DeviceScope,
                    ParentScopes = x.ParentScopes,
                    Status = String.Equals(ExportImportDeviceStatus.Disabled.ToString(), x.Status?.ToString(), StringComparison.OrdinalIgnoreCase) ? ExportImportDeviceStatus.Disabled : ExportImportDeviceStatus.Enabled,
                    StatusReason = x.StatusReason,
                    ImportMode = ExportImportDeviceImportMode.Create
                });

            return await registryManagerClient.BulkDeviceCrudAsync(registryOperations, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Update multiple devices. A maximum of 100 updates can be done per call, and each operation must be done on a different identity. For larger scale operations, consider using IoT Hub jobs (https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-identity-registry#import-and-export-device-identities).
        /// </summary>
        /// <param name="deviceIdentities">The devices to update.</param>
        /// <param name="force">If true, the devices will be updated even if their ETag is out of date.
        /// If false, each device will only be updated if its ETag is up to date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of the bulk operation.</returns>
        public virtual async Task<Response<BulkRegistryOperationResult>> UpdateIdentiesAsync(IEnumerable<DeviceIdentity> deviceIdentities, bool force, CancellationToken cancellationToken = default)
        {
            IEnumerable<ExportImportDevice> registryOperations = deviceIdentities
                .ToList()
                .Select(x => new ExportImportDevice()
                {
                    Id = x.DeviceId,
                    Authentication = x.Authentication,
                    Capabilities = x.Capabilities,
                    DeviceScope = x.DeviceScope,
                    ParentScopes = x.ParentScopes,
                    ETag = x.Etag,
                    Status = String.Equals(ExportImportDeviceStatus.Disabled.ToString(), x.Status?.ToString(), StringComparison.OrdinalIgnoreCase) ? ExportImportDeviceStatus.Disabled : ExportImportDeviceStatus.Enabled,
                    StatusReason = x.StatusReason,
                    ImportMode = force ? ExportImportDeviceImportMode.Update : ExportImportDeviceImportMode.UpdateIfMatchETag
                });

            return await registryManagerClient.BulkDeviceCrudAsync(registryOperations, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple devices. A maximum of 100 deletions can be done per call. For larger scale operations, consider using IoT Hub jobs (https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-identity-registry#import-and-export-device-identities).
        /// </summary>
        /// <param name="deviceIdentities">The devices to delete.</param>
        /// <param name="force">If true, the devices will be deleted even if their ETag is out of date.
        /// If false, each device will only be deleted if its ETag is up to date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of the bulk deletion.</returns>
        public virtual async Task<Response<BulkRegistryOperationResult>> DeleteIdentitiesAsync(IEnumerable<DeviceIdentity> deviceIdentities, bool force, CancellationToken cancellationToken = default)
        {
            IEnumerable<ExportImportDevice> registryOperations = deviceIdentities
                .ToList()
                .Select(x => new ExportImportDevice()
                {
                    Id = x.DeviceId,
                    ETag = x.Etag,
                    ImportMode = force ? ExportImportDeviceImportMode.Delete : ExportImportDeviceImportMode.DeleteIfMatchETag
                });

            return await registryManagerClient.BulkDeviceCrudAsync(registryOperations, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// List a set of device twins.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A pageable set of device twins.</returns>
        public virtual AsyncPageable<TwinData> GetTwinsAsync(CancellationToken cancellationToken = default)
        {
            async Task<Page<TwinData>> FirstPageFunc(int? pageSizeHint)
            {
                var querySpecification = new QuerySpecification
                {
                    Query = "select * from devices"
                };
                Response<IReadOnlyList<TwinData>> response = await registryManagerClient.QueryIotHubAsync(querySpecification, null, pageSizeHint?.ToString(), cancellationToken).ConfigureAwait(false);
                string continuationToken;
                response.GetRawResponse().Headers.TryGetValue(ContinuationTokenHeader, out continuationToken);

                return Page.FromValues(response.Value, continuationToken, response.GetRawResponse());
            }

            async Task<Page<TwinData>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var querySpecification = new QuerySpecification();
                Response<IReadOnlyList<TwinData>> response = await registryManagerClient.QueryIotHubAsync(querySpecification, nextLink, pageSizeHint?.ToString(), cancellationToken).ConfigureAwait(false);
                string continuationToken;
                response.GetRawResponse().Headers.TryGetValue(ContinuationTokenHeader, out continuationToken);
                return Page.FromValues(response.Value, continuationToken, response.GetRawResponse());
            }

            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary>
        /// Get a device's twin.
        /// </summary>
        /// <param name="deviceId">The unique identifier of the device to get the twin of.</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The device's twin, including reported properties and desired properties.</returns>
        public virtual async Task<Response<TwinData>> GetTwinAsync(string deviceId, CancellationToken cancellationToken = default)
        {
            return await twinClient.GetDeviceTwinAsync(deviceId, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Update a device's twin.
        /// </summary>
        /// <param name="twinPatch">The properties to update. Any existing properties not referenced by this patch will be unaffected by this patch.</param>
        /// <param name="ifMatch">A string representing a weak ETag for this twin, as per RFC7232. The update operation is performed
        /// only if this ETag matches the value maintained by the server, indicating that the twin has not been modified since it was last retrieved.
        /// To force an unconditional update, set If-Match to the wildcard character (*).</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The server's new representation of the device twin.</returns>
        public virtual async Task<Response<TwinData>> UpdateTwinAsync(TwinData twinPatch, string ifMatch = null, CancellationToken cancellationToken = default)
        {
            Argument.AssertNotNull(twinPatch, nameof(twinPatch));

            return await twinClient.UpdateDeviceTwinAsync(twinPatch.DeviceId, twinPatch, ifMatch, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Update multiple devices' twins. A maximum of 100 updates can be done per call, and each operation must be done on a different device twin. For larger scale operations, consider using IoT Hub jobs (https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-identity-registry#import-and-export-device-identities).
        /// </summary>
        /// <param name="twinUpdates">The new twins to replace the twins on existing devices</param>
        /// <param name="force">If true, all the update operations will ignore the provided twin ETags and will
        /// force the update. If false, each update operation will fail if the provided ETag for the update is out of date.</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result of the bulk operation.</returns>
        public virtual async Task<Response<BulkRegistryOperationResult>> UpdateTwinsAsync(IEnumerable<TwinData> twinUpdates, bool force, CancellationToken cancellationToken = default)
        {
            IEnumerable<ExportImportDevice> registryOperations = twinUpdates
                .ToList()
                .Select(x => new ExportImportDevice()
                {
                    Id = x.DeviceId,
                    Tags = x.Tags,
                    Properties = new PropertyContainer(x.Properties?.Desired, x.Properties?.Reported),
                    TwinETag = x.Etag,
                    ImportMode = force ? ExportImportDeviceImportMode.UpdateTwin : ExportImportDeviceImportMode.UpdateTwinIfMatchETag
                });

            return await registryManagerClient.BulkDeviceCrudAsync(registryOperations, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Invoke a method on a device.
        /// </summary>
        /// <param name="deviceId">The unique identifier of the device to invoke the method on.</param>
        /// <param name="directMethodRequest">The details of the method to invoke.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of the method invocation.</returns>
        public virtual async Task<Response<CloudToDeviceMethodResult>> InvokeMethodAsync(string deviceId, CloudToDeviceMethod directMethodRequest, CancellationToken cancellationToken = default)
        {
            return await deviceMethodClient.InvokeDeviceMethodAsync(deviceId, directMethodRequest, cancellationToken).ConfigureAwait(false);
        }
    }
}
