using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace Blog.API.Domain.Services.ElasticSearch
{
    public interface IElasticSearchService
    {
        Task AddOrUpdate<T>(T data, string indexName)
            where T : class;

        Task CreateIndex(string indexName);
        Task BulkAddOrUpdate<T>(IEnumerable<T> data, string indexName) where T : class;
        Task CreateIndexIfNotExists(string indexName);
        Task<ISearchResponse<T>> Search<T>(SearchRequest searchRequest) where T : class;
        ElasticClient Client { get; }
    }
}