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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this._mapper = mapper;
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categoryDomain = await categoryRepository.GetAllCategoriesAsync();
            if (categoryDomain is null)
                return NotFound();
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categoryDomain));
        }
        [HttpGet("GetCategoryById/{id}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
            var categoryDomain = await categoryRepository.GetCategoryByIdAsync(id);
            if (categoryDomain is null)
                return NotFound();
            return Ok(_mapper.Map<CategoryDto>(categoryDomain));
        }
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryRequestDto request)
        {
            if (request is null)
                return BadRequest("Category cannot be null");
            var categoryDomain = _mapper.Map<Category>(request);
            var addedCategory = await categoryRepository.AddCategoryAsync(categoryDomain);
            var categoryDto = _mapper.Map<CategoryDto>(addedCategory);
            return CreatedAtAction(nameof(GetCategoryById), new { id = categoryDto.Id }, categoryDto);
        }
        [HttpPut("UpdateCategoryById/{id}")]
        public async Task<IActionResult> UpdateCategoryById([FromRoute] Guid id, [FromBody] AddCategoryRequestDto request)
        {
            if (request is null || id == null)
                return BadRequest("CategoryRequest & Id cannot be null");
            var categoryDomain = _mapper.Map<Category>(request);
            categoryDomain.Id = id;
            var updatedCategory = await categoryRepository.UpdateCategoryAsync(categoryDomain);
            if (updatedCategory is null)
                return NotFound();
            return Ok(_mapper.Map<CategoryDto>(updatedCategory));
        }
        [HttpDelete("DeleteCategoryById/{id}")]
        public async Task<IActionResult> DeleteCategoryById([FromRoute] Guid id)
        {
            if (id == null)
                return BadRequest("Id cannot be null");
            var deletedCategory = await categoryRepository.DeleteCategoryAsync(id);
            if (deletedCategory is null)
                return NotFound();
            return Ok(_mapper.Map<CategoryDto>(deletedCategory));
        }
    }
}
