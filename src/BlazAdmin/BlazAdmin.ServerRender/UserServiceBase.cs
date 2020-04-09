using BlazAdmin.Abstract;
using Blazui.Component.Form;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace BlazAdmin.ServerRender
{
    public abstract class UserServiceBase<TUser, TRole> : IUserService
        where TUser : IdentityUser
        where TRole : IdentityRole
    {
        protected readonly SignInManager<TUser> SignInManager;
        protected readonly RoleManager<TRole> RoleManager;

        protected string GetResultMessage(IdentityResult identity)
        {
            if (identity.Succeeded)
            {
                return string.Empty;
            }
            foreach (var item in identity.Errors)
            {
                return item.Description;
            }
            return string.Empty;
        }
        public UserServiceBase(SignInManager<TUser> signInManager, RoleManager<TRole> roleManager)
        {
            SignInManager = signInManager;
            RoleManager = roleManager;
        }


        public async Task<bool> HasUserAsync()
        {
            return await SignInManager.UserManager.Users.AnyAsync();
        }

        public async Task<TUser> FindUserByNameAsync(string username)
        {
            return await SignInManager.UserManager.FindByNameAsync(username);
        }

        public async Task<string> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            var user = await FindUserByNameAsync(username);
            if (user == null)
            {
                return "当前用户名不存在";
            }
            var result = await SignInManager.UserManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    if (item.Code == "PasswordMismatch")
                    {
                        return "旧密码错误";
                    }
                    return item.Description;
                }
            }
            return null;
        }

        public abstract Task<string> CreateUserAsync(string username, string password);

        public abstract Task<string> CreateRoleAsync(string roleName, string id);

        public async Task<string> AddToRoleAsync(string username, params string[] roles)
        {
            var user = await FindUserByNameAsync(username);
            var result = await SignInManager.UserManager.AddToRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    return item.Description;
                }
            }
            return string.Empty;
        }

        public async Task<string> CheckPasswordAsync(string username, string password)
        {
            var user = await FindUserByNameAsync(username);
            if (user == null)
            {
                return "用户名或密码错误";
            }
            var result = await SignInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return "用户名或密码错误";
            }
            return string.Empty;
        }

        public async Task<List<object>> GetUsersAsync()
        {
            return (await SignInManager.UserManager.Users.ToListAsync()).Cast<object>().ToList();
        }

        public async Task<string> CreateSuperUserAsync(string username, string password)
        {
            string err = string.Empty;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                err = await CreateUserAsync(username, password);
                if (!string.IsNullOrWhiteSpace(err))
                {
                    return err;
                }
                err = await CreateRoleAsync("管理员", "admin");
                if (!string.IsNullOrWhiteSpace(err))
                {
                    return err;
                }
                err = await AddToRoleAsync(username, "管理员");
                if (!string.IsNullOrWhiteSpace(err))
                {
                    return err;
                }
                scope.Complete();
            }
            return err;
        }

        public async ValueTask<string> LoginAsync(BForm form, string username, string password, string callback)
        {
            if (form != null)
            {
                await form.SubmitAsync("/api/login?callback=" + callback);
                return string.Empty;
            }
            var identityUser = await FindUserByNameAsync(username);
            if (identityUser == null)
            {
                return "用户名或密码错误，登录失败";
            }
            var result = await SignInManager.PasswordSignInAsync(identityUser, password, false, false);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return "当前用户被锁定";
                }
                if (result.IsNotAllowed)
                {
                    return "当前用户不允许登录";
                }
                if (result.RequiresTwoFactor)
                {
                    return "当前用户需要两步验证";
                }
            }
            return string.Empty;
        }

        public async ValueTask<string> LogoutAsync(BForm form, string callback)
        {
            if (form != null)
            {
                await form.SubmitAsync("/api/logout?callback" + callback);
                return string.Empty;
            }
            await SignInManager.SignOutAsync();
            return string.Empty;
        }


        public abstract Task<string> DeleteUsersAsync(params object[] users);

        public async ValueTask SubmitLogoutAsync(BForm form, string callbackUri)
        {
            await form.SubmitAsync("/api/logout?callback=" + callbackUri);
        }
        public async ValueTask SubmitLoginAsync(BForm form, string callbackUri)
        {
            await form.SubmitAsync("/api/login?callback=" + callbackUri);
        }

        public async ValueTask<bool> IsRequireInitilizeAsync()
        {
            return (!await SignInManager.UserManager.Users.AnyAsync());
        }

        public async ValueTask<string> ExecuteLoginAsync(Func<ValueTask<string>> action)
        {
            return await action();
        }
    }
}
