using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Element.Admin
{
    public class BAuthorizeView : AuthorizeView
    {
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        protected bool IsAdmin { get; set; }
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (NotAuthorized == null)
            {
                if (string.IsNullOrWhiteSpace(Roles))
                {
                    NotAuthorized = ChildContent;
                }
                else
                {
                    NotAuthorized = provider =>
                    {
                        return builder =>
                        {
                            builder.AddContent(0, "您没有权限访问该页面");
                        };
                    };
                }
            }
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            IsAdmin = state.User.IsInRole("超级管理员");
        }

        protected override IAuthorizeData[] GetAuthorizeData()
        {
            if (IsAdmin)
            {
                return null;
            }
            return base.GetAuthorizeData();
        }
    }
}
