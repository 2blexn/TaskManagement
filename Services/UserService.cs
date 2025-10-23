using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Repositories;
using TaskManagement.Services;

namespace TaskManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
        {
            // Check if username or email already exists
            if (await _userRepository.UsernameExistsAsync(createUserDto.Username))
                throw new InvalidOperationException("Username already exists");

            if (await _userRepository.EmailExistsAsync(createUserDto.Email))
                throw new InvalidOperationException("Email already exists");

            var user = _mapper.Map<User>(createUserDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            user.CreatedAt = DateTime.UtcNow;

            var createdUser = await _userRepository.AddAsync(user);
            return _mapper.Map<UserDto>(createdUser);
        }

        public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            // Check if email is being changed and if it already exists
            if (!string.IsNullOrEmpty(updateUserDto.Email) && 
                updateUserDto.Email != user.Email && 
                await _userRepository.EmailExistsAsync(updateUserDto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            _mapper.Map(updateUserDto, user);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepository.DeleteAsync(id);
            return true;
        }

        public async Task<AuthResponseDto?> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null;

            var token = GenerateJwtToken(user);
            var userDto = _mapper.Map<UserDto>(user);

            return new AuthResponseDto
            {
                Token = token,
                User = userDto,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
            var issuer = jwtSettings["Issuer"] ?? "TaskManagement";
            var audience = jwtSettings["Audience"] ?? "TaskManagementUsers";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FirstName", user.FirstName ?? ""),
                new Claim("LastName", user.LastName ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
