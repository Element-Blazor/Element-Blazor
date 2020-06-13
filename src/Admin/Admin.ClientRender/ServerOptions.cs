using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Admin.ClientRender
{
    public class ServerOptions
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerUrl { get; set; } = "http://localhost:5050";

        /// <summary>
        /// 创建超级管理员的路由地址
        /// </summary>
        public string CreateSuperUserUrl { get; set; } = "api/createsuperuser";
        public string CreateUserUrl { get; set; } = "api/CreateUser";
        /// <summary>
        /// 系统是否初次使用，需要初始化
        /// </summary>
        public string RequireInitilizeUrl { get; set; } = "api/IsRequireInitilize";

        /// <summary>
        /// 检查密码正确性的路由地址，该地址仅检查密码正确性，不做登录操作
        /// </summary>
        public string CheckPasswordUrl { get; set; } = "api/checkpassword";

        /// <summary>
        /// 发送登录请求的地址
        /// </summary>
        public string LoginUrl { get; set; } = "/api/login";

        /// <summary>
        /// 发送登出请求的地址
        /// </summary>
        public string LogoutUrl { get; set; } = "/api/logout";

        /// <summary>
        /// 修改密码的地址
        /// </summary>
        public string ChangePasswordUrl { get; set; } = "api/changepassword";

        /// <summary>
        /// 删除用户，支持批量删除
        /// </summary>
        public string DeleteUserUrl { get; set; } = "api/deleteusers";
        /// <summary>
        /// 为用户添加角色
        /// </summary>
        public string AddToRoleUrl { get; set; } = "api/AddToRole";
        /// <summary>
        /// 创建角色
        /// </summary>
        public string CreateRoleUrl { get; set; } = "api/CreateRole";
        /// <summary>
        /// 删除角色
        /// </summary>
        public string DeleteRolesUrl { get; set; } = "api/DeleteRoles";

        /// <summary>
        /// 获取角色
        /// </summary>
        public string GetRolesUrl { get; set; } = "api/GetRoles";
        /// <summary>
        /// 根据用户ID获取用户
        /// </summary>
        public string GetUserUrl { get; set; } = "api/GetUser";
        /// <summary>
        /// 获取所以角色
        /// </summary>
        public string GetUsersUrl { get; set; } = "api/GetUsers";
        /// <summary>
        /// 重置密码
        /// </summary>
        public string ResetPasswordUrl { get; set; } = "api/ResetPassword";
        /// <summary>
        /// 更新角色
        /// </summary>
        public string UpdateRoledUrl { get; set; } = "api/UpdateRole";
        /// <summary>
        /// 更新用户
        /// </summary>
        public string UpdateUserUrl { get; set; } = "api/UpdateUser";

    }
}
