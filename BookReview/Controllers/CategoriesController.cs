using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories().Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound("Category not found");
            var category = _categoryRepository.GetCategory(categoryId);
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
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        public IActionResult GetBooksByCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound("Category not found");
            var books = _categoryRepository.GetBooksByCategory(categoryId).Select(b => new BookDto
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
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updateCategory)
        {
            if (updateCategory == null || categoryId != updateCategory.Id)
                return BadRequest("Invalid data");
            if (!_categoryRepository.CategoryExists(categoryId))
                return BadRequest("Category not found");
            var categoryToUpdate = _categoryRepository.GetCategory(categoryId);
            categoryToUpdate.Name = updateCategory.Name;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _categoryRepository.UpdateCategory(categoryToUpdate);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto newCategory)
        {
            if (newCategory == null)
                return BadRequest("Invalid data");
            var existingCategory = _categoryRepository.GetCategories().FirstOrDefault(c => c.Name.Trim().ToUpper() == newCategory.Name.Trim().ToUpper());
            if (existingCategory != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }
            var categoryToCreate = new Category
            {
                Name = newCategory.Name,
            };
            if (!_categoryRepository.CreateCategory(categoryToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return BadRequest("Category not found");
            var categoryToDelete = _categoryRepository.GetCategory(categoryId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
