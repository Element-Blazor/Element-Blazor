using Blazui.Admin.Sample.ClientRender.PWA.Shared.IServices;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazui.Admin.Sample.ClientRender.PWA.Client.Shared
{
    public class MainLayoutBase : LayoutComponentBase
    {
        protected List<MenuModel> Menus { get; set; } = new List<MenuModel>();

        [Inject]
        private IUserServiceExtension userServiceExtension { get; set; }


        protected override async Task OnInitializedAsync()
        {
            Menus.Add(new MenuModel()
            {
                Label = "产品管理",
                Icon = "el-icon-star-on",
                Route = "/products"
            });
            Menus.Add(new MenuModel()
            {
                Label = "文档管理",
                Icon = "el-icon-document",
                Children = new List<MenuModel>()
                {
                    new MenuModel(){
                         Label="入门文档",
                         Icon="el-icon-s-promotion",
                         Route="/docs/quickstart"
                    },
                    new MenuModel(){
                         Label="组件文档",
                         Icon="el-icon-s-management",
                         Route="/docs/component"
                    }
                }
            });

            //解决GetRolesWithResources为同步的折中办法
            await userServiceExtension.GetRolesAsync();
            await userServiceExtension.GetResourcesAsync();
        }
    }
}
