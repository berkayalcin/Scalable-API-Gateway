using Blog.API.Domain.Models;
using FluentValidation;

namespace Blog.API.Domain.Validators
{
    public class PostValidator : AbstractValidator<PostDto>
    {
        public PostValidator()
        {
            RuleFor(p => p.Name).NotNull().NotEmpty();
            RuleFor(p => p.FullDescription).NotNull().NotEmpty();
            RuleFor(p => p.ShortDescription).NotNull().NotEmpty();
        }
    }
}