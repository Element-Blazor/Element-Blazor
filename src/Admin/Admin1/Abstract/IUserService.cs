using Blazui.Component;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Admin.Abstract
{
    public interface IUserService
    {
        Task<string> ChangePasswordAsync(string username, string oldPassword, string newPassword);
        Task<string> CreateUserAsync(string username, string password);
        Task<string> CreateRoleAsync(string roleName, string id);
        Task<List<dynamic>> GetUsersAsync();
        Task<string> AddToRoleAsync(string username, params string[] roles);
        Task<string> DeleteUsersAsync(params object[] users);
        ValueTask<bool> IsRequireInitilizeAsync();

        /// <summary>
        /// 仅检查密码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<string> CheckPasswordAsync(string username, string password);
        Task<string> CreateSuperUserAsync(string username, string password);
        ValueTask<string> LogoutAsync(BForm form, string callback);

        /// <summary>
        /// 检查密码，同时设置登录Cookie
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        ValueTask<string> LoginAsync(BForm form, string username, string password, string callback);
    }
}
