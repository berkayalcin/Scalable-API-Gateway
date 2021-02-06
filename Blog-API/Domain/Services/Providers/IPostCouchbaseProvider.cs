using Couchbase.Extensions.DependencyInjection;

namespace Blog.API.Domain.Services.Providers
{
    public interface IPostCouchbaseProvider : INamedBucketProvider
    {
    }
}