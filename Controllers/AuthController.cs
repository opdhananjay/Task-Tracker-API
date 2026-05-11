using devops.Helpers;
using devops.Models;
using devops.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

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

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if(string.IsNullOrEmpty(resetPassword.Email) || string.IsNullOrEmpty(resetPassword.Password))
            {
                return BadRequest(new ApiResponse<object>(false, 400, "email and password is required", null));
            }

            var result = await authRepository.ResetPasswordAsync(resetPassword);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("RemoveUser/{userId}")]
        public async Task<IActionResult> RemoveUser(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new ApiResponse<object>(false, 400, "valid user id is required"));
            }

            var response = await authRepository.RemoveUserAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UpdateUser updateUser)
        {
            if (updateUser.Id <= 0)
            {
                return BadRequest(new ApiResponse<object>(false, 400, "valid user id is required"));
            }

            if (string.IsNullOrEmpty(updateUser.FirstName))
            {
                return BadRequest(new ApiResponse<object>(false, 400, "first name is required"));
            }

            var response = await authRepository.UpdateUserAsync(updateUser);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers(string? OrgId)
        {
            var result = await authRepository.GetUsers(OrgId);
            return StatusCode(result.StatusCode, result);
        }
    }

}
