using CodePulse.API.Models.Domain;

namespace CodePulse.API.Repositories.Interface
{
    public interface IBlogImageRepository
    {
        Task<BlogImage> UploadBlogImageAsync(IFormFile file, BlogImage blogImage);
    }
}
