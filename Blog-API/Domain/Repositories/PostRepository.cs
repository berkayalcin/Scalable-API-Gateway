using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Domain.Entities;
using Blog.API.Domain.Models;
using Blog.API.Domain.Services.Providers;
using Couchbase.Core;
using Couchbase.IO;

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

        public async Task<Post> Create(Post post)
        {
            var key = Guid.NewGuid();
            post.Id = key;
            post.CreatedAt = DateTime.UtcNow;
            post.IsDeleted = false;

            var result = await _postsBucket.InsertAsync<Post>(key.ToString(), post);
            if (result.Status != ResponseStatus.Success)
            {
                throw new OperationCanceledException(result.Message);
            }

            post.Id = Guid.Parse(result.Id);
            return post;
        }
    }
}