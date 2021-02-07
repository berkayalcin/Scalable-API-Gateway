using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.API.Domain.Models;
using Blog.API.Domain.Repositories;
using Couchbase;

namespace Blog.API.Domain.Services.Post
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public PostService(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public async Task<List<PostDto>> GetAll()
        {
            var posts = await _postRepository.GetAll();
            return null;
        }

        public async Task<PostDto> Create(PostDto postDto)
        {   
            var post = _mapper.Map<Entities.Post>(postDto);
            post = await _postRepository.Create(post);
            
            return _mapper.Map<PostDto>(post);
        }
    }
}