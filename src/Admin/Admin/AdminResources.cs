using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Element.Admin
{
    [Resources]
    public enum AdminResources
    {
        [Description("更新角色")]
        UpdateRole,

        [Description("删除角色")]
        DeleteRole,

        [Description("更新用户")]
        UpdateUser,

        [Description("删除用户")]
        DeleteUser,

        [Description("创建角色")]
        CreateRole,

        [Description("创建用户")]
        CreateUser,

        [Description("重置密码")]
        ResetPassword,

        [Description("修改密码")]
        ModifyPassword
    }
}
