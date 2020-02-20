// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using static Azure.Storage.Blobs.BlobClientOptions;

namespace Azure.Storage.Sas
{
    /// <summary>
    /// <see cref="BlobSasSignatureValues"/> is used to generate a Shared Access
    /// Signature (SAS) for an Azure Storage container or blob.
    /// For more information, see <see href="https://docs.microsoft.com/en-us/rest/api/storageservices/constructing-a-service-sas" />.
    /// </summary>
    public class BlobSasSignatureValues
    {
        /// <summary>
        /// The optional signed protocol field specifies the protocol
        /// permitted for a request made with the SAS.  Possible values are
        /// <see cref="SasProtocol.HttpsAndHttp"/>,
        /// <see cref="SasProtocol.Https"/>, and
        /// <see cref="SasProtocol.None"/>.
        /// </summary>
        public SasProtocol Protocol { get; set; } = SasProtocol.None;

        /// <summary>
        /// Optionally specify the time at which the shared access signature
        /// becomes valid.  A lack of start time will produce a SAS immediately
        /// valid upon creation, subject to other restrictions imposed by
        /// <see cref="ExpiresOn"/> or access policy associated with
        /// <see cref="Identifier"/>, whichever is specified.
        /// </summary>
        public DateTimeOffset StartsOn { get; set; }

        /// <summary>
        /// The time at which the shared access signature becomes invalid. Not
        /// present if <see cref="Identifier"/> has a value.
        /// </summary>
        public DateTimeOffset ExpiresOn { get; }

        /// <summary>
        /// The raw permissions string associated with the shared access signature.
        /// The user is restricted to operations allowed by the permissions. Not
        /// present if <see cref="Identifier"/> has a value.
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
        /// An optional unique value up to 64 characters in length that
        /// correlates to an access policy specified for the container.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Override the value returned for Cache-Control response header.
        /// </summary>
        public string CacheControl { get; set; }

        /// <summary>
        /// Override the value returned for Content-Disposition response
        /// header.
        /// </summary>
        public string ContentDisposition { get; set; }

        /// <summary>
        /// Override the value returned for Cache-Encoding response header.
        /// </summary>
        public string ContentEncoding { get; set; }

        /// <summary>
        /// Override the value returned for Cache-Language response header.
        /// </summary>
        public string ContentLanguage { get; set; }

        /// <summary>
        /// Override the value returned for Cache-Type response header.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobSasSignatureValues"/> class intended to describe access
        /// to a blob storage account.
        /// </summary>
        /// <param name="permissions">
        /// <see cref="BlobAccountSasPermissions"/> containing the allowed permissions.
        /// </param>
        /// <param name="expiresOn">
        /// The time at which the shared access signature becomes invalid.
        /// </param>
        public BlobSasSignatureValues(BlobAccountSasPermissions permissions, DateTimeOffset expiresOn)
        {
            Permissions = permissions.ToPermissionsString();
            ExpiresOn = expiresOn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobSasSignatureValues"/> class intended to describe access
        /// to a specific container.
        /// </summary>
        /// <param name="permissions">
        /// <see cref="BlobContainerSasPermissions"/> containing the allowed permissions.
        /// </param>
        /// <param name="expiresOn">
        /// The time at which the shared access signature becomes invalid.
        /// </param>
        public BlobSasSignatureValues(BlobContainerSasPermissions permissions, DateTimeOffset expiresOn)
        {
            Permissions = permissions.ToPermissionsString();
            ExpiresOn = expiresOn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobSasSignatureValues"/> class intended to describe access
        /// to a specific blob.
        /// </summary>
        /// <param name="permissions">
        /// <see cref="BlobSasPermissions"/> containing the allowed permissions.
        /// </param>
        /// <param name="expiresOn">
        /// The time at which the shared access signature becomes invalid.
        /// </param>
        public BlobSasSignatureValues(BlobSasPermissions permissions, DateTimeOffset expiresOn)
        {
            Permissions = permissions.ToPermissionsString();
            ExpiresOn = expiresOn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobSasSignatureValues"/> class intended to describe access
        /// to a specific snapshot of a specific blob.
        /// </summary>
        /// <param name="permissions">
        /// <see cref="SnapshotSasPermissions"/> containing the allowed permissions.
        /// </param>
        /// <param name="expiresOn">
        /// The time at which the shared access signature becomes invalid.
        /// </param>
        public BlobSasSignatureValues(SnapshotSasPermissions permissions, DateTimeOffset expiresOn)
        {
            Permissions = permissions.ToPermissionsString();
            ExpiresOn = expiresOn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobSasSignatureValues"/> class.
        ///
        /// For more information, see <see href="https://docs.microsoft.com/en-us/rest/api/storageservices/define-stored-access-policy"/>.
        /// </summary>
        /// <param name="identifier">
        /// Identifier for a service-side access policy this SAS will be bound to.
        /// </param>
        public BlobSasSignatureValues(string identifier)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Use an account's <see cref="StorageSharedKeyCredential"/> to sign this
        /// shared access signature values to produce the proper SAS query
        /// parameters for authenticating requests.
        /// </summary>
        /// <param name="sharedKeyCredential">
        /// The storage account's <see cref="StorageSharedKeyCredential"/>.
        /// </param>
        /// <param name="resource">Signed resource for this SAS.</param>
        /// <param name="path">Path of the resource in this container.</param>
        /// <param name="snapshotId">Snapshot ID of a blob specidied by <paramref name="path"/>.</param>
        /// <returns>
        /// The sas token.
        /// </returns>
        internal string GenerateSas(
            StorageSharedKeyCredential sharedKeyCredential,
            string resource,
            string path = default,
            string snapshotId = default)
        {
            sharedKeyCredential = sharedKeyCredential ?? throw Errors.ArgumentNull(nameof(sharedKeyCredential));
            if (!string.IsNullOrEmpty(snapshotId) && string.IsNullOrEmpty(path))
            {
                throw Errors.ArgumentNull(nameof(path));
            }

            var startTime = SasExtensions.FormatTimesForSasSigning(StartsOn);
            var expiryTime = SasExtensions.FormatTimesForSasSigning(ExpiresOn);

            // See http://msdn.microsoft.com/en-us/library/azure/dn140255.aspx
            var stringToSign = string.Join("\n",
                Permissions,
                startTime,
                expiryTime,
                GetCanonicalName(sharedKeyCredential.AccountName, path ?? string.Empty),
                Identifier ?? string.Empty,
                IPRange.ToString(),
                SasExtensions.ToProtocolString(Protocol),
                LatestVersion.ToVersionString(),
                resource,
                snapshotId,
                CacheControl,
                ContentDisposition,
                ContentEncoding,
                ContentLanguage,
                ContentType);

            var signature = StorageSharedKeyCredentialInternals.ComputeSasSignature(sharedKeyCredential, stringToSign);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Use an account's <see cref="UserDelegationKey"/> to sign this
        /// shared access signature values to produce the proper SAS query
        /// parameters for authenticating requests.
        /// </summary>
        /// <param name="userDelegationKey">
        /// A <see cref="UserDelegationKey"/> returned from
        /// <see cref="BlobServiceClient.GetUserDelegationKeyAsync"/>.
        /// </param>
        /// <param name="accountName">The name of the storage account.</param>
        /// <param name="resource">Signed resource for this SAS.</param>
        /// <param name="path">Path of the resource in this container.</param>
        /// <param name="snapshotId">Snapshot ID of a blob specidied by <paramref name="path"/>.</param>
        /// <returns>
        /// The <see cref="BlobSasQueryParameters"/> used for authenticating requests.
        /// </returns>
        internal string GenerateSas(
            UserDelegationKey userDelegationKey,
            string accountName,
            string resource,
            string path = default,
            string snapshotId = default)
        {
            userDelegationKey = userDelegationKey ?? throw Errors.ArgumentNull(nameof(userDelegationKey));
            if (Identifier != default)
            {
                throw Errors.UserDelegationSasWithIdentifier();
            }
            if (!string.IsNullOrEmpty(snapshotId) && string.IsNullOrEmpty(path))
            {
                throw Errors.ArgumentNull(nameof(path));
            }

            var startTime = SasExtensions.FormatTimesForSasSigning(StartsOn);
            var expiryTime = SasExtensions.FormatTimesForSasSigning(ExpiresOn);
            var userDelegationStartTime = SasExtensions.FormatTimesForSasSigning(userDelegationKey.SignedStartsOn);
            var userDelgationExpiryTime = SasExtensions.FormatTimesForSasSigning(userDelegationKey.SignedExpiresOn);

            // See http://msdn.microsoft.com/en-us/library/azure/dn140255.aspx
            var stringToSign = string.Join("\n",
                Permissions,
                startTime,
                expiryTime,
                GetCanonicalName(accountName, path ?? string.Empty),
                userDelegationKey.SignedObjectId,
                userDelegationKey.SignedTenantId,
                userDelegationStartTime,
                userDelgationExpiryTime,
                userDelegationKey.SignedService,
                userDelegationKey.SignedVersion,
                IPRange.ToString(),
                SasExtensions.ToProtocolString(Protocol),
                LatestVersion.ToVersionString(),
                resource,
                snapshotId,
                CacheControl,
                ContentDisposition,
                ContentEncoding,
                ContentLanguage,
                ContentType);

            var signature = ComputeHMACSHA256(userDelegationKey.Value, stringToSign);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Computes the canonical name for a container or blob resource for SAS signing.
        /// Container: "/blob/account/containername"
        /// Blob: "/blob/account/containername/blobname"
        /// </summary>
        /// <param name="account">The name of the storage account.</param>
        /// <param name="path">The resource path.</param>
        /// <returns>The canonical resource name.</returns>
        private static string GetCanonicalName(string account, string path)
            => $"/blob/{account}/{path.Replace("\\", "/")}";

        /// <summary>
        /// ComputeHMACSHA256 generates a base-64 hash signature string for an
        /// HTTP request or for a SAS.
        /// </summary>
        /// <param name="userDelegationKeyValue">
        /// A <see cref="UserDelegationKey.Value"/> used to sign with a key
        /// representing AD credentials.
        /// </param>
        /// <param name="message">The message to sign.</param>
        /// <returns>The signed message.</returns>
        private static string ComputeHMACSHA256(string userDelegationKeyValue, string message) =>
            Convert.ToBase64String(
                new HMACSHA256(
                    Convert.FromBase64String(userDelegationKeyValue))
                .ComputeHash(Encoding.UTF8.GetBytes(message)));

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() =>
            base.ToString();

        /// <summary>
        /// Check if two BlobSasBuilder instances are equal.
        /// </summary>
        /// <param name="obj">The instance to compare to.</param>
        /// <returns>True if they're equal, false otherwise.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
            => base.Equals(obj);

        /// <summary>
        /// Get a hash code for the BlobSasBuilder.
        /// </summary>
        /// <returns>Hash code for the BlobSasBuilder.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();
    }
}
