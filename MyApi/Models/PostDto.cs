using AutoMapper;
using Entities.Post;
using WebFramework.Api;

namespace MyApi.Models
{
    public class PostDto : BaseDto<PostDto, Post>
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string Address { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
    }

    public class PostSelectDto : BaseDto<PostSelectDto, Post>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Address { get; set; }
        public string ShortDescription { get; set; }
        public string CategoryName { get; set; } //=> Category.Name
        public string UserFullName { get; set; } //=> User.FullName
        public string FullTitle { get; set; } // => mapped from "Title (Category.Name)"

        public override void CustomMappings(IMappingExpression<Post, PostSelectDto> mappingExpression)
        {
            mappingExpression.ForMember(
                    dest => dest.FullTitle,
                    config => config.MapFrom(src => $"{src.Title} ({src.Category.Name})"));
        }
    }
}
