// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Azure.Core.Testing;
using Azure.Storage.Blobs;
using Azure.Storage.Test;
using NUnit.Framework;

namespace Azure.Storage.Blobs.Test
{
    public class BlobQuickQueryStreamTests
    {
        [Test]
        public void ValidateReadParameters()
        {
            TestHelper.AssertExpectedException(
                () => BlobQuickQueryStream.ValidateReadParameters(buffer: null, offset: 0, count: 0),
                new ArgumentException($"Parameter cannot be null.{Environment.NewLine}Parameter name: buffer"));

            TestHelper.AssertExpectedException(
                () => BlobQuickQueryStream.ValidateReadParameters(buffer: new byte[10], offset: -1, count: 0),
                new ArgumentException($"Parameter cannot be negative.{Environment.NewLine}Parameter name: offset"));

            TestHelper.AssertExpectedException(
                () => BlobQuickQueryStream.ValidateReadParameters(buffer: new byte[10], offset: 0, count: -1),
                new ArgumentException($"Parameter cannot be negative.{Environment.NewLine}Parameter name: count"));

            TestHelper.AssertExpectedException(
                () => BlobQuickQueryStream.ValidateReadParameters(buffer: new byte[5], offset: 6, count: 6),
                new ArgumentException("The sum of offset and count cannot be greater than buffer length."));
        }
    }
}
