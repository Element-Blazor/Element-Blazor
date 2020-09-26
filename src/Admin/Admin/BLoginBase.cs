using Element;
using Microsoft.AspNetCore.Components;

namespace Element.Admin
{
    public class BLoginBase : BAdminPageBase
    {
        public BForm Form { get; internal set; }
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

        public virtual async System.Threading.Tasks.Task LoginAsync()
        {
            if (!Form.IsValid())
            {
                return;
            }

            var model = Form.GetValue<LoginInfoModel>();
            var result = await UserService.CheckPasswordAsync(model.Username, model.Password);
            if (string.IsNullOrWhiteSpace(result))
            {
                await UserService.LoginAsync(Form, model.Username, model.Password, NavigationManager.Uri);
                return;
            }
            Toast(result);
        }
    }
}
