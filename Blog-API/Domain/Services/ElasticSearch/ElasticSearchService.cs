using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace Blog.API.Domain.Services.ElasticSearch
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly ElasticClient _client;

        public ElasticSearchService(ElasticClient client)
        {
            _client = client;
        }

        public ElasticClient Client => _client;

        public async Task<ISearchResponse<T>> Search<T>(SearchRequest searchRequest) where T : class
        {
            var result = _client.Search<T>(searchRequest);
            if (!result.IsValid)
                throw new OperationCanceledException(result.ServerError.ToString());

            return result;
        }

        public async Task CreateIndex(string indexName)
        {
            var createIndexResponse = await _client.Indices
                .CreateAsync(indexName, s => s
                    .Settings(se => se
                        .NumberOfReplicas(2)
                        .NumberOfShards(2)));

            if (!createIndexResponse.IsValid)
            {
                throw new OperationCanceledException(createIndexResponse.ServerError.ToString());
            }
        }

        public async Task BulkAddOrUpdate<T>(IEnumerable<T> data, string indexName) where T : class
        {
            await CreateIndexIfNotExists(indexName);

            _client.BulkAll(data, b =>
                b
                    .Index(indexName)
                    .BackOffTime("30s")
                    .BackOffRetries(2)
                    .RefreshOnCompleted()
                    .MaxDegreeOfParallelism(Environment.ProcessorCount)
                    .Size(1000)
            ).Wait(TimeSpan.FromMinutes(15), next => { });
        }

        public async Task AddOrUpdate<T>(T data, string indexName) where T : class
        {
            await CreateIndexIfNotExists(indexName);

            var indexResponse = await _client.IndexAsync(data, i => i.Index(indexName));
            if (!indexResponse.IsValid)
                throw new OperationCanceledException(indexResponse.ServerError.ToString());
        }

        public async Task CreateIndexIfNotExists(string indexName)
        {
            if ((await _client.Indices.ExistsAsync(indexName)).Exists == false)
            {
                await CreateIndex(indexName);
            }
        }
    }
}