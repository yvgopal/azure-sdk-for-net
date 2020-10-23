// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Management.StreamAnalytics.Models
{
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// The parameters needed to retrieve the default function definition for
    /// an Azure Machine Learning Studio function.
    /// </summary>
    [Newtonsoft.Json.JsonObject("Microsoft.MachineLearning/WebService")]
    [Rest.Serialization.JsonTransformation]
    public partial class AzureMachineLearningStudioFunctionRetrieveDefaultDefinitionParameters : FunctionRetrieveDefaultDefinitionParameters
    {
        /// <summary>
        /// Initializes a new instance of the
        /// AzureMachineLearningStudioFunctionRetrieveDefaultDefinitionParameters
        /// class.
        /// </summary>
        public AzureMachineLearningStudioFunctionRetrieveDefaultDefinitionParameters()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// AzureMachineLearningStudioFunctionRetrieveDefaultDefinitionParameters
        /// class.
        /// </summary>
        /// <param name="executeEndpoint">The Request-Response execute endpoint
        /// of the Azure Machine Learning Studio. Find out more here:
        /// https://docs.microsoft.com/en-us/azure/machine-learning/machine-learning-consume-web-services#request-response-service-rrs</param>
        /// <param name="udfType">The function type. Possible values include:
        /// 'Scalar'</param>
        public AzureMachineLearningStudioFunctionRetrieveDefaultDefinitionParameters(string executeEndpoint = default(string), UdfType? udfType = default(UdfType?))
        {
            ExecuteEndpoint = executeEndpoint;
            UdfType = udfType;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the Request-Response execute endpoint of the Azure
        /// Machine Learning Studio. Find out more here:
        /// https://docs.microsoft.com/en-us/azure/machine-learning/machine-learning-consume-web-services#request-response-service-rrs
        /// </summary>
        [JsonProperty(PropertyName = "bindingRetrievalProperties.executeEndpoint")]
        public string ExecuteEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the function type. Possible values include: 'Scalar'
        /// </summary>
        [JsonProperty(PropertyName = "bindingRetrievalProperties.udfType")]
        public UdfType? UdfType { get; set; }

    }
}
