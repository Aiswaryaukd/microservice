using Microsoft.AspNetCore.Mvc;
using Product.Api.Models;

namespace Product.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto model)
        {
            if (model == null) return BadRequest();
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Demo: hardcoded validation. Replace with real user store.
            if (model.Email == "admin@example.com" && model.Password == "Password123")
            {
                var user = new UserDto
                {
                    Email = model.Email,
                    Name = "Administrator",
                    Token = "demo-token-abc123"
                };
                return Ok(user);
            }

            return Unauthorized();
        }
    }
}
