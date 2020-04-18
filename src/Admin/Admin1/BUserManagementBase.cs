using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component;

namespace Blazui.Admin
{
    public class BUserManagementBase : BAdminPageBase
    {
        protected List<object> Users { get; private set; } = new List<object>();


        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            Users = await UserService.GetUsersAsync();
        }
        public void Edit(object user)
        {

        }
        public async Task Del(object user)
        {
            var result = await UserService.DeleteUsersAsync(user);
            if (string.IsNullOrWhiteSpace(result))
            {
                return;
            }
            Toast(result);
        }
    }
}
