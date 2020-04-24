using Blazui.Admin.Abstract;
using Blazui.Component;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Admin.ClientRender
{
    public class UserService : IUserService
    {
        private readonly HttpClient httpClient;

        public ServerOptions Options { get; }

        public UserService(IOptions<ServerOptions> options, IHttpClientFactory httpClientFactory)
        {
            this.Options = options.Value;
            this.httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(Options.ServerUrl);
        }

        public Task<List<dynamic>> GetUsersAsync()
        {
            return Task.FromResult(new List<dynamic>());
        }

        public async Task<bool> IsRequireInitilizeAsync()
        {
            var response = await httpClient.GetAsync(Options.RequireInitilizeUrl);
            return response.StatusCode == System.Net.HttpStatusCode.NotFound;
        }

        public async Task<string> CreateSuperUserAsync(string username, string password)
        {
            var response = await httpClient.PostAsync(Options.CreateSuperUserUrl, new
            {
                Username = username,
                Password = password
            });
            return response;
        }

        public async Task<string> DeleteUsersAsync(params object[] users)
        {
            var ids = new List<string>();
            var type = users.GetType().GetElementType();
            var properties = type.GetProperties();
            var idProperty = properties.FirstOrDefault(x => x.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase));
            if (idProperty == null)
            {
                idProperty = properties.FirstOrDefault(x => x.Name.Equals("userid", StringComparison.CurrentCultureIgnoreCase));
            }
            if (idProperty == null)
            {
                idProperty = properties.FirstOrDefault(x => x.Name.EndsWith("id", StringComparison.CurrentCultureIgnoreCase));
            }
            if (idProperty == null)
            {
                throw new BlazuiException($"类型 {type.Name} 没有找到 id 属性，已按照如下规则查找，不区分大小写：1、id 属性，2、userid 属性，3、以 id 结尾的属性");
            }
            foreach (var user in users)
            {
                ids.Add(idProperty.GetValue(user)?.ToString());
            }
            var response = await httpClient.PostAsync(Options.DeleteUserUrl, ids);
            return response;
        }

        public async Task<string> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            var response = await httpClient.PostAsync(Options.ChangePasswordUrl, new
            {
                Username = username,
                OldPassword = oldPassword,
                NewPasword = newPassword
            });
            return response;
        }

        public async Task<string> CheckPasswordAsync(string username, string password)
        {
            var response = await httpClient.PostAsync(Options.CheckPasswordUrl, new
            {
                Username = username,
                Password = password
            });
            return response;
        }

        public Task<string> CreateUserAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateRoleAsync(string roleName, string id)
        {
            throw new NotImplementedException();
        }

        public Task<string> AddToRoleAsync(string username, params string[] roles)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteUserAsync(object user)
        {
            throw new NotImplementedException();
        }

        public Task<string> LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        ValueTask<bool> IUserService.IsRequireInitilizeAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<string> LogoutAsync(BForm form, string callback)
        {
            throw new NotImplementedException();
        }

        public ValueTask<string> LoginAsync(BForm form, string username, string password, string callback)
        {
            throw new NotImplementedException();
        }
    }
}
