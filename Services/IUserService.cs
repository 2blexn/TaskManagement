using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Repositories;

namespace TaskManagement.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<UserDto> CreateAsync(CreateUserDto createUserDto);
        Task<UserDto?> UpdateAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteAsync(int id);
        Task<AuthResponseDto?> AuthenticateAsync(LoginDto loginDto);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
