using BookReview.Dto;
using BookReview.Entities;
using BookReview.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user == null)
                return BadRequest("User already exists.");
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var token = await authService.LoginAsync(request);
            if (token == null)
                return BadRequest("Invalid username or password.");
            return Ok(token);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public ActionResult AdminOnlyEndpoint()
        {
            return Ok("Authorized endpoint");
        }
    }
}
