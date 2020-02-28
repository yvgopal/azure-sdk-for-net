// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Azure.Core.Pipeline;

namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// Convert StorageErrors into StorageRequestFailedExceptions.
    /// </summary>
    internal partial class StorageError
    {
        /// <summary>
        /// Additional error information.
        /// </summary>
        public IDictionary<string, string> AdditionalInformation { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Create an exception corresponding to the StorageError.
        /// </summary>
        /// <param name="clientDiagnostics">The <see cref="ClientDiagnostics"/> instance to use.</param>
        /// <param name="response">The failed response.</param>
        /// <returns>A RequestFailedException.</returns>
        public Exception CreateException(ClientDiagnostics clientDiagnostics, Azure.Response response)
            => clientDiagnostics.CreateRequestFailedExceptionWithContent(response, message: Message, content: null, errorCode: response.GetErrorCode(Code), additionalInfo: AdditionalInformation);
    }
}
