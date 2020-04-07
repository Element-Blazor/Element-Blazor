using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazAdmin
{
    public class BNotAuthorizedBase : BAdminPageBase
    {
        internal bool? requireInitilize;
        [Parameter]
        public object LoginPage { get; set; }
        [Parameter]
        public object CreatePage { get; set; }

        [Parameter]
        public LoginInfoModel DefaultUser { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender)
            {
                return;
            }
            requireInitilize = await UserService.IsRequireInitilizeAsync();
            RequireRender = true;
            StateHasChanged();
        }
    }
}
