using System;
using System.Threading.Tasks;
using Blog.API.Domain.Models;
using Blog.API.Domain.Services.Post;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Blog.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostService _postService;

        public PostController(ILogger<PostController> logger, IPostService postService)
        {
            _logger = logger;
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string query)
        {
            var posts = await _postService.GetAll(query);
            return Ok(posts);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostDto post)
        {
            _logger.LogInformation("Request Arrived");
            post = await _postService.Create(post);
            return Created("api/Post", post);
        }
    }
}