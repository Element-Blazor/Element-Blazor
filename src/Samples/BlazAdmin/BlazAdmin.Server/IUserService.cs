using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazAdmin.Server
{
    public interface IUserService
    {
        Task<string> AddToRoleAsync(string username, params string[] roles);
        Task<string> ChangePasswordAsync(string username, string oldPassword, string newPassword);
        Task<string> CheckPasswordAsync(string username, string password);
        Task<string> CreateRoleAsync(string roleName, string id);
        Task<string> CreateSuperUserAsync(string username, string password);
        Task<string> CreateUserAsync(string username, string password);
        Task<string> DeleteUserAsync(object user);
        Task<List<object>> GetUsersAsync();
        Task<string> LoginAsync(string username, string password);
        Task<string> LogoutAsync();
        Task<bool> HasUserAsync();
    }
}