using Element;
using Element;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Element.Admin
{
    public class BRoleEditBase : BAdminPageBase
    {
        [Inject]
        private ResourceAccessor ResourceAccessor { get; set; }

        internal List<TransferItem> Resources { get; set; }
        internal BForm form;
        [Parameter]
        public RoleModel Role { get; set; }

        [Parameter]
        public DialogOption Dialog { get; set; }
        private bool isCreate = false;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            isCreate = Role == null;
            Resources = ResourceAccessor.Resources.Select(x => new TransferItem()
            {
                Id = x.Key,
                Label = x.Value
            }).ToList();
        }
        public async System.Threading.Tasks.Task SubmitAsync()
        {
            if (!form.IsValid())
            {
                return;
            }

            string error;
            Role = form.GetValue<RoleModel>();
            if (isCreate)
            {
                error = await UserService.CreateRoleAsync(Role);
            }
            else
            {
                error = await UserService.UpdateRoleAsync(Role);
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
