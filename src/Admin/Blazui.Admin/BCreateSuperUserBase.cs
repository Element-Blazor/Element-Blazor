using Blazui.Component;
using Microsoft.AspNetCore.Components;

namespace Blazui.Admin
{
    public class BCreateSuperUserBase : BAdminPageBase
    {
        internal BForm form;
        [Parameter]
        public LoginInfoModel DefaultUser { get; set; }

        protected InputType passwordType = InputType.Password;
        internal void TogglePassword()
        {
            if (passwordType == InputType.Password)
            {
                passwordType = InputType.Text;
            }
            else
            {
                passwordType = InputType.Password;
            }
        }

        public virtual async System.Threading.Tasks.Task CreateAsync()
        {
            if (!form.IsValid())
            {
                return;
            }

            var model = form.GetValue<LoginInfoModel>();
            var result = await UserService.CreateSuperUserAsync(model.Username, model.Password);
            if (string.IsNullOrWhiteSpace(result))
            {
                await UserService.LoginAsync(form, model.Username, model.Password, string.Empty);
                return;
            }
            Toast(result);
        }
    }
}
