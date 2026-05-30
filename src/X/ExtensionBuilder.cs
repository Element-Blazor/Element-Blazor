using Microsoft.Extensions.DependencyInjection;

namespace Element.X
{
    public static class ExtensionBuilder
    {
        public static IServiceCollection AddElementX(this IServiceCollection services)
        {
            services.AddScoped<ElementXStreamService>();
            services.AddScoped<ElementXRequestService>();
            services.AddScoped<ElementXRecordService>();
            return services;
        }
    }
}
