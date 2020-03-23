// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Azure.Storage.Internal.Avro.Tests
{
    public class AvroReaderTests
    {
        [Test]
        public async Task Test()
        {
            // Arrange
            //using FileStream stream = File.OpenRead("C:\\Users\\Sean\\Desktop\\avro\\quickQuery.avro");
            using FileStream stream = File.OpenRead("C:\\Users\\Sean\\Desktop\\avro\\changeFeed.avro");
            AvroReader avroReader = new AvroReader(stream);

            // Act
            object o = await avroReader.Next(async: true).ConfigureAwait(false);
            bool hasNext = avroReader.HasNext();
        }
    }
}
