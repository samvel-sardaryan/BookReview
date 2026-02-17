using BookReview.Dto;
using BookReview.Interfaces;
using BookReview.Models;
using BookReview.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = _countryRepository.GetCountries().Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound("Country not found");
            var country = _countryRepository.GetCountry(countryId);
            var countryDto = new CountryDto
            {
                Id = country.Id,
                Name = country.Name
            };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(countryDto);
        }

        [HttpGet("authors/{authorId}")]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        [ProducesResponseType(400)]
        public IActionResult GetCountryByAuthor(int authorId)
        {
            var country = _countryRepository.GetCountryByAuthor(authorId);
            if (country == null)
                return NotFound("Country not found for the given author");
            var countryDto = new CountryDto
            {
                Id = country.Id,
                Name = country.Name
            };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(countryDto);
        }

        [HttpGet("{countryId}/authors")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Author>))]
        [ProducesResponseType(400)]
        public IActionResult GetAuthorsFromCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound("Country not found");
            var authors = _countryRepository.GetAuthorsFromCountry(countryId).Select(a => new AuthorDto
            {
                Id = a.Id,
                Name = a.Name,
                Bio = a.Bio
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(authors);
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto updateCountry)
        {
            if (updateCountry == null || countryId != updateCountry.Id)
                return BadRequest("Invalid data");
            if (!_countryRepository.CountryExists(countryId))
                return BadRequest("Country not found");
            var countryToUpdate = _countryRepository.GetCountry(countryId);
            countryToUpdate.Name = updateCountry.Name;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _countryRepository.UpdateCountry(countryToUpdate);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDto newCountry)
        {
            if (newCountry == null)
                return BadRequest("Invalid data");
            var existingCountry = _countryRepository.GetCountries().FirstOrDefault(c => c.Name.Trim().ToUpper() == newCountry.Name.Trim().ToUpper());
            if (existingCountry != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }
            var countryToCreate = new Country
            {
                Name = newCountry.Name,
            };
            if (!_countryRepository.CreateCountry(countryToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return BadRequest("Country not found");
            var countryToDelete = _countryRepository.GetCountry(countryId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
