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
            var search = _elasticSearchService.Client
                .Search<PostDto>(s =>
                    s.Index("posts")
                        .Query(q =>
                            q.MultiMatch(c => c
                                .Fields(p => p.Field(f => f.Name))
                                .Analyzer("standard")
                                .Boost(1.1)
                                .Query(query)
                                .Fuzziness(Fuzziness.AutoLength(3, 6))
                                .Lenient()
                                .FuzzyTranspositions()
                                .MinimumShouldMatch(2)
                                .Operator(Operator.Or)
                                .FuzzyRewrite(MultiTermQueryRewrite.TopTermsBlendedFreqs(10))
                                .Name("named_query")
                                .AutoGenerateSynonymsPhraseQuery(false)
                            )
                        )
                        .StoredFields(sf =>
                            sf.Field(f => f.Name).Field(f => f.Id)
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