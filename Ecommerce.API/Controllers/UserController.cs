using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Domain.DTOs.UserDTOs;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="user">User registration details.</param>
        /// <returns>Status of registration.</returns>
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(NewUserDTO user)
        {
            try
            {
                var newUser = await _userService.RegisterUser(user);
                return Ok(newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="dto">Login credentials.</param>
        /// <returns>JWT token if login is successful.</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var token = await _userService.Login(dto.Email, dto.Password);

                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Invalid email or password.");

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Changes the authenticated user's password.
        /// </summary>
        /// <param name="dto">Change password data.</param>
        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _userService.ChangePassword(userId, dto);

            if (!result)
                return BadRequest("Password change failed. Please check your current password.");

            return Ok("Password changed successfully.");
        }

        /// <summary>
        /// Resets the authenticated user's password.
        /// </summary>
        /// <param name="dto">Reset password data.</param>
        [Authorize]
        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _userService.ResetPassword(userId, dto);

            if (!result)
                return BadRequest("Password reset failed. Please check your information and try again.");

            return Ok("Password changed successfully.");
        }

        /// <summary>
        /// Updates the authenticated user's profile.
        /// </summary>
        /// <param name="dto">New profile details.</param>
        [Authorize]
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var result = await _userService.UpdateProfile(email, dto.FirstName, dto.LastName, dto.ProfilePictureUrl);

            if (!result)
                return BadRequest("Profile update failed.");

            return Ok("Profile updated successfully.");
        }

        /// <summary>
        /// Sends a confirmation email and marks email as confirmed.
        /// </summary>
        /// <param name="email">Email to confirm.</param>
        /// <returns>Status message of confirmation.</returns>
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email)
        {
            var message = await _userService.SendConfirmationEmail(email);

            if (message.Contains("already confirmed", StringComparison.OrdinalIgnoreCase))
                return BadRequest(message);

            return Ok(message);
        }
    }
}
