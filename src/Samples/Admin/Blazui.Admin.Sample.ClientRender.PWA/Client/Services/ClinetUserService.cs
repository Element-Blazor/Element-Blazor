using Blazui.Admin;
using Blazui.Admin.ClientRender;
using Blazui.Admin.Sample.ClientRender.PWA.Client.Options;
using Blazui.Admin.Sample.ClientRender.PWA.Shared;
using Blazui.Admin.Sample.ClientRender.PWA.Shared.IServices;
using Blazui.Component;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Blazor.Cms.Client.Services
{
    public class ClinetUserService : IUserServiceExtension
    {
        private readonly HttpClient _httpClient;
        private readonly ServerOptionsExtension _serverOptions;
        public ClinetUserService(HttpClient httpClient, IOptions<ServerOptionsExtension> serverOptions)
        {
            _httpClient = httpClient;
            _serverOptions = serverOptions.Value;
        }
        public List<RoleModel> roleModels { get; private set; }
        public List<RoleResource> roleResources { get; private set; }

        public Task<string> AddToRoleAsync(string username, params string[] roles)
        {
            return _httpClient.PostAsync($"{_serverOptions.AddToRoleUrl}/{username}", roles);
        }

        public Task<string> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            var data = new ChangePassword()
            {
                username = username,
                oldPassword = oldPassword,
                newPassword = newPassword,
            };
            return _httpClient.PostAsync($"{_serverOptions.ChangePasswordUrl}", data);
        }

        public Task<string> CheckPasswordAsync(string username, string password)
        {
            var data = new UserModel()
            {
                Username = username,
                Password = password
            };
            return _httpClient.PostAsync($"{_serverOptions.CheckPasswordUrl}", data);
        }

        public Task<string> CreateRoleAsync(RoleModel role)
        {
            return _httpClient.PostAsync($"{_serverOptions.CreateRoleUrl}", role);
        }

        public Task<string> CreateSuperUserAsync(string username, string password)
        {
            var data = new UserModel()
            {
                Username = username,
                Password = password
            };
            return _httpClient.PostAsync($"{_serverOptions.CreateSuperUserUrl}", data);
        }

        public Task<string> CreateUserAsync(UserModel userModel)
        {
            return _httpClient.PostAsync($"{_serverOptions.CreateUserUrl}", userModel);
        }

        public async ValueTask<string> DeleteRolesAsync(params string[] ids)
        {
            return await _httpClient.PostAsync($"{_serverOptions.DeleteRolesUrl}", ids);
        }

        public Task<string> DeleteUsersAsync(params string[] userIds)
        {
            return _httpClient.PostAsync($"{_serverOptions.DeleteUserUrl}", userIds);
        }

        public async Task<List<RoleResource>> GetResourcesAsync()
        {
            return this.roleResources = await _httpClient.GetFromJsonAsync<List<RoleResource>>($"{_serverOptions.GetResourcesUrl}");
        }

        public async Task<List<RoleModel>> GetRolesAsync()
        {
            return this.roleModels = await _httpClient.GetFromJsonAsync<List<RoleModel>>($"{_serverOptions.GetRolesUrl}");
        }

        public Task<List<RoleModel>> GetRolesAsync(string userId)
        {
            return _httpClient.GetFromJsonAsync<List<RoleModel>>($"{_serverOptions.GetRolesUrl}/{userId}");
        }

        public string GetRolesWithResources(params string[] resources)
        {
            var roleIds = this.roleResources.Where(x => resources.Contains(x.ResourceId)).Select(x => x.RoleId).ToArray();
            var roleNames = this.roleModels.Where(x => roleIds.Contains(x.Id)).Select(x => x.Name).ToArray();
            return string.Join(",", roleNames);
        }

        public Task<UserModel> GetUserAsync(string userId)
        {
            return _httpClient.GetFromJsonAsync<UserModel>($"{_serverOptions.GetUserUrl}/{userId}");
        }

        public Task<List<UserModel>> GetUsersAsync()
        {
            return _httpClient.GetFromJsonAsync<List<UserModel>>($"{_serverOptions.GetUsersUrl}");
        }

        public async ValueTask<bool> IsRequireInitilizeAsync()
        {
            return await _httpClient.GetFromJsonAsync<bool>($"{_serverOptions.RequireInitilizeUrl}");
        }

        public async ValueTask<string> LoginAsync(BForm form, string username, string password, string callback)
        {
            await form.SubmitAsync($"{_serverOptions.LoginUrl}?callback=" + callback);
            return string.Empty;
        }

        public async ValueTask<string> LogoutAsync(BForm form, string callback)
        {
            await form.SubmitAsync($"{_serverOptions.LogoutUrl}?callback" + callback);
            return string.Empty;
        }

        public async ValueTask<string> ResetPasswordAsync(string id, string password)
        {
            var data = new UserModel()
            {
                Id = id,
                Password = password
            };
            return await _httpClient.PostAsync($"{_serverOptions.ResetPasswordUrl}", data);
        }

        public Task<string> UpdateRoleAsync(RoleModel roleModel)
        {
            return _httpClient.PostAsync($"{_serverOptions.UpdateRoledUrl}", roleModel);
        }

        public Task<string> UpdateUserAsync(UserModel userModel)
        {
            return _httpClient.PostAsync($"{_serverOptions.UpdateUserUrl}", userModel);
        }
    }
}
