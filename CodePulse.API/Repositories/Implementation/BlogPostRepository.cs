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

        public BlogPostRepository(CodePulseDbContext codePulseDbContext)
        {
            this.codePulseDbContext = codePulseDbContext;
        }

        public async Task<BlogPost> AddBlogPostAsync(BlogPost blogPost)
        {
            await codePulseDbContext.BlogPost.AddAsync(blogPost);
            await codePulseDbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> DeleteBlogPostAsync(Guid id)
        {
            var existingBlogPost = await codePulseDbContext.BlogPost.FirstOrDefaultAsync(x => x.Id == id);
            if (existingBlogPost is null)
            {
                return null;
            }
            codePulseDbContext.BlogPost.Remove(existingBlogPost);
            await codePulseDbContext.SaveChangesAsync();
            return existingBlogPost;
        }

        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync()
        {
            return await codePulseDbContext.BlogPost.ToListAsync();
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(Guid id)
        {
            return await codePulseDbContext.BlogPost.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> UpdateBlogPostAsync(BlogPost blogPost)
        {
            var existingBlogPost = await codePulseDbContext.BlogPost.FirstOrDefaultAsync(x => x.Id == blogPost.Id);
            if (existingBlogPost is null)
            {
                return null;
            }
            codePulseDbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);
            await codePulseDbContext.SaveChangesAsync();
            return existingBlogPost;
        }
    }
}
