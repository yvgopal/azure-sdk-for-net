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
using Microsoft.Azure.Management.RemoteApp.Models;

namespace Microsoft.Azure.Management.RemoteApp.Models
{
    /// <summary>
    /// Operation response for TemplateImageOperations.List.
    /// </summary>
    public partial class TemplateImageListResult : AzureOperationResponse
    {
        private IList<TemplateImage> _remoteAppTemplateImageList;
        
        /// <summary>
        /// Optional. List of template images.
        /// </summary>
        public IList<TemplateImage> RemoteAppTemplateImageList
        {
            get { return this._remoteAppTemplateImageList; }
            set { this._remoteAppTemplateImageList = value; }
        }
        
        /// <summary>
        /// Initializes a new instance of the TemplateImageListResult class.
        /// </summary>
        public TemplateImageListResult()
        {
            this.RemoteAppTemplateImageList = new LazyList<TemplateImage>();
        }
    }
}
