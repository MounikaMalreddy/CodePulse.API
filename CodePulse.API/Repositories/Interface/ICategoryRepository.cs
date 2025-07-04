using CodePulse.API.Models.Domain;

namespace CodePulse.API.Repositories.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(string? filterQuery = null, string? sortBy = null,
            string? sortDirection = null,
            int? pageNumber = 1, int? pageSize = 100);
        Task<Category?> GetCategoryByIdAsync(Guid id);
        Task<Category> AddCategoryAsync(Category category);
        Task<Category?> UpdateCategoryAsync(Category category);
        Task<Category?> DeleteCategoryAsync(Guid id);
        Task<int> CountCategoriesAsync();
    }
}
