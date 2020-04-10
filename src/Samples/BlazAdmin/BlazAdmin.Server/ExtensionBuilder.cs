using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazAdmin.Server
{
    public static class ExtensionBuilder
    {
        public static IApplicationBuilder UseBlazAdminServerCore(this IApplicationBuilder builder)
        {
            return builder;
        }
    }
}
