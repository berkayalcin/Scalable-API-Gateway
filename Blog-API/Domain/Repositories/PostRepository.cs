using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Domain.Entities;
using Blog.API.Domain.Models;
using Blog.API.Domain.Services.Providers;
using Couchbase.Core;

namespace Blog.API.Domain.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IBucket _postsBucket;

        public PostRepository(IPostCouchbaseProvider postCouchbaseProvider)
        {
            _postsBucket = postCouchbaseProvider.GetBucket();
        }

        public async Task<List<Post>> GetAll()
        {
            var pong = _postsBucket.Ping();
            return null;
        }
    }
}