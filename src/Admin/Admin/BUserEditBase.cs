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
        internal BTransfer btransfer;
        [Parameter]
        public UserModel EditingUser { get; set; }

        internal List<TransferItem> RoleItems { get; set; }
        [Parameter]
        public DialogOption Dialog { get; set; }
        private bool isCreate = false;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            isCreate = EditingUser == null;
           
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            RoleItems = (await UserService.GetRolesAsync()).Select(x => new TransferItem()
            {
                Id = x.Id,
                Label = x.Name
            }).ToList();
            form.Reset();
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

    }
}
