using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Repositories.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CodePulseDbContext codePulseDbContext;

        public CategoryRepository(CodePulseDbContext dbContext)
        {
            this.codePulseDbContext = dbContext;
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            await codePulseDbContext.Category.AddAsync(category);
            await codePulseDbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> DeleteCategoryAsync(Guid id)
        {
            var existingCategory = await codePulseDbContext.Category.FirstOrDefaultAsync(x => x.Id == id);
            if (existingCategory is null)
                return null;
            codePulseDbContext.Category.Remove(existingCategory);
            await codePulseDbContext.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await codePulseDbContext.Category.ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            var existingCategory = await codePulseDbContext.Category.FirstOrDefaultAsync(x => x.Id == id);
            if (existingCategory is null) 
                return null;
            return existingCategory;
        }

        public async Task<Category?> UpdateCategoryAsync(Category category)
        {
            var existingCategory = await codePulseDbContext.Category.FirstOrDefaultAsync(x => x.Id == category.Id);
            if (existingCategory is null)
                return null;
            codePulseDbContext.Entry(existingCategory).CurrentValues.SetValues(category);
            await codePulseDbContext.SaveChangesAsync();
            return existingCategory;
        }
    }
}
