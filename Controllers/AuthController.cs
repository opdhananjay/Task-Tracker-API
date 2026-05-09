using devops.Helpers;
using devops.Models;
using devops.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace devops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthRepository authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            this.authRepository = authRepository;
        }

        [HttpGet]
        public IActionResult Crash()
        {
            throw new Exception("Boom! This is a test exception from the controller.");
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registration(Registration registration)
        {
            if (string.IsNullOrEmpty(registration.FirstName))
            {
                return BadRequest(new ApiResponse<object>(false,400,"first name is required"));
            }

            if (string.IsNullOrEmpty(registration.Email))
            {
                return BadRequest(new ApiResponse<object>(false, 400, "email is required"));
            }

            if (string.IsNullOrEmpty(registration.Password))
            {
                return BadRequest(new ApiResponse<object>(false, 400, "password is required"));
            }

            if (string.IsNullOrEmpty(registration.Role))
            {
                return BadRequest(new ApiResponse<object>(false, 400, "role is required"));
            }

            var response = await authRepository.RegisterUserAsync(registration);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login login)
        {
            if(string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest(new ApiResponse<object>(false, 400, "email and password required"));
            }

            var response = await authRepository.LoginAsync(login);
            return StatusCode(response.StatusCode, response);
        }

    }
}
