using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogImageController : ControllerBase
    {
        private readonly IBlogImageRepository blogImageRepository;

        public BlogImageController(IBlogImageRepository blogImageRepository)
        {
            this.blogImageRepository = blogImageRepository;
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file, string fileName, string title)
        {
            ValidateFileUpload(file);
            if (ModelState.IsValid)
            {
                var blogImage = new BlogImage
                {
                    FileName = fileName,
                    Title = title,
                    FileExtension = Path.GetExtension(file.FileName ).ToLower(),
                    DateCreated = DateTime.Now,
                };
                var uploadedImage = await blogImageRepository.UploadBlogImageAsync(file, blogImage);
                return Ok(uploadedImage);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetAllBlogImages")]
        public async Task<IActionResult> GetAllBlogImages()
        {
            var blogImagesDomain = await blogImageRepository.GetBlogImagesAsync();
            if (blogImagesDomain == null)
            {
                return NotFound();
            }
            return Ok(blogImagesDomain);
        }
        private void ValidateFileUpload(IFormFile file) {
            var allowedExtensions = new string[] { ".jpg", ".png", ".jpng" };
            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                ModelState.AddModelError("file","Unsupported file format");
            }
            if (file.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size cannot be more than 10MB");
            }
        }
    }
}
