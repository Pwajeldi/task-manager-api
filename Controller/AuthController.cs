using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_Management_App.DTOs;
using Task_Management_App.Services;

namespace Task_Management_App.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        } //Dependency Injection of the IAuthService via the constructor

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            // Runs the implementes method and returns the generated token after login
            return Ok(new {token});
        }
    }
}
