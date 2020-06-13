using Blazor.Cms.Client.Services;
using Blazui.Admin.Abstract;
using Blazui.Admin.ClientRender;
using Blazui.Admin.Sample.ClientRender.PWA.Client.Options;
using Blazui.Admin.Sample.ClientRender.PWA.Shared.IServices;
using Blazui.Component;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blazui.Admin.Sample.ClientRender.PWA.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddOptions<ServerOptionsExtension>()
                .Configure(o => o.ServerUrl = builder.HostEnvironment.BaseAddress);

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(sp.GetRequiredService<IOptions<ServerOptionsExtension>>().Value.ServerUrl) });

            builder.Services.AddSingleton<IUserServiceExtension, ClinetUserService>();
            builder.Services.AddSingleton<IUserService>(sp =>sp.GetRequiredService<IUserServiceExtension>());
            await builder.Services.AddBlazuiServicesAsync();
            builder.Services.AddSingleton<RouteService>();
            builder.Services.AddSingleton<ResourceAccessor>();

            //简单实现认证
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddAuthorizationCore();
            await builder.Build().RunAsync();
        }
    }
}
