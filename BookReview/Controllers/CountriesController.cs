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
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public async Task<IActionResult> GetCountries()
        {
            var countries = (await _countryRepository.GetCountriesAsync()).Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name
            });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCountry(int countryId)
        {
            var country = await _countryRepository.GetCountryAsync(countryId);
            if (country == null)
                return NotFound("Country not found");
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
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCountryByAuthor(int authorId)
        {
            var country = await _countryRepository.GetCountryByAuthorAsync(authorId);
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
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Author>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAuthorsFromCountry(int countryId)
        {
            if (!await _countryRepository.CountryExistsAsync(countryId))
                return NotFound("Country not found");
            var authors = (await _countryRepository.GetAuthorsFromCountryAsync(countryId)).Select(a => new AuthorDto
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
        public async Task<IActionResult> UpdateCountry(int countryId, [FromBody] CountryDto updateCountry)
        {
            if (updateCountry == null || countryId != updateCountry.Id)
                return BadRequest("Invalid data");
            var countryToUpdate = await _countryRepository.GetCountryAsync(countryId);
            if (countryToUpdate == null)
                return BadRequest("Country not found");
            countryToUpdate.Name = updateCountry.Name;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _countryRepository.UpdateCountryAsync(countryToUpdate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCountry([FromBody] CountryDto newCountry)
        {
            if (newCountry == null)
                return BadRequest("Invalid data");
            var existingCountry = (await _countryRepository.GetCountriesAsync()).FirstOrDefault(c => c.Name.Trim().ToUpper() == newCountry.Name.Trim().ToUpper());
            if (existingCountry != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }
            var countryToCreate = new Country
            {
                Name = newCountry.Name,
            };
            if (!await _countryRepository.CreateCountryAsync(countryToCreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteCountry(int countryId)
        {
            var countryToDelete = await _countryRepository.GetCountryAsync(countryId);
            if (countryToDelete == null)
                return BadRequest("Country not found");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _countryRepository.DeleteCountryAsync(countryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
