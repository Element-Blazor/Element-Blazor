using Blazui.Admin.Sample.ClientRender.PWA.Client.Options;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blazor.Cms.Client.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        readonly HttpClient _httpClient;
        private readonly ServerOptionsExtension _serverOptions;
        Dictionary<string, string> User;
        public CustomAuthStateProvider(HttpClient httpClient, IOptions<ServerOptionsExtension> serverOptions)
        {
            
            _httpClient = httpClient;
            _serverOptions = serverOptions.Value;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            User ??= await _httpClient.GetFromJsonAsync<Dictionary<string, string>>(_serverOptions.GetLoginUserUrl);

            if (User == null|| User.Count<=0)
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }
            var identity = new ClaimsIdentity(User.Select(c=>new Claim(c.Key,c.Value)), "Fake authentication type");

            return new AuthenticationState(new ClaimsPrincipal(identity));

        }
        public void NotifyAuthenticationState()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
