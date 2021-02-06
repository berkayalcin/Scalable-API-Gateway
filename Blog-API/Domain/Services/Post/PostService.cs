using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Domain.Models;
using Blog.API.Domain.Repositories;

namespace Blog.API.Domain.Services.Post
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<PostDto>> GetAll()
        {
            var posts = await _postRepository.GetAll();
            return null;
        }
    }
}