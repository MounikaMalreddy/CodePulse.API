using AutoMapper;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;

namespace CodePulse.API.Mapper
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AddCategoryRequestDto, Category>().ReverseMap();
            CreateMap<Category,CategoryDto>().ReverseMap();
            CreateMap<BlogPost, BlogPostDto>().ReverseMap();
            CreateMap<BlogPost, AddBlogPostRequestDto>().ReverseMap();
        }
    }
}
