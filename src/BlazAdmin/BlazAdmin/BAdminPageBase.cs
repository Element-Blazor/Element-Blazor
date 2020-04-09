using BlazAdmin.Abstract;
using Blazui.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazAdmin
{
    public class BAdminPageBase : BComponentBase
    {
        [Inject]
        public IUserService UserService { get; set; }

        public string Username { get; private set; }
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState?.User;
            Username = user?.Identity?.Name;
        }
    }
}
