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

        public BlogPostController(IBlogPostRepository blogPostRepository, IMapper mapper)
        {
            this.blogPostRepository = blogPostRepository;
            this._mapper = mapper;
        }

        [HttpGet("GetAllBlogPosts")]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            var blogPostDomain = await blogPostRepository.GetAllBlogPostsAsync();
            if (blogPostDomain is null || !blogPostDomain.Any())
                return NotFound("No blog posts found.");
            return Ok(_mapper.Map<IEnumerable<BlogPostDto>>(blogPostDomain));
        }
        [HttpGet("GetBlogPostById/{id}")]
        public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id)
        {
            var blogPostDomain = await blogPostRepository.GetBlogPostByIdAsync(id);
            if (blogPostDomain is null)
                return NotFound($"Blog post with ID {id} not found.");
            return Ok(_mapper.Map<BlogPostDto>(blogPostDomain));
        }
        [HttpPost("AddBlogPost")]
        public async Task<IActionResult> AddBlogPost([FromBody] AddBlogPostRequestDto request)
        {
            if (request is null)
                return BadRequest("Blog post cannot be null.");
            var blogPostDomain = _mapper.Map<BlogPost>(request);
            var addedBlogPost = await blogPostRepository.AddBlogPostAsync(blogPostDomain);
            var blogPostDto = _mapper.Map<BlogPostDto>(addedBlogPost);
            return CreatedAtAction(nameof(GetBlogPostById), new { id = blogPostDto.Id }, blogPostDto);
        }
        [HttpPut("UpdateBlogPostById/{id}")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, [FromBody] AddBlogPostRequestDto request)
        {
            if (request is null || id == Guid.Empty)
                return BadRequest("Blog post request and ID cannot be null.");
            var blogPostDomain = _mapper.Map<BlogPost>(request);
            blogPostDomain.Id = id;
            var updatedBlogPost = await blogPostRepository.UpdateBlogPostAsync(blogPostDomain);
            if (updatedBlogPost is null)
                return NotFound($"Blog post with ID {id} not found.");
            return Ok(_mapper.Map<BlogPostDto>(updatedBlogPost));
        }
        [HttpDelete("DeleteBlogPostById/{id}")]
        public async Task<IActionResult> DeleteBlogPostById([FromRoute] Guid id)
        {
            var deletedBlogPost = await blogPostRepository.DeleteBlogPostAsync(id);
            if (deletedBlogPost is null)
                return NotFound($"Blog post with ID {id} not found.");
            return Ok(_mapper.Map<BlogPostDto>(deletedBlogPost));
        }
    }
}
