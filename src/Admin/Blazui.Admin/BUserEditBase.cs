using Blazui.Component;
using Blazui.Component;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Admin
{
    public class BUserEditBase : BAdminPageBase
    {
        internal BForm form;
        [Parameter]
        public UserModel EditingUser { get; set; }

        internal List<TransferItem> RoleItems = new List<TransferItem>();
        internal List<TransferItem> ExistsRoleItems = new List<TransferItem>();
        [Parameter]
        public DialogOption Dialog { get; set; }
        private bool isCreate = false;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            isCreate = EditingUser == null;
        }
        public async System.Threading.Tasks.Task SubmitAsync()
        {
            if (!form.IsValid())
            {
                return;
            }

            string error;
            EditingUser = form.GetValue<UserModel>();
            if (string.IsNullOrWhiteSpace(EditingUser.Password))
            {
                EditingUser.Password = "123456";
            }
            if (isCreate)
            {
                error = await UserService.CreateUserAsync(EditingUser);
            }
            else
            {
                error = await UserService.UpdateUserAsync(EditingUser);
            }
            if (!string.IsNullOrWhiteSpace(error))
            {
                Toast(error);
                return;
            }
            _ = DialogService.CloseDialogAsync(this, (object)null);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender)
            {
                return;
            }

            RoleItems = UserService.GetRoles().Select(x => new TransferItem()
            {
                Id = x.Id,
                Label = x.Name
            }).ToList();
            MarkAsRequireRender();
            StateHasChanged();
            //if (EditingUser == null)
            //{
            //    return;
            //}
            //var user = await UserService.GetUserAsync(EditingUser.Id);
            //ExistsRoleItems = (await UserService.GetRolesAsync(user.Id)).Select(x => new TransferItem()
            //{
            //    Id = x.Id,
            //    Label = x.Name
            //}).ToList();
            //StateHasChanged();
        }
    }
}
