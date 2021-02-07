using System.Collections.Generic;

namespace Blog.API.Domain.Models
{
    public class ElasticSearchOptions
    {
        public string HostUrls { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}