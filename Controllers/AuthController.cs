using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.DTOs;
using TaskManagement.Services;
using TaskManagement.Validators;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly CreateUserDtoValidator _createUserValidator;
        private readonly LoginDtoValidator _loginValidator;

        public AuthController(IUserService userService, CreateUserDtoValidator createUserValidator, LoginDtoValidator loginValidator)
        {
            _userService = userService;
            _createUserValidator = createUserValidator;
            _loginValidator = loginValidator;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto createUserDto)
        {
            var validationResult = await _createUserValidator.ValidateAsync(createUserDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var user = await _userService.CreateAsync(createUserDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Login user and get JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var authResponse = await _userService.AuthenticateAsync(loginDto);
            if (authResponse == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(authResponse);
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }

        /// <summary>
        /// Update current user information
        /// </summary>
        [HttpPut("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> UpdateCurrentUser([FromBody] UpdateUserDto updateUserDto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userService.UpdateAsync(userId, updateUserDto);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var success = await _userService.ChangePasswordAsync(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!success)
            {
                return BadRequest("Current password is incorrect");
            }
            return Ok("Password changed successfully");
        }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
