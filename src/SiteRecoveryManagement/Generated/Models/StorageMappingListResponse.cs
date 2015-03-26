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
using System.Collections.Generic;
using System.Linq;
using Hyak.Common;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Management.SiteRecovery.Models;

namespace Microsoft.WindowsAzure.Management.SiteRecovery.Models
{
    /// <summary>
    /// The response model for the list of storage mappings operation.
    /// </summary>
    public partial class StorageMappingListResponse : AzureOperationResponse, IEnumerable<StorageMapping>
    {
        private IList<StorageMapping> _storageMappings;
        
        /// <summary>
        /// Optional. The list of storage mappings for the given vault.
        /// </summary>
        public IList<StorageMapping> StorageMappings
        {
            get { return this._storageMappings; }
            set { this._storageMappings = value; }
        }
        
        /// <summary>
        /// Initializes a new instance of the StorageMappingListResponse class.
        /// </summary>
        public StorageMappingListResponse()
        {
            this.StorageMappings = new LazyList<StorageMapping>();
        }
        
        /// <summary>
        /// Gets the sequence of StorageMappings.
        /// </summary>
        public IEnumerator<StorageMapping> GetEnumerator()
        {
            return this.StorageMappings.GetEnumerator();
        }
        
        /// <summary>
        /// Gets the sequence of StorageMappings.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
