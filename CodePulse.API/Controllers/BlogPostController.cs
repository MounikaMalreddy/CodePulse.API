using AutoMapper;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogPostController> _logger;
        private readonly ICategoryRepository categoryRepository;

        public BlogPostController(IBlogPostRepository blogPostRepository, IMapper mapper,
            ILogger<BlogPostController> logger, ICategoryRepository categoryRepository)
        {
            this.blogPostRepository = blogPostRepository;
            this._mapper = mapper;
            this._logger = logger;
            this.categoryRepository = categoryRepository;
        }

        [HttpGet("GetAllBlogPosts")]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            _logger.LogInformation("GetAllBlogPosts action invoked.");
            var blogPostDomain = await blogPostRepository.GetAllBlogPostsAsync();
            if (blogPostDomain is null || !blogPostDomain.Any())
            {
                _logger.LogWarning("No blog posts found.");
                return NotFound("No blog posts found.");
            }
            _logger.LogInformation("Returning {Count} blog posts.", blogPostDomain.Count());
            return Ok(_mapper.Map<IEnumerable<BlogPostDto>>(blogPostDomain));
        }

        [HttpGet("GetBlogPostById/{id}")]
        public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id)
        {
            _logger.LogInformation("GetBlogPostById action invoked with ID: {Id}", id);
            var blogPostDomain = await blogPostRepository.GetBlogPostByIdAsync(id);
            if (blogPostDomain is null)
            {
                _logger.LogWarning("Blog post not found with ID: {Id}", id);
                return NotFound($"Blog post with ID {id} not found.");
            }
            _logger.LogInformation("Blog post retrieved: {@BlogPost}", blogPostDomain);
            return Ok(_mapper.Map<BlogPostDto>(blogPostDomain));
        }

        [HttpPost("AddBlogPost")]
        public async Task<IActionResult> AddBlogPost([FromBody] AddBlogPostRequestDto request)
        {
            _logger.LogInformation("AddBlogPost action invoked with request: {@Request}", request);
            if (request is null)
            {
                _logger.LogWarning("Request is null in AddBlogPost.");
                return BadRequest("Blog post cannot be null.");
            }
            var blogPostDomain = _mapper.Map<BlogPost>(request);
            blogPostDomain.Categories = new List<Category>();
            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetCategoryByIdAsync(categoryGuid);
                if (existingCategory is not null)
                {
                    blogPostDomain.Categories.Add(existingCategory);
                }
            }
            var addedBlogPost = await blogPostRepository.AddBlogPostAsync(blogPostDomain);
            var blogPostDto = _mapper.Map<BlogPostDto>(addedBlogPost);
            _logger.LogInformation("Blog post added successfully: {@BlogPost}", blogPostDto);
            return CreatedAtAction(nameof(GetBlogPostById), new { id = blogPostDto.Id }, blogPostDto);
        }

        [HttpPut("UpdateBlogPostById/{id}")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, [FromBody] AddBlogPostRequestDto request)
        {
            _logger.LogInformation("UpdateBlogPostById action invoked for ID: {Id}", id);
            if (request is null || id == Guid.Empty)
            {
                _logger.LogWarning("Request or ID is invalid in UpdateBlogPostById.");
                return BadRequest("Blog post request and ID cannot be null.");
            }

            var blogPostDomain = _mapper.Map<BlogPost>(request);
            blogPostDomain.Categories = new List<Category>();
            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetCategoryByIdAsync(categoryGuid);
                if (existingCategory is not null)
                {
                    blogPostDomain.Categories.Add(existingCategory);
                }
            }
            blogPostDomain.Id = id;
            var updatedBlogPost = await blogPostRepository.UpdateBlogPostAsync(blogPostDomain);
            if (updatedBlogPost is null)
            {
                _logger.LogWarning("Blog post not found with ID: {Id}", id);
                return NotFound($"Blog post with ID {id} not found.");
            }
            _logger.LogInformation("Blog post updated successfully: {@BlogPost}", updatedBlogPost);
            return Ok(_mapper.Map<BlogPostDto>(updatedBlogPost));
        }

        [HttpDelete("DeleteBlogPostById/{id}")]
        public async Task<IActionResult> DeleteBlogPostById([FromRoute] Guid id)
        {
            _logger.LogInformation("DeleteBlogPostById action invoked with ID: {Id}", id);
            var deletedBlogPost = await blogPostRepository.DeleteBlogPostAsync(id);
            if (deletedBlogPost is null)
            {
                _logger.LogWarning("Blog post not found for deletion with ID: {Id}", id);
                return NotFound($"Blog post with ID {id} not found.");
            }
            _logger.LogInformation("Blog post deleted successfully: {@BlogPost}", deletedBlogPost);
            return Ok(_mapper.Map<BlogPostDto>(deletedBlogPost));
        }
        [HttpGet("GetBlogPostByUrlHandle/{urlHandle}")]
        public async Task<IActionResult> GetBlogPostByUrlHandle([FromRoute] string urlHandle)
        {
            _logger.LogInformation("GetBlogsByUrl action invoked with URL: {Url}", urlHandle);
            var blogPostDomain = await blogPostRepository.GetBlogPostByUrlHandleAsync(urlHandle);
            if (blogPostDomain is null)
                return NotFound();
            _logger.LogInformation($"Blog post retrieved: {blogPostDomain}");
            return Ok(_mapper.Map<BlogPostDto>(blogPostDomain));
        }
    }
}
