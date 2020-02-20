// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Azure.Storage.Sas
{
    /// <summary>
    /// <see cref="AccountSasSignatureValues"/> is used to generate a Shared Access
    /// Signature (SAS) for an Azure Storage account.
    /// For more information, see <see href="https://docs.microsoft.com/en-us/rest/api/storageservices/create-account-sas" />.
    /// </summary>
    public class AccountSasSignatureValues
    {

        /// <summary>
        /// The optional signed protocol field specifies the protocol
        /// permitted for a request made with the SAS.  Possible values are
        /// <see cref="SasProtocol.HttpsAndHttp"/>,
        /// <see cref="SasProtocol.Https"/>, and
        /// <see cref="SasProtocol.None"/>.
        /// </summary>
        public SasProtocol Protocol { get; set; }

        /// <summary>
        /// Optionally specify the time at which the shared access signature
        /// becomes valid.  A lack of start time will produce a SAS immediately
        /// valid upon creation, subject to other restrictions imposed by
        /// <see cref="ExpiresOn"/>.
        /// </summary>
        public DateTimeOffset StartsOn { get; set; }

        /// <summary>
        /// The time at which the shared access signature becomes invalid.
        /// </summary>
        public DateTimeOffset ExpiresOn { get; }

        /// <summary>
        /// The permissions associated with the shared access signature. The
        /// user is restricted to operations allowed by the permissions. The
        /// <see cref="AccountSasPermissions"/> type can be used to create the
        /// permissions string.
        /// </summary>
        public string Permissions { get; }

        /// <summary>
        /// Specifies an IP address or a range of IP addresses from which to
        /// accept requests. If the IP address from which the request
        /// originates does not match the IP address or address range
        /// specified on the SAS token, the request is not authenticated.
        /// When specifying a range of IP addresses, note that the range is
        /// inclusive.
        /// </summary>
        public SasIPRange IPRange { get; set; }

        /// <summary>
        /// The services associated with the shared access signature. The
        /// user is restricted to operations with the specified services.
        /// </summary>
        public AccountSasServices Services { get; set; }

        /// <summary>
        /// The resource types associated with the shared access signature. The
        /// user is restricted to operations on the specified resources.
        /// </summary>
        public AccountSasResourceTypes ResourceTypes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountSasSignatureValues"/> class intended to describe access
        /// to a specific account.
        /// </summary>
        /// <param name="permissions">
        /// <see cref="AccountSasPermissions"/> containing the allowed permissions.
        /// </param>
        /// <param name="expiresOn">
        /// The time at which the shared access signature becomes invalid.
        /// </param>
        public AccountSasSignatureValues(AccountSasPermissions permissions, DateTimeOffset expiresOn)
        {
            Permissions = permissions.ToPermissionsString();
            ExpiresOn = expiresOn;
        }

        /* Internal "GenerateSas" methods for Account SAS are in an AccountSasSignatureValuesExtensions.cs,
         * explicitly compiled into each package that consumes Common. This is to have access to versioning
         * constants while still having a single public class in common that adheres to established namespace
         * patterns.
         */

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => base.ToString();

        /// <summary>
        /// Check if two <see cref="AccountSasBuilder"/> instances are equal.
        /// </summary>
        /// <param name="obj">The instance to compare to.</param>
        /// <returns>True if they're equal, false otherwise.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => base.Equals(obj);

        /// <summary>
        /// Get a hash code for the <see cref="AccountSasBuilder"/>.
        /// </summary>
        /// <returns>Hash code for the <see cref="AccountSasBuilder"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();
    }
}
