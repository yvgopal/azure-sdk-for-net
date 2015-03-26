// 
// Copyright (c) Microsoft and contributors.  All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the License for the specific language governing permissions and
// limitations under the License.
// 

// Warning: This code was generated by a tool.
// 
// Changes to this file may cause incorrect behavior and will be lost if the
// code is regenerated.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Management.SiteRecovery.Models;

namespace Microsoft.WindowsAzure.Management.SiteRecovery
{
    /// <summary>
    /// Definition of virtual machine operations for the Site Recovery
    /// extension.
    /// </summary>
    public partial interface IVirtualMachineOperations
    {
        /// <summary>
        /// Get the VM object by Id.
        /// </summary>
        /// <param name='protectionContainerId'>
        /// Parent Protection Container ID.
        /// </param>
        /// <param name='virtualMachineId'>
        /// VM ID.
        /// </param>
        /// <param name='customRequestHeaders'>
        /// Request header parameters.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        /// <returns>
        /// The response model for the Vm object.
        /// </returns>
        Task<VirtualMachineResponse> GetAsync(string protectionContainerId, string virtualMachineId, CustomRequestHeaders customRequestHeaders, CancellationToken cancellationToken);
        
        /// <summary>
        /// Get the list of all Vms.
        /// </summary>
        /// <param name='protectionContainerId'>
        /// Parent Protection Container ID.
        /// </param>
        /// <param name='customRequestHeaders'>
        /// Request header parameters.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        /// <returns>
        /// The response model for the list Vm operation.
        /// </returns>
        Task<VirtualMachineListResponse> ListAsync(string protectionContainerId, CustomRequestHeaders customRequestHeaders, CancellationToken cancellationToken);
        
        /// <summary>
        /// Updates VM properties.
        /// </summary>
        /// <param name='protectionContainerId'>
        /// Parent Protection Container ID.
        /// </param>
        /// <param name='virtualMachineId'>
        /// VM ID.
        /// </param>
        /// <param name='parameters'>
        /// Update VM properties input.
        /// </param>
        /// <param name='customRequestHeaders'>
        /// Request header parameters.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        /// <returns>
        /// The response model for the Job details object.
        /// </returns>
        Task<JobResponse> UpdateVmPropertiesAsync(string protectionContainerId, string virtualMachineId, UpdateVmPropertiesInput parameters, CustomRequestHeaders customRequestHeaders, CancellationToken cancellationToken);
    }
}
