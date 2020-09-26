using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.Admin.Sample.ServerRender
{
    public class MainLayoutBase : LayoutComponentBase
    {
        protected List<MenuModel> Menus { get; set; } = new List<MenuModel>();

        protected override void OnInitialized()
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
            }); ;

        }
    }
}
