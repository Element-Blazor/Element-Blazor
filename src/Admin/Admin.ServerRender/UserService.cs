using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Blazui.Admin.ServerRender
{
    public class UserService : UserServiceBase<IdentityUser, IdentityRole>
    {
        public UserService(SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, DbContext dbContext) : base(signInManager, roleManager, dbContext)
        {
        }

        public override async Task<string> CreateRoleAsync(RoleModel role)
        {
            var roleModel = new IdentityRole(role.Name);
            var result = await RoleManager.CreateAsync(roleModel);
            role.Id = roleModel.Id;
            var r = GetResultMessage(result);
            if (!string.IsNullOrWhiteSpace(r))
            {
                return r;
            }

            for (int i = 0; i < role.Resources.Count; i++)
            {
                DbContext.Set<RoleResource>().AddRange(new RoleResource()
                {
                    ResourceId = role.Resources[i],
                    RoleId = roleModel.Id
                });
            }
            DbContext.SaveChanges();
            return string.Empty;
        }

        public override async Task<string> CreateUserAsync(UserModel userModel)
        {
            var user = new IdentityUser(userModel.Username);
            user.Email = userModel.Email; ;
            user.EmailConfirmed = true;
            user.NormalizedEmail = userModel.Email;
            var result = await SignInManager.UserManager.CreateAsync(user, userModel.Password);
            if (!string.IsNullOrWhiteSpace(GetResultMessage(result)))
            {
                return GetResultMessage(result);
            }
            var roles = RoleManager.Roles.Where(x => userModel.Roles.Contains(x.Id)).Select(x => x.Name).ToArray();
            result = await SignInManager.UserManager.AddToRolesAsync(user, roles);
            if (!string.IsNullOrWhiteSpace(GetResultMessage(result)))
            {
                return GetResultMessage(result);
            }
            return string.Empty;
        }

        public override async ValueTask<string> DeleteRolesAsync(params string[] ids)
        {
            foreach (var id in ids)
            {
                var result = await RoleManager.DeleteAsync(await RoleManager.FindByIdAsync(id));
                if (result.Succeeded)
                {
                    continue;
                }
                return GetResultMessage(result);
            }
            return string.Empty;
        }

        public override async Task<string> DeleteUsersAsync(params string[] userIds)
        {
            foreach (var id in userIds)
            {
                var result = await SignInManager.UserManager.DeleteAsync(await SignInManager.UserManager.FindByIdAsync(id));
                if (result.Succeeded)
                {
                    continue;
                }
                return GetResultMessage(result);
            }
            return string.Empty;
        }

        public override List<RoleModel> GetRoles()
        {
            var roles = RoleManager.Roles.Select(x => new RoleModel()
            {
                Name = x.Name,
                Id = x.Id,
            }).ToList();
            var roleIds = roles.Select(x => x.Id).ToArray();
            var resources = DbContext.Set<RoleResource>().Where(x => roleIds.Contains(x.RoleId)).ToArray();
            foreach (var role in roles)
            {
                role.Resources = resources.Where(x => x.RoleId == role.Id).Select(x => x.ResourceId).ToList();
            }
            return roles.ToList();
        }

        public override string GetRolesWithResources(params string[] resources)
        {
            var roleIds = DbContext.Set<RoleResource>().Where(x => resources.Contains(x.ResourceId)).Select(x => x.RoleId).ToArray();
            var roleNames = RoleManager.Roles.Where(x => roleIds.Contains(x.Id)).Select(x => x.Name).ToArray();
            return string.Join(",", roleNames);
        }

        public override async Task<string> UpdateUserAsync(UserModel userModel)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var user = await SignInManager.UserManager.FindByIdAsync(userModel.Id);
                if (user == null)
                {
                    return "当前用户不存在";
                }
                user.UserName = userModel.Username;
                user.Email = userModel.Email;
                var existRoles = await SignInManager.UserManager.GetRolesAsync(user);
                var result = await SignInManager.UserManager.RemoveFromRolesAsync(user, existRoles);
                if (!result.Succeeded)
                {
                    return GetResultMessage(result);
                }
                var newRoles = RoleManager.Roles.Where(x => userModel.Roles.Contains(x.Id)).Select(x => x.Name).ToArray();
                result = await SignInManager.UserManager.AddToRolesAsync(user, newRoles);
                if (!result.Succeeded)
                {
                    return GetResultMessage(result);
                }
                result = await SignInManager.UserManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return GetResultMessage(result);
                }
                scope.Complete();
            }
            return string.Empty;
        }
    }
}
