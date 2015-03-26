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
using Microsoft.Azure.Search.Models;

namespace Microsoft.Azure.Search.Models
{
    /// <summary>
    /// Represents an index definition in Azure Search, which describes the
    /// fields and search behavior of an index.
    /// </summary>
    public partial class Index
    {
        private CorsOptions _corsOptions;
        
        /// <summary>
        /// Optional. Gets or sets options to control Cross-Origin Resource
        /// Sharing (CORS) for the index.
        /// </summary>
        public CorsOptions CorsOptions
        {
            get { return this._corsOptions; }
            set { this._corsOptions = value; }
        }
        
        private string _defaultScoringProfile;
        
        /// <summary>
        /// Optional. Gets or sets the name of the scoring profile to use if
        /// none is specified in the query. If this property is not set and no
        /// scoring profile is specified in the query, then default scoring
        /// (tf-idf) will be used.
        /// </summary>
        public string DefaultScoringProfile
        {
            get { return this._defaultScoringProfile; }
            set { this._defaultScoringProfile = value; }
        }
        
        private IList<Field> _fields;
        
        /// <summary>
        /// Optional. Gets or sets the fields of the index.
        /// </summary>
        public IList<Field> Fields
        {
            get { return this._fields; }
            set { this._fields = value; }
        }
        
        private string _name;
        
        /// <summary>
        /// Optional. Gets or sets the name of the index.  (see
        /// https://msdn.microsoft.com/library/azure/dn857353.aspx for more
        /// information)
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
        
        private IList<ScoringProfile> _scoringProfiles;
        
        /// <summary>
        /// Optional. Gets or sets the scoring profiles for the index.
        /// </summary>
        public IList<ScoringProfile> ScoringProfiles
        {
            get { return this._scoringProfiles; }
            set { this._scoringProfiles = value; }
        }
        
        private IList<Suggester> _suggesters;
        
        /// <summary>
        /// Optional. Gets or sets the suggesters for the index.
        /// </summary>
        public IList<Suggester> Suggesters
        {
            get { return this._suggesters; }
            set { this._suggesters = value; }
        }
        
        /// <summary>
        /// Initializes a new instance of the Index class.
        /// </summary>
        public Index()
        {
            this.Fields = new LazyList<Field>();
            this.ScoringProfiles = new LazyList<ScoringProfile>();
            this.Suggesters = new LazyList<Suggester>();
        }
    }
}
