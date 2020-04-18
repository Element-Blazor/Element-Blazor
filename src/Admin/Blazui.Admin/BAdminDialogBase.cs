using Blazui.Admin.Abstract;
using Blazui.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Admin
{
    public class BAdminDialogBase : BComponentBase
    {
        /// <summary>
        /// 可用于操作当前窗口
        /// </summary>
        [Parameter]
        public DialogOption Dialog { get; set; }

        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        /// <param name="result">窗口返回值，该值将作为 <seealso cref="DialogResult"/> 的 <seealso cref="DialogResult.Result"/> 属性</param>
        /// <returns></returns>
        public Task CloseAsync<T>(T result)
        {
            return Dialog.CloseDialogAsync(result);
        }

        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        /// <returns></returns>
        public Task CloseAsync()
        {
            return Dialog.CloseDialogAsync();
        }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        protected bool IsAdmin { get; set; }
        [Inject]
        public IUserService UserService { get; set; }

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
