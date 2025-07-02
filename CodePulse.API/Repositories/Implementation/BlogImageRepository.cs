using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Repositories.Implementation
{
    public class BlogImageRepository : IBlogImageRepository
    {
        private readonly CodePulseDbContext codePulseDbContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public BlogImageRepository(CodePulseDbContext codePulseDbContext, IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            this.codePulseDbContext = codePulseDbContext;
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<BlogImage>> GetBlogImagesAsync()
        {
            return await codePulseDbContext.BlogImage.ToListAsync();
        }

        public async Task<BlogImage> UploadBlogImageAsync(IFormFile file, BlogImage blogImage)
        {
            //1. Upload the Image to API to Images Folder

            var localPath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{blogImage.FileName}{blogImage.FileExtension}");
            using var stream = new FileStream(localPath, FileMode.Create);
            await file.CopyToAsync(stream);

            //2. Update the Database
            var httpRequest = httpContextAccessor.HttpContext.Request;
            var imageUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/Images/{blogImage.FileName}{blogImage.FileExtension}";
            blogImage.Url = imageUrl;
            await codePulseDbContext.BlogImage.AddAsync(blogImage);
            await codePulseDbContext.SaveChangesAsync();
            return blogImage;
        }
    }
}
