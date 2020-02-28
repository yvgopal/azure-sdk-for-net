// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Azure.Storage.QuickQuery.Samples
{
    public class QuickQuerySample : SampleTest
    {
        /// <summary>
        /// Creates a Stream of a CSV file.
        /// This is so we have some data to work with.
        /// </summary>
        /// <param name="length">The legnth of the stream to create.</param>
        protected Stream CreateCsvStream(long length)
        {
            MemoryStream stream = new MemoryStream();
            byte[] rowData = Encoding.UTF8.GetBytes("100,200,300,400\n300,400,500,600\n");
            long blockLength = 0;
            while (blockLength < length)
            {
                stream.Write(rowData, 0, rowData.Length);
                blockLength += rowData.Length;
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
