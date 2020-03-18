// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Moq;
using NUnit.Framework;

namespace Azure.Storage.ChangeFeed.Tests
{
    public class ChangeFeedTests : ChangeFeedTestBase
    {
        public ChangeFeedTests(bool async)
            : base(async, null /* RecordedTestMode.Record /* to re-record */)
        {
        }

        [Test]
        public async Task GetYearPathsTest()
        {
            // Arrange
            Mock<BlobContainerClient> containerClient = new Mock<BlobContainerClient>();
            ChangeFeed changeFeed = new ChangeFeed(containerClient.Object);

            //if (IsAsync)
            //{
                AsyncPageable<BlobHierarchyItem> asyncPageable = PageResponseEnumerator.CreateAsyncEnumerable<BlobHierarchyItem>(GetYearsPathFuncAsync);

                containerClient.Setup(r => r.GetBlobsByHierarchyAsync(
                    default,
                    default,
                    Constants.ChangeFeed.SegmentPrefix,
                    "/",
                    default)).Returns(asyncPageable);
            //}
            //else
            //{
                Pageable<BlobHierarchyItem> pageable =
                    PageResponseEnumerator.CreateEnumerable<BlobHierarchyItem>(GetYearPathFunc);

                containerClient.Setup(r => r.GetBlobsByHierarchy(
                    default,
                    default,
                    Constants.ChangeFeed.SegmentPrefix,
                    "/",
                    default)).Returns(pageable);
            //}

            // Act
            Queue<string> years = await changeFeed.GetYearPaths(IsAsync).ConfigureAwait(false);

            // Assert
            Queue<string> expectedYears = new Queue<string>();
            expectedYears.Enqueue("idx/segments/2019/");
            expectedYears.Enqueue("idx/segments/2020/");
            expectedYears.Enqueue("idx/segments/2022/");
            expectedYears.Enqueue("idx/segments/2023/");
            Assert.AreEqual(expectedYears, years);

        }

        private static Task<Page<BlobHierarchyItem>> GetYearsPathFuncAsync(string continuation, int? pageSizeHint)
            => Task.FromResult(GetYearPathFunc(continuation, pageSizeHint));

        private static Page<BlobHierarchyItem> GetYearPathFunc(string continuation, int? pageSizeHint)
            => new BlobHierarchyItemPage(new List<BlobHierarchyItem>
            {
                BlobsModelFactory.BlobHierarchyItem("idx/segments/2019/", null),
                BlobsModelFactory.BlobHierarchyItem("idx/segments/2020/", null),
                BlobsModelFactory.BlobHierarchyItem("idx/segments/2022/", null),
                BlobsModelFactory.BlobHierarchyItem("idx/segments/2023/", null),
            });

        private class BlobHierarchyItemPage : Page<BlobHierarchyItem>
        {
            private List<BlobHierarchyItem> _items;

            public BlobHierarchyItemPage(List<BlobHierarchyItem> items)
            {
                _items = items;
            }

            public override IReadOnlyList<BlobHierarchyItem> Values => _items;

            public override string ContinuationToken => throw new NotImplementedException();

            public override Response GetRawResponse()
            {
                throw new NotImplementedException();
            }
        }
    }
}
