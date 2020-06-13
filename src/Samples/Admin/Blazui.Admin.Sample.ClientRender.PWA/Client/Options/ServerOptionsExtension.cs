using Blazui.Admin.ClientRender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Admin.Sample.ClientRender.PWA.Client.Options
{
    public class ServerOptionsExtension: ServerOptions
    {
        /// <summary>
        /// 获取Resources
        /// </summary>
        public string GetResourcesUrl { get; set; } = "api/GetResources";

        public string GetLoginUserUrl { get; set; } = "api/getLoginuser";
    }
}
