using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.API.Domain.Models;
using Blog.API.Domain.Repositories;
using Blog.API.Domain.Services.ElasticSearch;
using Couchbase;
using Nest;

namespace Blog.API.Domain.Services.Post
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IElasticSearchService _elasticSearchService;

        public PostService(IPostRepository postRepository, IMapper mapper, IElasticSearchService elasticSearchService)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _elasticSearchService = elasticSearchService;
        }

        public async Task<List<PostDto>> GetAll(string query)
        {
            var searchResponse = await _elasticSearchService.Client.SearchAsync<PostDto>(s =>
                s.Index("posts")
                    .Query(q =>
                        q.Match(m =>
                            m.Field(f => f.IsDeleted)
                                .Query("false")
                        ) &&
                        q.QueryString(c => c
                            .Fields(f =>
                                f.Field(p => p.Name)
                                    .Field(p => p.ShortDescription)
                                    .Field(p => p.FullDescription)
                            )
                            .Boost(1.1)
                            .Query(query)
                            .Analyzer("standard")
                            .DefaultOperator(Operator.Or)
                            .Lenient()
                            .AnalyzeWildcard()
                            .MinimumShouldMatch("40%")
                            .FuzzyPrefixLength(0)
                            .FuzzyMaxExpansions(50)
                            .FuzzyTranspositions()
                            .AutoGenerateSynonymsPhraseQuery(false)
                        )
                    )
                    .StoredFields(sf =>
                        sf.Fields(f => f.Id, f => f.Name)
                    )
            );


            
            var posts = await _postRepository.GetAll();
            return null;
        }

        public async Task<PostDto> Create(PostDto postDto)
        {
            var post = _mapper.Map<Entities.Post>(postDto);
            post = await _postRepository.Create(post);
            await _elasticSearchService.AddOrUpdate(post, "posts");

            return _mapper.Map<PostDto>(post);
        }
    }
}