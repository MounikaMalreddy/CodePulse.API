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

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(string? filterQuery = null,
            string? sortBy = null,
            string? sortDirection = "asc",
            int? pageNumber = 1, int? pageSize = 100)
        {
            _logger.LogInformation("📦 [Repository] GetAllCategoriesAsync invoked.");
            //quering
            var categories = codePulseDbContext.Category.AsQueryable();
            //filtering
            if (!string.IsNullOrWhiteSpace(filterQuery) && filterQuery != "undefined")
            {
                _logger.LogInformation("📦 [Repository] Applying filter: {FilterQuery}", filterQuery);
                categories = categories.Where(x => x.Name.Contains(filterQuery));
            }
            //sorting
            if (!string.IsNullOrWhiteSpace(sortBy) && sortBy != "undefined")
            {
                if (string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
                {
                    //var isAscending = sortDirection?.ToLower() == "asc" ? true : false;
                    var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;
                    categories = isAsc ? categories.OrderBy(x => x.Name) : categories.OrderByDescending(x => x.Name);
                }
                if (string.Equals(sortBy, "URL", StringComparison.OrdinalIgnoreCase))
                {
                    //var isAscending = sortDirection?.ToLower() == "asc" ? true : false;
                    var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;
                    categories = isAsc ? categories.OrderBy(x => x.UrlHandle) : categories.OrderByDescending(x => x.UrlHandle);
                }
            }
            //pagination
            var skipResults = (pageNumber - 1) * pageSize;
            _logger.LogInformation($"📦 [Repository] Retrieved categories.");
            return await categories.Skip(skipResults ?? 0).Take(pageSize ?? 100).ToListAsync();
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

        public async Task<int> CountCategoriesAsync()
        {
            _logger.LogInformation("CountCategoriesAsync Invoked");
            return await codePulseDbContext.Category.CountAsync();
        }
    }
}
