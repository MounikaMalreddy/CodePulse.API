using AutoMapper;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper,
            ILogger<CategoryController> logger)
        {
            this.categoryRepository = categoryRepository;
            this._mapper = mapper;
            this._logger = logger;
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            _logger.LogInformation("➡️ [GetAllCategories] Action Invoked");

            var categoryDomain = await categoryRepository.GetAllCategoriesAsync();

            if (categoryDomain == null || !categoryDomain.Any())
            {
                _logger.LogWarning("⚠️ [GetAllCategories] No categories found.");
                return NotFound("No categories found.");
            }

            _logger.LogInformation("✅ [GetAllCategories] Success. Returning {Count} categories.", categoryDomain.Count());
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categoryDomain));
        }

        [HttpGet("GetCategoryById/{id}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
            _logger.LogInformation("➡️ [GetCategoryById] Invoked with Id: {Id}", id);

            var categoryDomain = await categoryRepository.GetCategoryByIdAsync(id);

            if (categoryDomain is null)
            {
                _logger.LogWarning("⚠️ [GetCategoryById] No category found with Id: {Id}", id);
                return NotFound($"Category with ID {id} not found.");
            }

            _logger.LogInformation("✅ [GetCategoryById] Success. Category: {Category}", JsonSerializer.Serialize(categoryDomain));
            return Ok(_mapper.Map<CategoryDto>(categoryDomain));
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryRequestDto request)
        {
            _logger.LogInformation("📨 [AddCategory] Request received: {Request}", JsonSerializer.Serialize(request));

            if (request is null)
            {
                _logger.LogWarning("❌ [AddCategory] Request body is null");
                return BadRequest("Category cannot be null");
            }

            var categoryDomain = _mapper.Map<Category>(request);
            var addedCategory = await categoryRepository.AddCategoryAsync(categoryDomain);
            var categoryDto = _mapper.Map<CategoryDto>(addedCategory);

            _logger.LogInformation("✅ [AddCategory] Successfully added category: {Response}", JsonSerializer.Serialize(categoryDto));
            return CreatedAtAction(nameof(GetCategoryById), new { id = categoryDto.Id }, categoryDto);
        }

        [HttpPut("UpdateCategoryById/{id}")]
        public async Task<IActionResult> UpdateCategoryById([FromRoute] Guid id, [FromBody] AddCategoryRequestDto request)
        {
            _logger.LogInformation("🔄 [UpdateCategory] Request received for ID: {Id} with data: {Request}", id, JsonSerializer.Serialize(request));

            if (request is null || id == Guid.Empty)
            {
                _logger.LogWarning("❌ [UpdateCategory] Invalid request or ID");
                return BadRequest("CategoryRequest & Id cannot be null");
            }

            var categoryDomain = _mapper.Map<Category>(request);
            categoryDomain.Id = id;

            var updatedCategory = await categoryRepository.UpdateCategoryAsync(categoryDomain);
            if (updatedCategory is null)
            {
                _logger.LogWarning("⚠️ [UpdateCategory] Category with ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("✅ [UpdateCategory] Successfully updated category: {Response}", JsonSerializer.Serialize(updatedCategory));
            return Ok(_mapper.Map<CategoryDto>(updatedCategory));
        }

        [HttpDelete("DeleteCategoryById/{id}")]
        public async Task<IActionResult> DeleteCategoryById([FromRoute] Guid id)
        {
            _logger.LogInformation("🗑️ [DeleteCategory] Request to delete category with ID: {Id}", id);

            if (id == Guid.Empty)
            {
                _logger.LogWarning("❌ [DeleteCategory] ID is empty");
                return BadRequest("Id cannot be null");
            }

            var deletedCategory = await categoryRepository.DeleteCategoryAsync(id);
            if (deletedCategory is null)
            {
                _logger.LogWarning("⚠️ [DeleteCategory] Category with ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("✅ [DeleteCategory] Successfully deleted category: {Response}", JsonSerializer.Serialize(deletedCategory));
            return Ok(_mapper.Map<CategoryDto>(deletedCategory));
        }

    }
}
