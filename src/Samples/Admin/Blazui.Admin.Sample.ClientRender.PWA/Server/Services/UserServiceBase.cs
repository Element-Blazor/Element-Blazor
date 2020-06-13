using Blazui.Admin;
using Blazui.Admin.Abstract;
using Blazui.Admin.Sample.ClientRender.PWA.Shared.IServices;
using Blazui.Component;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Blazui.Admin.Sample.ClientRender.PWA.Server.Services
{
    public abstract class UserServiceBase<TUser, TRole> : IUserServiceExtension
           where TUser : IdentityUser
           where TRole : IdentityRole
    {
        protected readonly SignInManager<TUser> SignInManager;
        protected readonly RoleManager<TRole> RoleManager;
        protected DbContext DbContext { get; }

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
        public UserServiceBase(SignInManager<TUser> signInManager, RoleManager<TRole> roleManager, DbContext dbContext)
        {
            SignInManager = signInManager;
            RoleManager = roleManager;
            DbContext = dbContext;
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

        public abstract Task<string> CreateUserAsync(UserModel user);

        public abstract Task<string> CreateRoleAsync(RoleModel role);

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

        public async Task<List<UserModel>> GetUsersAsync()
        {
            return (await Task.WhenAll((await SignInManager.UserManager.Users.ToListAsync()).Select(async x =>
            {
                var roleNames = await SignInManager.UserManager.GetRolesAsync(x);
                var roles = await Task.WhenAll(roleNames.Select(name => RoleManager.FindByNameAsync(name)));
                var user = new UserModel()
                {
                    Email = x.Email,
                    Id = x.Id,
                    Username = x.UserName,
                    RoleIds = roles.Select(x => x.Id).ToList()
                };
                return user;
            }))).ToList();
        }

        public async Task<string> CreateSuperUserAsync(string username, string password)
        {
            string err = string.Empty;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                err = await CreateUserAsync(new UserModel()
                {
                    Password = password,
                    Username = username
                });
                if (!string.IsNullOrWhiteSpace(err))
                {
                    return err;
                }
                var roleModel = new RoleModel()
                {
                    Name = "超级管理员"
                };
                err = await CreateRoleAsync(roleModel);
                if (!string.IsNullOrWhiteSpace(err))
                {
                    return err;
                }
                err = await AddToRoleAsync(username, "超级管理员");
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
                return "用户名或密码错误，登录失败";
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


        public abstract Task<string> DeleteUsersAsync(params string[] users);

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


        public abstract Task<string> UpdateUserAsync(UserModel userModel);

        public async Task<string> UpdateRoleAsync(RoleModel roleModel)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                if (role == null)
                {
                    return "当前角色不存在";
                }
                role.Name = roleModel.Name;
                var result = await RoleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    return GetResultMessage(result);
                }
                var roleResources = DbContext.Set<RoleResource>();
                var deletings = roleResources.Where(x => x.RoleId == roleModel.Id);
                roleResources.RemoveRange(deletings);
                roleResources.AddRange(roleModel.Resources.Select(x => new RoleResource()
                {
                    ResourceId = x,
                    RoleId = roleModel.Id
                }));
                await DbContext.SaveChangesAsync();
                scope.Complete();
            }
            return string.Empty;
        }

        public abstract Task<List<RoleModel>> GetRolesAsync();

        public abstract string GetRolesWithResources(params string[] resources);

        public abstract Task<List<RoleResource>> GetResourcesAsync();

        public abstract ValueTask<string> DeleteRolesAsync(params string[] ids);


        public async ValueTask<string> ResetPasswordAsync(string id, string password)
        {
            var user = await SignInManager.UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return "该用户不存在";
            }
            var token = await SignInManager.UserManager.GeneratePasswordResetTokenAsync(user);
            var result = await SignInManager.UserManager.ResetPasswordAsync(user, token, password);
            return GetResultMessage(result);
        }

        public async Task<UserModel> GetUserAsync(string userId)
        {
            var user = await SignInManager.UserManager.FindByIdAsync(userId);
            return new UserModel()
            {
                Email = user.Email,
                Id = user.Id,
                RoleIds = (await SignInManager.UserManager.GetRolesAsync(user)).ToList(),
                Username = user.UserName
            };
        }

        public async Task<List<RoleModel>> GetRolesAsync(string userId)
        {
            var user = await SignInManager.UserManager.FindByIdAsync(userId);
            var roleNames = await SignInManager.UserManager.GetRolesAsync(user);
            var roles = await RoleManager.Roles.Where(x => roleNames.Contains(x.Name)).ToListAsync();
            return roles.Select(x => new RoleModel()
            {
                Id = x.Id,
                Name = x.Name,
                Resources = DbContext.Set<RoleResource>().Where(x => roles.Select(y => y.Id).Contains(x.RoleId))
                .Select(x => x.ResourceId).ToList()
            }).ToList();
        }
    }
}