using Blazui.Component;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Blazui.Admin
{
    public class BModifyPasswordBase : BAdminPageBase
    {
        protected BForm form;

        protected bool CanModify { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            CanModify = IsCanAccessAny(AdminResources.ModifyPassword.ToString());
        }

        [Parameter]
        public DialogOption Dialog { get; set; }

        public virtual async System.Threading.Tasks.Task ModifyAsync()
        {
            if (!form.IsValid())
            {
                return;
            }

            var info = form.GetValue<ModifyPasswordModel>();

            var result = await UserService.ChangePasswordAsync(Username, info.OldPassword, info.NewPassword);
            if (string.IsNullOrWhiteSpace(result))
            {
                _ = Dialog.CloseDialogAsync(info);
                return;
            }
            Toast(result);
        }
    }
}
