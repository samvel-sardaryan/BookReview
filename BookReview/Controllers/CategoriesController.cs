using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public async Task<IActionResult> GetCategories()
        {
            var categories = (await _categoryRepository.GetCategoriesAsync()).Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCategory(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryAsync(categoryId);
            if (category == null)
                return NotFound("Category not found");
            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(categoryDto);
        }

        [HttpGet("{categoryId}/books")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetBooksByCategory(int categoryId)
        {
            if (!await _categoryRepository.CategoryExistsAsync(categoryId))
                return NotFound("Category not found");
            var books = (await _categoryRepository.GetBooksByCategoryAsync(categoryId)).Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                ReleaseDate = b.ReleaseDate
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(books);
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoryDto updateCategory)
        {
            if (updateCategory == null || categoryId != updateCategory.Id)
                return BadRequest("Invalid data");
            var categoryToUpdate = await _categoryRepository.GetCategoryAsync(categoryId);
            if (categoryToUpdate == null)
                return BadRequest("Category not found");
            categoryToUpdate.Name = updateCategory.Name;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _categoryRepository.UpdateCategoryAsync(categoryToUpdate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto newCategory)
        {
            if (newCategory == null)
                return BadRequest("Invalid data");
            var existingCategory = (await _categoryRepository.GetCategoriesAsync()).FirstOrDefault(c => c.Name.Trim().ToUpper() == newCategory.Name.Trim().ToUpper());
            if (existingCategory != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }
            var categoryToCreate = new Category
            {
                Name = newCategory.Name,
            };
            if (!await _categoryRepository.CreateCategoryAsync(categoryToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var categoryToDelete = await _categoryRepository.GetCategoryAsync(categoryId);
            if (categoryToDelete == null)
                return BadRequest("Category not found");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _categoryRepository.DeleteCategoryAsync(categoryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
