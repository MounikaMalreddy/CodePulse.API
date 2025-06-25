using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Repositories.Implementation
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly CodePulseDbContext codePulseDbContext;
        private readonly ILogger<BlogPostRepository> _logger;

        public BlogPostRepository(CodePulseDbContext codePulseDbContext, ILogger<BlogPostRepository> logger)
        {
            this.codePulseDbContext = codePulseDbContext;
            this._logger = logger;
        }

        public async Task<BlogPost> AddBlogPostAsync(BlogPost blogPost)
        {
            _logger.LogInformation("AddBlogPostAsync invoked with data: {@blogPost}", blogPost);
            await codePulseDbContext.BlogPost.AddAsync(blogPost);
            await codePulseDbContext.SaveChangesAsync();
            _logger.LogInformation("BlogPost added successfully with ID: {Id}", blogPost.Id);
            return blogPost;
        }

        public async Task<BlogPost?> DeleteBlogPostAsync(Guid id)
        {
            _logger.LogInformation("DeleteBlogPostAsync invoked with ID: {Id}", id);
            var existingBlogPost = await codePulseDbContext.BlogPost.Include(x=>x.Categories).FirstOrDefaultAsync(x => x.Id == id);
            if (existingBlogPost is null)
            {
                _logger.LogWarning("No BlogPost found with ID: {Id}", id);
                return null;
            }
            codePulseDbContext.BlogPost.Remove(existingBlogPost);
            await codePulseDbContext.SaveChangesAsync();
            _logger.LogInformation("BlogPost deleted successfully with ID: {Id}", id);
            return existingBlogPost;
        }

        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync()
        {
            _logger.LogInformation("GetAllBlogPostsAsync invoked.");
            var posts = await codePulseDbContext.BlogPost.Include(x=>x.Categories).ToListAsync();
            _logger.LogInformation("Retrieved {Count} blog posts.", posts.Count);
            return posts;
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(Guid id)
        {
            _logger.LogInformation("GetBlogPostByIdAsync invoked with ID: {Id}", id);
            var post = await codePulseDbContext.BlogPost.Include(x=>x.Categories).FirstOrDefaultAsync(x => x.Id == id);
            if (post == null)
                _logger.LogWarning("BlogPost not found for ID: {Id}", id);
            return post;
        }

        public async Task<BlogPost?> UpdateBlogPostAsync(BlogPost blogPost)
        {
            _logger.LogInformation("UpdateBlogPostAsync invoked with data: {@blogPost}", blogPost);
            var existingBlogPost = await codePulseDbContext.BlogPost.Include(x=>x.Categories).FirstOrDefaultAsync(x => x.Id == blogPost.Id);
            if (existingBlogPost is null)
            {
                _logger.LogWarning("No BlogPost found with ID: {Id}", blogPost.Id);
                return null;
            }
            existingBlogPost.Categories = blogPost.Categories;
            codePulseDbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);
            await codePulseDbContext.SaveChangesAsync();
            _logger.LogInformation("BlogPost updated successfully with ID: {Id}", blogPost.Id);
            return existingBlogPost;
        }
    }
}
