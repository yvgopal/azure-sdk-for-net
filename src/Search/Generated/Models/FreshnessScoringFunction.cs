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
using Microsoft.Azure.Search.Models;

namespace Microsoft.Azure.Search.Models
{
    /// <summary>
    /// Defines a function that boosts scores based on the value of a date-time
    /// field.  (see https://msdn.microsoft.com/library/azure/dn798928.aspx
    /// for more information)
    /// </summary>
    public partial class FreshnessScoringFunction : ScoringFunction
    {
        private FreshnessScoringParameters _parameters;
        
        /// <summary>
        /// Required. Gets parameter values for the freshness scoring function.
        /// </summary>
        public FreshnessScoringParameters Parameters
        {
            get { return this._parameters; }
            set { this._parameters = value; }
        }
        
        /// <summary>
        /// Initializes a new instance of the FreshnessScoringFunction class.
        /// </summary>
        public FreshnessScoringFunction()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the FreshnessScoringFunction class
        /// with required arguments.
        /// </summary>
        public FreshnessScoringFunction(FreshnessScoringParameters parameters, string fieldName, double boost)
            : this()
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }
            this.Parameters = parameters;
            this.FieldName = fieldName;
            this.Boost = boost;
        }
    }
}
