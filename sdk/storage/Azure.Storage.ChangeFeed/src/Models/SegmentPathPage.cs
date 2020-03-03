using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Storage.ChangeFeed.Models
{
    internal class SegmentPathPage : Page<string>
    {
        private readonly IReadOnlyList<string> _list;
        private readonly string _continuationToken;

        public SegmentPathPage(
            IReadOnlyList<string> list,
            string continuationToken)
        {
            _list = list;
            _continuationToken = continuationToken;
        }
        public override IReadOnlyList<string> Values => _list;
        public override string ContinuationToken => _continuationToken;
        public override Response GetRawResponse() => null;
    }
}
