using Blazui.Admin.Sample.ClientRender.PWA.Shared;
using Blazui.Admin.Sample.ClientRender.PWA.Shared.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Admin.Sample.ClientRender.PWA.Server.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServiceExtension userService;

        public UserController(IUserServiceExtension userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// 执行用户登出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/logout")]
        public async ValueTask<IActionResult> Logout([FromQuery] string callback)
        {
            var err = await userService.LogoutAsync(null, callback);

            if (string.IsNullOrWhiteSpace(err))
            {
                return Redirect(callback ?? "/");
            }
            return BadRequest(err);
        }

        /// <summary>
        /// 执行用户登出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/login")]
        public async ValueTask<IActionResult> Login([FromForm] UserModel user, [FromQuery] string callback)
        {
            var err = await userService.LoginAsync(null, user.Username, user.Password, callback);

            if (string.IsNullOrWhiteSpace(err))
            {
                return Redirect(callback ?? "/");
            }
            return BadRequest(err);
        }

        [HttpGet]
        [Route("api/getLoginuser")]
        public IActionResult GetLoginUser()
        {
            if (User.Identity.IsAuthenticated)
                return Ok(User.Claims.ToDictionary(c => c.Type, c => c.Value));
            else
                return Ok(new Dictionary<string, string>());

        }

        [HttpPost("api/AddToRole/{username}")]
        public async Task<string> AddToRoleAsync(string username, string[] roles)
        {
            return await this.userService.AddToRoleAsync(username, roles);
        }
        [HttpPost("api/ChangePassword")]
        public async Task<string> ChangePasswordAsync(ChangePassword changePassword)
        {
            return await this.userService.ChangePasswordAsync(changePassword.username, changePassword.oldPassword, changePassword.newPassword);
        }
        [HttpPost("api/CheckPassword")]
        public async Task<string> CheckPasswordAsync(UserModel userModel)
        {
            return await this.userService.CheckPasswordAsync(userModel.Username, userModel.Password);
        }

        [HttpPost("api/CreateRole")]
        public async Task<string> CreateRoleAsync(RoleModel role)
        {
            return await this.userService.CreateRoleAsync(role);
        }

        [HttpPost("api/CreateSuperUser")]
        public async Task<string> CreateSuperUserAsync(UserModel userModel)
        {
            return await this.userService.CreateSuperUserAsync(userModel.Username, userModel.Password);
        }

        [HttpPost("api/CreateUser")]
        public async Task<string> CreateUserAsync(UserModel userModel)
        {
            return await this.userService.CreateUserAsync(userModel);
        }

        [HttpPost("api/DeleteRoles")]
        public async ValueTask<string> DeleteRolesAsync(string[] ids)
        {
            return await this.userService.DeleteRolesAsync(ids);
        }

        [HttpPost("api/DeleteUsers")]
        public async Task<string> DeleteUsersAsync(string[] userIds)
        {
            return await this.userService.DeleteUsersAsync(userIds);
        }

        [HttpGet("api/GetRoles")]
        public Task<List<RoleModel>> GetRoles()
        {
            return this.userService.GetRolesAsync();
        }

        [HttpGet("api/GetRoles/{userId}")]
        public async Task<List<RoleModel>> GetRolesAsync(string userId)
        {
            return await this.userService.GetRolesAsync(userId);
        }

        [HttpGet("api/GetRolesWithResources")]
        public string GetRolesWithResources(string resources)
        {
            return this.userService.GetRolesWithResources(resources.Split(','));
        }

        [HttpGet("api/GetResources")]
        public Task<List<RoleResource>> GetResourcesAsync()
        {
            return this.userService.GetResourcesAsync();
        }

        [HttpGet("api/GetUser/{userId}")]
        public async Task<UserModel> GetUserAsync(string userId)
        {
            return await this.userService.GetUserAsync(userId);
        }

        [HttpGet("api/GetUsers")]
        public async Task<List<UserModel>> GetUsersAsync()
        {
            return await this.userService.GetUsersAsync();
        }

        [HttpGet("api/IsRequireInitilize")]
        public async ValueTask<bool> IsRequireInitilizeAsync()
        {
            return await this.userService.IsRequireInitilizeAsync();
        }

        [HttpPost("api/ResetPassword")]
        public async ValueTask<string> ResetPasswordAsync(UserModel userModel)
        {
            return await this.userService.ResetPasswordAsync(userModel.Id, userModel.Password);
        }

        [HttpPost("api/UpdateRole")]
        public async Task<string> UpdateRoleAsync(RoleModel roleModel)
        {
            return await this.userService.UpdateRoleAsync(roleModel);
        }

        [HttpPost("api/UpdateUser")]
        public async Task<string> UpdateUserAsync(UserModel userModel)
        {
            return await this.userService.UpdateUserAsync(userModel);
        }
    }
}
