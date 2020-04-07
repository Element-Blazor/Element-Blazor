using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazAdmin.Docs.Shared
{
    public class MainLayoutBase : LayoutComponentBase
    {
        protected LoginInfoModel DefaultUser { get; set; } = new LoginInfoModel()
        {
            Username = "admin",
            Password = "admin888"
        };
        protected List<MenuModel> Menus { get; set; } = new List<MenuModel>();

        protected override void OnInitialized()
        {
            Menus.Add(new MenuModel()
            {
                Label = "快速上手",
                Icon = "el-icon-s-promotion",
                Children = new List<MenuModel>() {
                  new MenuModel(){
                   Label="Blazui 入门",
                Icon = "el-icon-s-promotion",
                   Route="/guide/blazui"
                  },
                     new MenuModel(){
                   Label="BlazAdmin 入门",
                Icon = "el-icon-s-promotion",
                   Route="/guide/blazadmin"
                  }
                 }
            });
            Menus.Add(new MenuModel()
            {
                Label = "基础组件",
                Children = new List<MenuModel>() {
                  new MenuModel(){
                   Label="Button 按钮",
                   Route="/button",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="Input 输入框",
                   Route="/input",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="Radio 单选框",
                   Route="/radio",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="Checkbox 多选框",
                   Route="/checkbox",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="Switch",
                   Route="/switch",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="Select 选择器",
                   Route="/select",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="NavMenu 导航菜单",
                   Route="/menu",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="Pagination 分页",
                   Route="/pagination",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="Tabs 标签页",
                   Route="/tabs",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="Table 表格",
                   Route="/table",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="Form 表单",
                   Route="/form",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="DatePicker 日期选择器",
                   Route="/datepicker",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="Layout 布局组件",
                   Route="/layout",
                   Flex=false
                  }
                 }
            });
            Menus.Add(new MenuModel()
            {
                Label = "弹窗组件",
                Children = new List<MenuModel>() {
                  new MenuModel(){
                   Label="Message 消息",
                   Route="/message",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="MessageBox 消息弹窗",
                   Route="/messagebox",
                   Flex=false
                  },
                     new MenuModel(){
                   Label="Dialog 对话框",
                   Route="/dialog",
                   Flex=false
                  }
                 }
            });
        }
    }
}
