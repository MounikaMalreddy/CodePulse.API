using CodePulse.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Data
{
    public class CodePulseDbContext : DbContext
    {
        public CodePulseDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Category> Category { get; set; }
        public DbSet<BlogPost> BlogPost { get; set; }
        public DbSet<BlogImage> BlogImage { get; set; }
    }
}
