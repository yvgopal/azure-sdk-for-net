namespace Azure.Search
{
    public partial class SearchApiKeyCredential
    {
        public SearchApiKeyCredential(string apiKey) { }
        public void Refresh(string apiKey) { }
    }
    public partial class SearchClientOptions : Azure.Core.ClientOptions
    {
        public SearchClientOptions(Azure.Search.SearchClientOptions.ServiceVersion version = Azure.Search.SearchClientOptions.ServiceVersion.V2019_05_06) { }
        public Azure.Search.SearchClientOptions.ServiceVersion Version { get { throw null; } }
        public enum ServiceVersion
        {
            V2019_05_06 = 1,
        }
    }
    public partial class SearchIndexClient
    {
        protected SearchIndexClient() { }
        public SearchIndexClient(System.Uri endpoint, string indexName, Azure.Search.SearchApiKeyCredential credential) { }
        public SearchIndexClient(System.Uri endpoint, string indexName, Azure.Search.SearchApiKeyCredential credential, Azure.Search.SearchClientOptions options) { }
        public virtual System.Uri Endpoint { get { throw null; } }
        public virtual string IndexName { get { throw null; } }
        public virtual string ServiceName { get { throw null; } }
        public virtual Azure.Response<long> GetCount(System.Guid? clientRequestId = default(System.Guid?), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public virtual System.Threading.Tasks.Task<Azure.Response<long>> GetCountAsync(System.Guid? clientRequestId = default(System.Guid?), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
    }
    public partial class SearchServiceClient
    {
        protected SearchServiceClient() { }
        public SearchServiceClient(System.Uri endpoint, Azure.Search.SearchApiKeyCredential credential) { }
        public SearchServiceClient(System.Uri endpoint, Azure.Search.SearchApiKeyCredential credential, Azure.Search.SearchClientOptions options) { }
        public virtual System.Uri Endpoint { get { throw null; } }
        public virtual string ServiceName { get { throw null; } }
        public virtual Azure.Search.SearchIndexClient GetSearchIndexClient(string indexName) { throw null; }
        public virtual Azure.Response<Azure.Search.Models.SearchServiceStatistics> GetStatistics(System.Guid? clientRequestId = default(System.Guid?), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public virtual System.Threading.Tasks.Task<Azure.Response<Azure.Search.Models.SearchServiceStatistics>> GetStatisticsAsync(System.Guid? clientRequestId = default(System.Guid?), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
    }
}
namespace Azure.Search.Models
{
    public partial class AutocompleteItem
    {
        public AutocompleteItem() { }
        public string QueryPlusText { get { throw null; } }
        public string Text { get { throw null; } }
    }
    public enum AutocompleteMode
    {
        OneTerm = 0,
        TwoTerms = 1,
        OneTermWithContext = 2,
    }
    public partial class AutocompleteRequest
    {
        public AutocompleteRequest() { }
        public Azure.Search.Models.AutocompleteMode? AutocompleteMode { get { throw null; } set { } }
        public string Filter { get { throw null; } set { } }
        public string HighlightPostTag { get { throw null; } set { } }
        public string HighlightPreTag { get { throw null; } set { } }
        public double? MinimumCoverage { get { throw null; } set { } }
        public string SearchFields { get { throw null; } set { } }
        public string SearchText { get { throw null; } set { } }
        public string SuggesterName { get { throw null; } set { } }
        public int? Top { get { throw null; } set { } }
        public bool? UseFuzzyMatching { get { throw null; } set { } }
    }
    public partial class AutocompleteResult
    {
        public AutocompleteResult() { }
        public double? Coverage { get { throw null; } }
        public System.Collections.Generic.IList<Azure.Search.Models.AutocompleteItem> Results { get { throw null; } }
    }
    public partial class FacetResult : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.Generic.IDictionary<string, object>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.IEnumerable
    {
        public FacetResult() { }
        public long? Count { get { throw null; } }
        public object this[string key] { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<string> Keys { get { throw null; } }
        int System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Count { get { throw null; } }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.IsReadOnly { get { throw null; } }
        public System.Collections.Generic.ICollection<object> Values { get { throw null; } }
        public void Add(string key, object value) { }
        public bool ContainsKey(string key) { throw null; }
        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>> GetEnumerator() { throw null; }
        public bool Remove(string key) { throw null; }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Add(System.Collections.Generic.KeyValuePair<string, object> value) { }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Clear() { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Contains(System.Collections.Generic.KeyValuePair<string, object> value) { throw null; }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.CopyTo(System.Collections.Generic.KeyValuePair<string, object>[] destination, int offset) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Remove(System.Collections.Generic.KeyValuePair<string, object> value) { throw null; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
        public bool TryGetValue(string key, out object value) { throw null; }
    }
    public partial class IndexAction : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.Generic.IDictionary<string, object>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.IEnumerable
    {
        public IndexAction() { }
        public Azure.Search.Models.IndexActionType? ActionType { get { throw null; } set { } }
        public object this[string key] { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<string> Keys { get { throw null; } }
        int System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Count { get { throw null; } }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.IsReadOnly { get { throw null; } }
        public System.Collections.Generic.ICollection<object> Values { get { throw null; } }
        public void Add(string key, object value) { }
        public bool ContainsKey(string key) { throw null; }
        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>> GetEnumerator() { throw null; }
        public bool Remove(string key) { throw null; }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Add(System.Collections.Generic.KeyValuePair<string, object> value) { }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Clear() { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Contains(System.Collections.Generic.KeyValuePair<string, object> value) { throw null; }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.CopyTo(System.Collections.Generic.KeyValuePair<string, object>[] destination, int offset) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Remove(System.Collections.Generic.KeyValuePair<string, object> value) { throw null; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
        public bool TryGetValue(string key, out object value) { throw null; }
    }
    public enum IndexActionType
    {
        Upload = 0,
        Merge = 1,
        MergeOrUpload = 2,
        Delete = 3,
    }
    public partial class IndexBatch
    {
        public IndexBatch() { }
        public System.Collections.Generic.IList<Azure.Search.Models.IndexAction> Actions { get { throw null; } set { } }
    }
    public partial class IndexDocumentsResult
    {
        public IndexDocumentsResult() { }
        public System.Collections.Generic.IList<Azure.Search.Models.IndexingResult> Results { get { throw null; } }
    }
    public partial class IndexingResult
    {
        public IndexingResult() { }
        public string ErrorMessage { get { throw null; } }
        public string Key { get { throw null; } }
        public int? StatusCode { get { throw null; } }
        public bool? Succeeded { get { throw null; } }
    }
    public enum QueryType
    {
        Simple = 0,
        Full = 1,
    }
    public partial class SearchDocumentsResult
    {
        public SearchDocumentsResult() { }
        public long? Count { get { throw null; } }
        public double? Coverage { get { throw null; } }
        public System.Collections.Generic.IDictionary<string, System.Collections.Generic.IList<Azure.Search.Models.FacetResult>> Facets { get { throw null; } }
        public string NextLink { get { throw null; } }
        public Azure.Search.Models.SearchRequest NextPageParameters { get { throw null; } }
        public System.Collections.Generic.IList<Azure.Search.Models.SearchResult> Results { get { throw null; } }
    }
    public enum SearchMode
    {
        Any = 0,
        All = 1,
    }
    public partial class SearchRequest
    {
        public SearchRequest() { }
        public System.Collections.Generic.IList<string> Facets { get { throw null; } set { } }
        public string Filter { get { throw null; } set { } }
        public string HighlightFields { get { throw null; } set { } }
        public string HighlightPostTag { get { throw null; } set { } }
        public string HighlightPreTag { get { throw null; } set { } }
        public bool? IncludeTotalResultCount { get { throw null; } set { } }
        public double? MinimumCoverage { get { throw null; } set { } }
        public string OrderBy { get { throw null; } set { } }
        public Azure.Search.Models.QueryType? QueryType { get { throw null; } set { } }
        public System.Collections.Generic.IList<string> ScoringParameters { get { throw null; } set { } }
        public string ScoringProfile { get { throw null; } set { } }
        public string SearchFields { get { throw null; } set { } }
        public Azure.Search.Models.SearchMode? SearchMode { get { throw null; } set { } }
        public string SearchText { get { throw null; } set { } }
        public string Select { get { throw null; } set { } }
        public int? Skip { get { throw null; } set { } }
        public int? Top { get { throw null; } set { } }
    }
    public partial class SearchResourceCounter
    {
        public SearchResourceCounter() { }
        public long? Quota { get { throw null; } set { } }
        public long? Usage { get { throw null; } set { } }
    }
    public partial class SearchResult : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.Generic.IDictionary<string, object>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.IEnumerable
    {
        public SearchResult() { }
        public System.Collections.Generic.IDictionary<string, System.Collections.Generic.IList<string>> Highlights { get { throw null; } }
        public object this[string key] { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<string> Keys { get { throw null; } }
        public double? Score { get { throw null; } }
        int System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Count { get { throw null; } }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.IsReadOnly { get { throw null; } }
        public System.Collections.Generic.ICollection<object> Values { get { throw null; } }
        public void Add(string key, object value) { }
        public bool ContainsKey(string key) { throw null; }
        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>> GetEnumerator() { throw null; }
        public bool Remove(string key) { throw null; }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Add(System.Collections.Generic.KeyValuePair<string, object> value) { }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Clear() { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Contains(System.Collections.Generic.KeyValuePair<string, object> value) { throw null; }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.CopyTo(System.Collections.Generic.KeyValuePair<string, object>[] destination, int offset) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Remove(System.Collections.Generic.KeyValuePair<string, object> value) { throw null; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
        public bool TryGetValue(string key, out object value) { throw null; }
    }
    public partial class SearchServiceCounters
    {
        public SearchServiceCounters() { }
        public Azure.Search.Models.SearchResourceCounter DataSourceCounter { get { throw null; } set { } }
        public Azure.Search.Models.SearchResourceCounter DocumentCounter { get { throw null; } set { } }
        public Azure.Search.Models.SearchResourceCounter IndexCounter { get { throw null; } set { } }
        public Azure.Search.Models.SearchResourceCounter IndexerCounter { get { throw null; } set { } }
        public Azure.Search.Models.SearchResourceCounter StorageSizeCounter { get { throw null; } set { } }
        public Azure.Search.Models.SearchResourceCounter SynonymMapCounter { get { throw null; } set { } }
    }
    public partial class SearchServiceLimits
    {
        public SearchServiceLimits() { }
        public int? MaxComplexCollectionFieldsPerIndex { get { throw null; } set { } }
        public int? MaxComplexObjectsInCollectionsPerDocument { get { throw null; } set { } }
        public int? MaxFieldNestingDepthPerIndex { get { throw null; } set { } }
        public int? MaxFieldsPerIndex { get { throw null; } set { } }
    }
    public partial class SearchServiceStatistics
    {
        public SearchServiceStatistics() { }
        public Azure.Search.Models.SearchServiceCounters Counters { get { throw null; } set { } }
        public Azure.Search.Models.SearchServiceLimits Limits { get { throw null; } set { } }
    }
    public partial class SuggestDocumentsResult
    {
        public SuggestDocumentsResult() { }
        public double? Coverage { get { throw null; } }
        public System.Collections.Generic.IList<Azure.Search.Models.SuggestResult> Results { get { throw null; } }
    }
    public partial class SuggestRequest
    {
        public SuggestRequest() { }
        public string Filter { get { throw null; } set { } }
        public string HighlightPostTag { get { throw null; } set { } }
        public string HighlightPreTag { get { throw null; } set { } }
        public double? MinimumCoverage { get { throw null; } set { } }
        public string OrderBy { get { throw null; } set { } }
        public string SearchFields { get { throw null; } set { } }
        public string SearchText { get { throw null; } set { } }
        public string Select { get { throw null; } set { } }
        public string SuggesterName { get { throw null; } set { } }
        public int? Top { get { throw null; } set { } }
        public bool? UseFuzzyMatching { get { throw null; } set { } }
    }
    public partial class SuggestResult : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.Generic.IDictionary<string, object>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.IEnumerable
    {
        public SuggestResult() { }
        public object this[string key] { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<string> Keys { get { throw null; } }
        int System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Count { get { throw null; } }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.IsReadOnly { get { throw null; } }
        public string Text { get { throw null; } }
        public System.Collections.Generic.ICollection<object> Values { get { throw null; } }
        public void Add(string key, object value) { }
        public bool ContainsKey(string key) { throw null; }
        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>> GetEnumerator() { throw null; }
        public bool Remove(string key) { throw null; }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Add(System.Collections.Generic.KeyValuePair<string, object> value) { }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Clear() { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Contains(System.Collections.Generic.KeyValuePair<string, object> value) { throw null; }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.CopyTo(System.Collections.Generic.KeyValuePair<string, object>[] destination, int offset) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String,System.Object>>.Remove(System.Collections.Generic.KeyValuePair<string, object> value) { throw null; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
        public bool TryGetValue(string key, out object value) { throw null; }
    }
}
