using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Element.Markdown
{
    public static class ExtensionBuilder
    {
        public static IServiceCollection AddMarkdown(this IServiceCollection services)
        {
            var handlers = Assembly.GetExecutingAssembly().GetExportedTypes()
                 .Where(x => x.IsClass)
                 .Where(x => x.Namespace.EndsWith("IconHandlers"))
                 .Where(x => x.GetInterfaces().Any(x => x == typeof(IIconHandler)))
                 .ToList();
            foreach (var item in handlers)
            {
                services.AddScoped(item);
            }
            return services;
        }
    }
}
