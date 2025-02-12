namespace ODBP.Apis.Search
{
    public interface ISearchClient
    {
        Task<PaginatedSearchResults> Search(SearchRequest request, CancellationToken token);
    }
}
