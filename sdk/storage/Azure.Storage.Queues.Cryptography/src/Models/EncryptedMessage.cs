// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Azure.Storage.Common.Cryptography.Models;

namespace Azure.Storage.Queues.Specialized.Models
{
    internal class EncryptedMessage
    {
        public string EncryptedMessageContents { get; set; }

        public EncryptionData EncryptionData { get; set; }
    }
}
