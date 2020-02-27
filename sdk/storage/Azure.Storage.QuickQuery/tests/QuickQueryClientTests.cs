// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Core.Testing;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.QuickQuery.Models;
using NUnit.Framework;

namespace Azure.Storage.QuickQuery.Tests
{
    public class QuickQueryClientTests : QuickQueryTestBase
    {
        public QuickQueryClientTests(bool async) : this(async, null) { }

        public QuickQueryClientTests(bool async, RecordedTestMode? mode = null)
            : base(async, mode) { }

        [Test]
        public async Task QueryAsync_Min()
        {
            // Arrange
            await using DisposingContainer test = await GetTestContainerAsync();
            BlockBlobClient blockBlobClient = InstrumentClient(test.Container.GetBlockBlobClient(GetNewBlobName()));

            MemoryStream blobData = new MemoryStream();
            byte[] rowData = Encoding.UTF8.GetBytes("100,200,300,400\n300,400,500,600\n");
            long blockLength = 0;
            while (blockLength < 1024)
            {
                blobData.Write(rowData, 0, rowData.Length);
                blockLength += rowData.Length;
            }

            blobData.Seek(0, SeekOrigin.Begin);

            await blockBlobClient.UploadAsync(blobData);

            // Act
            BlobQuickQueryClient queryClient = blockBlobClient.GetQuickQueryClient();
            string query = @"SELECT _2 from BlobStorage WHERE _1 > 250;";
            Response<BlobDownloadInfo> response =  await queryClient.QueryAsync(query);

            using StreamReader streamReader = new StreamReader(response.Value.Content);
            string s = await streamReader.ReadToEndAsync();
            Console.WriteLine(s);

            // Assert
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task QueryAsync_NonFatalError()
        {
            // Arrange
            await using DisposingContainer test = await GetTestContainerAsync();
            BlockBlobClient blockBlobClient = InstrumentClient(test.Container.GetBlockBlobClient(GetNewBlobName()));

            MemoryStream blobData = new MemoryStream();
            byte[] rowData = Encoding.UTF8.GetBytes("100,200,300,400\n300,400,500,600\n");
            long blockLength = 0;
            while (blockLength < 1024)
            {
                blobData.Write(rowData, 0, rowData.Length);
                blockLength += rowData.Length;
            }

            blobData.Seek(0, SeekOrigin.Begin);

            await blockBlobClient.UploadAsync(blobData);

            // Act
            BlobQuickQueryClient queryClient = blockBlobClient.GetQuickQueryClient();
            string query = @"SELECT * from BlobStorage;";
            CvsTextConfiguration cvsTextConfiguration = new CvsTextConfiguration
            {
                ColumnSeparator = ',',
                FieldQuote ='\"',
                RecordSeparator = '\n',
                EscapeCharacter = null,
                HasHeaders = false
            };

            Response<BlobDownloadInfo> response = await queryClient.QueryAsync(
                query,
                inputTextConfiguration: cvsTextConfiguration);

            using StreamReader streamReader = new StreamReader(response.Value.Content);
            string s = await streamReader.ReadToEndAsync();
            Console.WriteLine(s);

            // Assert
            Assert.IsNotNull(response);
        }

    }
}
