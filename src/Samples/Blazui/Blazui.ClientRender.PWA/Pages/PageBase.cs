

using Blazui.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using Blazui.ClientRender.PWA.Model;
using System.Reflection;

namespace Blazui.ClientRender.PWA.Pages
{
    public class PageBase : ComponentBase
    {
        private static List<DemoPageModel> demoPages = new List<DemoPageModel>();
        static PageBase()
        {
            demoPages.Add(new DemoPageModel()
            {
                Name = "button",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicButton.razor"
                      },
                       Name="BasicButton",
                        Title="基础用法"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "ButtonGroup.razor"
                      },
                       Name="BasicButton",
                        Title="按钮组"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "ButtonSize.razor"
                      },
                       Name="BasicButton",
                        Title="按钮尺寸"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "DisabledButton.razor"
                      },
                       Name="BasicButton",
                        Title="禁用的按钮"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "LoadingButton.razor"
                      },
                       Name="BasicButton",
                        Title="按钮加载中"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "TextButton.razor"
                      },
                       Name="BasicButton",
                        Title="文字按钮"
                 }
                 }

            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "input",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicInput.razor"
                      },
                       Name="BasicInput",
                        Title="基础用法"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "SizeInput.razor"
                      },
                       Name="BasicInput",
                        Title="输入框尺寸"
                 }
                 }

            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "radio",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicRadio.razor"
                      },
                       Name="BasicRadio",
                        Title="基础用法"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BorderedRadio.razor"
                      },
                       Name="BasicRadio",
                        Title="有边框"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "DisabledRadio.razor"
                      },
                       Name="BasicRadio",
                        Title="禁用的单选框"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "DisallowChangeRadio.razor"
                      },
                       Name="BasicRadio",
                        Title="不允许变更的单选框"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "RadioButton.razor"
                      },
                       Name="BasicRadio",
                        Title="单选按钮"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "RadioGroup.razor"
                      },
                       Name="BasicRadio",
                        Title="单选框组"
                 }
                 }

            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "select",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicSelect.razor"
                      },
                       Name="BasicSelect",
                        Title="基础用法"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BindEnum.razor"
                      },
                       Name="BasicSelect",
                        Title="绑定枚举(可空)"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "ItemDisabledSelect.razor"
                      },
                       Name="BasicSelect",
                        Title="选项被禁用"
                 }
                 }

            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "checkbox",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "CheckBoxButtonGroup.razor"
                      },
                       Name="CheckBox",
                        Title="复选按钮组"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "CheckBoxGroup.razor"
                      },
                       Name="CheckBox",
                        Title="复选框组"
                 },
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "HardCode.razor"
                      },
                       Name="CheckBox",
                        Title="硬编码复选框"
                 }
                 }

            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "datepicker",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicPicker.razor"
                      },
                       Name="DatePicker",
                        Title="基础用法"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "dialog",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicDialog.razor"
                      },
                       Name="Dialog",
                        Title="基础用法"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "NestDialog.razor"
                      },
                       Name="Dialog",
                        Title="无限弹窗"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "PassParameter.razor"
                      },
                       Name="Dialog",
                        Title="传递参数"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "dropdown",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicDropDown.razor"
                      },
                       Name="DropDown",
                        Title="基础用法"
                 }
                }
            });
        }
        private IList<DemoModel> Code(string name)
        {
            var demoInfo = demoPages.SingleOrDefault(x => x.Name == name);
            if (demoInfo == null)
            {
                return new List<DemoModel>();
            }
            var demos = new List<DemoModel>();
            foreach (var item in demoInfo.Demos)
            {
                var demoModel = new DemoModel()
                {
                    Type = "Blazui.ClientRender.PWA.Demo." + item.Name + "." + item.Files.FirstOrDefault().Replace(".razor", string.Empty),
                    Title = item.Title
                };
                demos.Add(demoModel);
            }
            return demos;
        }
        [Inject]
        private IHttpClientFactory httpClientFactory { get; set; }
        [Inject]
        protected IJSRuntime jSRuntime { get; set; }

        protected IList<DemoModel> demos;


        [Inject]
        private NavigationManager NavigationManager { get; set; }
        protected override void OnInitialized()
        {
            var router = NavigationManager.Uri.Split('/').LastOrDefault();
            demos = Code(router);

            foreach (var item in demos)
            {
                item.Demo = Assembly.GetExecutingAssembly().GetType(item.Type);
            }
        }

        protected async Task TabCode_OnRenderCompleteAsync(object tab)
        {
            await jSRuntime.InvokeVoidAsync("renderHightlight", ((BTabPanelBase)tab).TabContainer.Content);
        }
    }
}
