using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Domain.Models;

namespace Blog.API.Domain.Services.Post
{
    public interface IPostService
    {
        Task<List<PostDto>> GetAll();
        Task<PostDto> Create(PostDto postDto);
    }
}