using AutoMapper;
using Blog.API.Domain.Entities;
using Blog.API.Domain.Models;

namespace Blog.API.Domain.Mappers
{
    public class PostMapper : Profile
    {
        public PostMapper()
        {
            CreateMap<Post, PostDto>().ReverseMap();
        }
    }
}