using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CodePulse.API.Repositories.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CodePulseDbContext codePulseDbContext;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(CodePulseDbContext dbContext, ILogger<CategoryRepository> logger)
        {
            this.codePulseDbContext = dbContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("📦 [Repository] GetAllCategoriesAsync invoked.");
            var result = await codePulseDbContext.Category.ToListAsync();
            _logger.LogInformation("📦 [Repository] Retrieved {Count} categories.", result.Count);
            return result;
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            _logger.LogInformation("📦 [Repository] GetCategoryByIdAsync invoked for Id: {Id}", id);

            var existingCategory = await codePulseDbContext.Category.FirstOrDefaultAsync(x => x.Id == id);

            if (existingCategory == null)
            {
                _logger.LogWarning("📦 [Repository] No category found with Id: {Id}", id);
                return null;
            }

            _logger.LogInformation("📦 [Repository] Found category: {Category}", JsonSerializer.Serialize(existingCategory));
            return existingCategory;
        }
        public async Task<Category> AddCategoryAsync(Category category)
        {
            _logger.LogInformation("📥 [AddCategoryAsync] Adding new category: {Category}", JsonSerializer.Serialize(category));
            await codePulseDbContext.Category.AddAsync(category);
            await codePulseDbContext.SaveChangesAsync();
            _logger.LogInformation("✅ [AddCategoryAsync] Successfully added category with ID: {Id}", category.Id);
            return category;
        }

        public async Task<Category?> DeleteCategoryAsync(Guid id)
        {
            _logger.LogInformation("🗑️ [DeleteCategoryAsync] Attempting to delete category with ID: {Id}", id);

            var existingCategory = await codePulseDbContext.Category.FirstOrDefaultAsync(x => x.Id == id);
            if (existingCategory is null)
            {
                _logger.LogWarning("⚠️ [DeleteCategoryAsync] Category with ID {Id} not found", id);
                return null;
            }

            codePulseDbContext.Category.Remove(existingCategory);
            await codePulseDbContext.SaveChangesAsync();

            _logger.LogInformation("✅ [DeleteCategoryAsync] Successfully deleted category with ID: {Id}", id);
            return existingCategory;
        }

        public async Task<Category?> UpdateCategoryAsync(Category category)
        {
            _logger.LogInformation("✏️ [UpdateCategoryAsync] Updating category with ID: {Id}", category.Id);

            var existingCategory = await codePulseDbContext.Category.FirstOrDefaultAsync(x => x.Id == category.Id);
            if (existingCategory is null)
            {
                _logger.LogWarning("⚠️ [UpdateCategoryAsync] Category with ID {Id} not found", category.Id);
                return null;
            }

            codePulseDbContext.Entry(existingCategory).CurrentValues.SetValues(category);
            await codePulseDbContext.SaveChangesAsync();

            _logger.LogInformation("✅ [UpdateCategoryAsync] Successfully updated category with ID: {Id}", category.Id);
            return existingCategory;
        }

    }
}
