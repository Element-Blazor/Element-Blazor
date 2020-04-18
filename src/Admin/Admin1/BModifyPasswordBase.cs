using Blazui.Component;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Admin
{
    public class BModifyPasswordBase : BAdminPageBase
    {
        protected BForm form;


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
                _ = DialogService.CloseDialogAsync(this, info);
                return;
            }
            Toast(result);
        }
    }
}
