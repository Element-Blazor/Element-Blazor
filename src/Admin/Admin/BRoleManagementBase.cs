using Blazui.Component;
using Blazui.Component;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Admin
{
    public class BRoleManagementBase : BAdminPageBase
    {
        protected List<RoleModel> RoleModels { get; private set; } = new List<RoleModel>();
        internal bool CanCreate { get; private set; }
        internal bool CanUpdate { get; private set; }
        internal bool CanDelete { get; private set; }

        protected BTable table;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            CanCreate = IsCanAccessAny(AdminResources.CreateRole.ToString());
            CanUpdate = IsCanAccessAny(AdminResources.UpdateRole.ToString());
            CanDelete = IsCanAccessAny(AdminResources.DeleteRole.ToString());
        }

        public async Task CreateRoleAsync()
        {
            await DialogService.ShowDialogAsync<BRoleEdit>("创建角色", 800, new Dictionary<string, object>());
            await RefreshRolesAsync();
        }

        private async Task RefreshRolesAsync()
        {
            if (table == null)
            {
                return;
            }
            RoleModels =await UserService.GetRolesAsync();
            table.MarkAsRequireRender();
            RequireRender = true;
            StateHasChanged();
        }

        public async Task EditAsync(object role)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(nameof(BRoleEdit.Role), role);
            await DialogService.ShowDialogAsync<BRoleEdit>("编辑角色", 800, parameters);
            await RefreshRolesAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender)
            {
                return;
            }
            await RefreshRolesAsync();
        }

        public async Task Del(object role)
        {
            var confirm = await ConfirmAsync("将删除该角色下所有用户，确认删除该角色？");
            if (confirm != MessageBoxResult.Ok)
            {
                return;
            }
            var result = await UserService.DeleteRolesAsync(((RoleModel)role).Id);
            if (string.IsNullOrWhiteSpace(result))
            {
                await RefreshRolesAsync();
                return;
            }
            Toast(result);
        }
    }
}
