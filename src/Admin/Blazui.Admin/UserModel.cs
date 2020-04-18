using Blazui.Component;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Admin
{
    public class UserModel
    {
        public string Id { get; set; }
        [TableColumn(Text = "用户名")]
        public string Username { get; set; }

        [TableColumn(Text = "邮箱")]
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        public string Password { get; set; }
    }
}
