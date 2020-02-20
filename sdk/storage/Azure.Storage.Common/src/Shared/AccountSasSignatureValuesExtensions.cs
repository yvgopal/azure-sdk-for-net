// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using static Azure.Storage.Blobs.BlobClientOptions;

namespace Azure.Storage.Sas
{
    /// <summary>
    /// Extension methods for <see cref="AccountSasSignatureValues"/>, allowing internal mechanisms
    /// in a publically shared class.
    /// </summary>
    internal static class AccountSasSignatureValuesExtensions
    {
        internal static string GenerateSas(
            this AccountSasSignatureValues values,
            StorageSharedKeyCredential sharedKeyCredential)
        {
            // https://docs.microsoft.com/en-us/rest/api/storageservices/Constructing-an-Account-SAS
            sharedKeyCredential = sharedKeyCredential ?? throw Errors.ArgumentNull(nameof(sharedKeyCredential));

            if (values.ExpiresOn == default || string.IsNullOrEmpty(values.Permissions) || values.ResourceTypes == default || values.Services == default)
            {
                throw Errors.AccountSasMissingData();
            }
            //if (string.IsNullOrEmpty(Version))
            {
                //Version = SasQueryParameters.DefaultSasVersion;
            }
            var startTime = SasExtensions.FormatTimesForSasSigning(values.StartsOn);
            var expiryTime = SasExtensions.FormatTimesForSasSigning(values.ExpiresOn);

            // String to sign: http://msdn.microsoft.com/en-us/library/azure/dn140255.aspx
            var stringToSign = string.Join("\n",
                sharedKeyCredential.AccountName,
                values.Permissions,
                values.Services.ToPermissionsString(),
                values.ResourceTypes.ToPermissionsString(),
                startTime,
                expiryTime,
                values.IPRange.ToString(),
                values.Protocol.ToProtocolString(),
                LatestVersion.ToVersionString(),
                "");  // That's right, the account SAS requires a terminating extra newline

            var signature = StorageSharedKeyCredentialInternals.ComputeSasSignature(sharedKeyCredential, stringToSign);

            throw new NotImplementedException();
        }
    }
}
