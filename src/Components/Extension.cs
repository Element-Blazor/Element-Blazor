using Element.ControlRender;
using Element.ControlRenders;
using Element.Core;
using Element.DisplayRenders;
using Element.Lang;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Element
{
    public static class Extension
    {
        /// <summary>
        /// 添加 Blazui 相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBlazuiServices(this IServiceCollection services)
        {
            services.AddSingleton<FormFieldControlMap>();
            services.AddSingleton<TableEditorMap>();
            services.AddScoped<Document>();
            services.AddScoped<MessageService>();
            services.AddSingleton<LoadingService>();
            services.AddScoped<DialogService>();
            services.AddScoped<PopupService>();
            services.AddScoped<MessageBox>();
            services.AddScoped<IInputRender, InputRender>();
            services.AddScoped<ISelectRender, SelectRender>();
            services.AddScoped<ISwitchRender, SwitchRender>();
            services.AddScoped<IDatePickerRender, EmptyRender>();
            services.AddScoped<IUploadRender, UploadRender>();
            services.AddScoped<ITableRender, TableRender>();
            services.AddSingleton<DisplayRenderFactory>();
            services.AddSingleton<DateTimeRender>();
            services.AddSingleton<EnumRender>();
            services.AddSingleton<GenericRender>();
            return services;
        }

    }
}
