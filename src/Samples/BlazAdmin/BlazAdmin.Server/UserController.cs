using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace BlazAdmin.Server
{
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IOptions<CookieAuthenticationOptions> options;

        public UserController(IUserService userService, IOptions<CookieAuthenticationOptions> options)
        {
            this.userService = userService;
            this.options = options;
        }

        /// <summary>
        /// 判断系统是否首次使用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/requireinitilize")]
        public async System.Threading.Tasks.Task<IActionResult> RequireInitilizeAsync()
        {
            if (await userService.HasUserAsync())
            {
                return Ok();
            }
            return NotFound();
        }

        /// <summary>
        /// 判断系统是否首次使用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/user/createsuperuser")]
        public async System.Threading.Tasks.Task<IActionResult> CreateSuperUserAsync([FromBody]UserInfo user)
        {
            var err = await userService.CreateSuperUserAsync(user.Username, user.Password);

            if (string.IsNullOrWhiteSpace(err))
            {
                return Ok();
            }
            return BadRequest(err);
        }

        /// <summary>
        /// 执行用户登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/login")]
        public async System.Threading.Tasks.Task<IActionResult> Login([FromForm]UserInfo user, [FromQuery]string callback)
        {
            var err = await userService.LoginAsync(user.Username, user.Password);

            //HttpContext.Response.Headers.TryGetValue("Set-Cookie", out var cookie);
            //if (cookie != string.Empty)
            //{
            //    var value = options.Value.TicketDataFormat.Unprotect(cookie.ToString());
            //}
            if (string.IsNullOrWhiteSpace(err))
            {
                return Redirect(callback);
            }
            return BadRequest(err);
        }

        /// <summary>
        /// 执行用户登出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/logout")]
        public async System.Threading.Tasks.Task<IActionResult> Logout()
        {
            var err = await userService.LogoutAsync();

            if (string.IsNullOrWhiteSpace(err))
            {
                return Ok();
            }
            return BadRequest(err);
        }

        /// <summary>
        /// 检查用户密码是否正确，不进行登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/checkpassword")]
        public async System.Threading.Tasks.Task<IActionResult> CheckPassword([FromBody]UserInfo user)
        {
            var err = await userService.CheckPasswordAsync(user.Username, user.Password);

            if (string.IsNullOrWhiteSpace(err))
            {
                return Ok();
            }
            return BadRequest(err);
        }

        /// <summary>
        /// 更换用户密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/changepassword")]
        public async System.Threading.Tasks.Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel user)
        {
            var err = await userService.ChangePasswordAsync(user.Username, user.OldPassword, user.NewPassword);

            if (string.IsNullOrWhiteSpace(err))
            {
                return Ok();
            }
            return BadRequest(err);
        }
    }
}
