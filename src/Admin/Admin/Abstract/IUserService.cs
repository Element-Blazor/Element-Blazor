using Element;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element.Admin.Abstract
{
    public interface IUserService
    {
        Task<string> ChangePasswordAsync(string username, string oldPassword, string newPassword);
        Task<string> CreateUserAsync(UserModel userModel);
        Task<string> CreateRoleAsync(RoleModel role);
        Task<List<UserModel>> GetUsersAsync();
        Task<UserModel> GetUserAsync(string userId);
        Task<string> UpdateUserAsync(UserModel userModel);
        Task<string> UpdateRoleAsync(RoleModel roleModel);
        Task<string> AddToRoleAsync(string username, params string[] roles);
        List<RoleModel> GetRoles();
        Task<List<RoleModel>> GetRolesAsync(string userId);
        Task<string> DeleteUsersAsync(params string[] userIds);
        ValueTask<bool> IsRequireInitilizeAsync();
        string GetRolesWithResources(params string[] resources);

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
        ValueTask<string> DeleteRolesAsync(params string[] ids);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        ValueTask<string> ResetPasswordAsync(string id, string password);
    }
}
