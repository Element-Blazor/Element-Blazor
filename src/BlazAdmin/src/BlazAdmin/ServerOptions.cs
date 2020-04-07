using System;
using System.Collections.Generic;
using System.Text;

namespace BlazAdmin
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
        public string CreateSuperUserUrl { get; set; } = "/api/user/createsuperuser";

        /// <summary>
        /// 系统是否初次使用，需要初始化
        /// </summary>
        public string RequireInitilizeUrl { get; set; } = "/api/requireinitilize";

        /// <summary>
        /// 检查密码正确性的路由地址，该地址仅检查密码正确性，不做登录操作
        /// </summary>
        public string CheckPasswordUrl { get; set; } = "/api/checkpassword";

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
        public string ChangePasswordUrl { get; set; } = "/api/user/changepassword";

        /// <summary>
        /// 删除用户，支持批量删除
        /// </summary>
        public string DeleteUserUrl { get; set; } = "/api/user/deleteusers";
    }
}
