using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Element;

namespace Element.Admin
{
    public class BUserManagementBase : BAdminPageBase
    {
        protected List<UserModel> Users { get; private set; } = new List<UserModel>();
        internal bool CanCreate { get; private set; }
        internal bool CanUpdate { get; private set; }
        internal bool CanDelete { get; private set; }
        internal bool CanReset { get; private set; }

        protected BTable table;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            CanCreate = IsCanAccessAny(AdminResources.CreateUser.ToString());
            CanUpdate = IsCanAccessAny(AdminResources.UpdateUser.ToString());
            CanDelete = IsCanAccessAny(AdminResources.DeleteUser.ToString());
            CanReset = IsCanAccessAny(AdminResources.ResetPassword.ToString());
        }


        public async Task CreateUserAsync()
        {
            await DialogService.ShowDialogAsync<BUserEdit>("创建用户", 800, new Dictionary<string, object>());
            await RefreshUsersAsync();
        }

        private async Task RefreshUsersAsync()
        {
            if (table == null)
            {
                return;
            }
            Users = await UserService.GetUsersAsync();
            table.MarkAsRequireRender();
            RequireRender = true;
            StateHasChanged();
        }

        public async Task EditAsync(object user)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(nameof(BUserEdit.EditingUser), user);
            await DialogService.ShowDialogAsync<BUserEdit>("编辑用户", 800, parameters);
            await RefreshUsersAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender)
            {
                return;
            }
            await RefreshUsersAsync();
        }

        public async Task Del(object user)
        {
            var confirm = await ConfirmAsync("确认删除该用户？");
            if (confirm != MessageBoxResult.Ok)
            {
                return;
            }
            var result = await UserService.DeleteUsersAsync(((UserModel)user).Id);
            if (string.IsNullOrWhiteSpace(result))
            {
                return;
            }
            await RefreshUsersAsync();
        }

        public async Task Reset(object user)
        {
            var confirm = await ConfirmAsync("确认将该用户的密码重置为 12345678 吗？");
            if (confirm != MessageBoxResult.Ok)
            {
                return;
            }
            var error = await UserService.ResetPasswordAsync(((UserModel)user).Id, "12345678");
            if (string.IsNullOrWhiteSpace(error))
            {
                return;
            }
            Toast(error);
        }
    }
}
