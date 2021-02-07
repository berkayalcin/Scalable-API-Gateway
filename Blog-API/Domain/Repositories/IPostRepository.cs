using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Domain.Entities;
using Blog.API.Domain.Models;

namespace Blog.API.Domain.Repositories
{
    public interface IPostRepository
    {
        Task<List<Post>> GetAll();
        Task<Post> Create(Post post);
    }
}