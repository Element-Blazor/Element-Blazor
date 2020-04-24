using Blazui.Admin.Abstract;
using Blazui.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blazui.Admin
{
    public class BAdminPageBase : BComponentBase
    {
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        protected bool IsAdmin { get; set; }
        [Inject]
        public IUserService UserService { get; set; }

        /// <summary>
        /// 当前页面允许访问的角色
        /// </summary>
        protected string Roles { get; private set; }

        /// <summary>
        /// 当前用户
        /// </summary>
        public ClaimsPrincipal User { get; private set; }
        public string Username { get; private set; }
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var resource = GetType().GetCustomAttributes(false)
                .OfType<ResourceAttribute>()
                .FirstOrDefault();
            if (resource != null)
            {
                Roles = UserService.GetRolesWithResources(resource.Id);
                if (!string.IsNullOrWhiteSpace(Roles))
                {
                    Roles += ",超级管理员";
                }
                else
                {
                    Roles = "超级管理员";
                }
            }
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            User = authState?.User;
            IsAdmin = User.IsInRole("超级管理员");
            Username = User?.Identity?.Name;
        }

        /// <summary>
        /// 是否可以访问指定资源之一
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        protected bool IsCanAccessAny(params string[] resources)
        {
            if (User == null)
            {
                return false;
            }
            if (IsAdmin)
            {
                return true;
            }
            var roles = UserService.GetRolesWithResources(resources);
            foreach (var role in roles.Split(','))
            {
                if (User.IsInRole(role))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否可以访问指定资源
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        protected bool IsCanAccessAll(params string[] resources)
        {
            if (User == null)
            {
                return false;
            }
            if (IsAdmin)
            {
                return true;
            }
            var roles = UserService.GetRolesWithResources(resources);
            foreach (var role in roles.Split(','))
            {
                if (!User.IsInRole(role))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
