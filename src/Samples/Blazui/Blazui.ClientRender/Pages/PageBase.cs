

using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Element.ClientRender.Model;
using System.Reflection;

namespace Element.ClientRender.Pages
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
            }); demoPages.Add(new DemoPageModel()
            {
                Name = "lang",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicLang.razor"
                      },
                       Name="Lang",
                        Title="基础用法"
                 }, new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicLangInject.razor"
                      },
                       Name="Lang",
                        Title="继承用法"
                 }

                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "layout",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicLayout.razor"
                      },
                       Name="Layout",
                        Title="基础用法"
                 }, new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "NestedLayout.razor"
                      },
                       Name="Layout",
                        Title="嵌套布局"
                 }, new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "Layout.razor"
                      },
                       Name="Layout",
                        Title="后台模板用法"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "loading",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicLoading.razor"
                      },
                       Name="Loading",
                        Title="区域加载"
                 }, new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "CustomLoading.razor"
                      },
                       Name="Loading",
                        Title="自定义加载"
                 }, new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "ServiceLoading.razor"
                      },
                       Name="Loading",
                        Title="服务调用"
                 }, new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "ManualLoading.razor"
                      },
                       Name="Loading",
                        Title="区域手动加载"
                 }, new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "CardLoading.razor"
                      },
                       Name="Loading",
                        Title="卡片 Loading"
                 }, new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "MenuLoading.razor"
                      },
                       Name="Loading",
                        Title="菜单 Loading"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "markdowneditor",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicEditor.razor"
                      },
                       Name="MarkdownEditor",
                        Title="基础用法"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "FormMarkdown.razor"
                      },
                       Name="MarkdownEditor",
                        Title="表单提交"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "Markdown.razor"
                      },
                       Name="MarkdownEditor",
                        Title="Markdown 显示"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "menu",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "HorizontalMenu.razor"
                      },
                       Name="Menu",
                        Title="横向菜单"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "VerticalMenu.razor"
                      },
                       Name="Menu",
                        Title="坚向菜单"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "CustomBackgroundMenu.razor"
                      },
                       Name="Menu",
                        Title="自定义背景"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "message",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicMessage.razor"
                      },
                       Name="Message",
                        Title="消息"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "StatusMessage.razor"
                      },
                       Name="Message",
                        Title="不同状态"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "MultiMessage.razor"
                      },
                       Name="Message",
                        Title="快速多个消息"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "messagebox",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicMessageBox.razor"
                      },
                       Name="MessageBox",
                        Title="基础用法"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "ConfirmMessageBox.razor"
                      },
                       Name="MessageBox",
                        Title="确认消息"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "pagination",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicPagination.razor"
                      },
                       Name="Pagination",
                        Title="基础用法"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BackgroundPagination.razor"
                      },
                       Name="Pagination",
                        Title="无背景"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "switch",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicSwitch.razor"
                      },
                       Name="Switch",
                        Title="基础用法"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "TextSwitch.razor"
                      },
                       Name="Switch",
                        Title="文本描述"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "DisabledSwitch.razor"
                      },
                       Name="Switch",
                        Title="禁用状态"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "tabs",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicTab.razor"
                      },
                       Name="Tab",
                        Title="基础的、简洁的标签页"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "CardTab.razor"
                      },
                       Name="Tab",
                        Title="选项卡样式的标签页"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BorderCardTab.razor"
                      },
                       Name="Tab",
                        Title="卡片化的标签页"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "LeftTab.razor"
                      },
                       Name="Tab",
                        Title="在左边的标签页"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "EditableTab.razor"
                      },
                       Name="Tab",
                        Title="调用事件API实现可编辑的标签页"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BindingEditableTab.razor"
                      },
                       Name="Tab",
                        Title="双向绑定实现可编辑的标签页"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "table",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicTable.razor"
                      },
                       Name="Table",
                        Title="基础用法"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "StripeTable.razor"
                      },
                       Name="Table",
                        Title="斑马条纹用法"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BorderedTable.razor"
                      },
                       Name="Table",
                        Title="边框表格"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "FixHeaderTable.razor"
                      },
                       Name="Table",
                        Title="固定表头"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "CheckBoxTable.razor"
                      },
                       Name="Table",
                        Title="复选框表格"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "CustomColumnTable.razor"
                      },
                       Name="Table",
                        Title="带操作列的表格"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "AutoGenerateColumnTable.razor"
                      },
                       Name="Table",
                        Title="自动生成列，宽度自适应"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "PaginationTable.razor"
                      },
                       Name="Table",
                        Title="分页"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "IgnoreColumnTable.razor"
                      },
                       Name="Table",
                        Title="过滤显示字段"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "SearchTable.razor"
                      },
                       Name="Table",
                        Title="搜索"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "CustomTableOrder.razor"
                      },
                       Name="Table",
                        Title="自定义列顺序"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "form",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicForm.razor"
                      },
                       Name="Form",
                        Title="基础用法"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "InitilizeForm.razor"
                      },
                       Name="Form",
                        Title="表单初始值"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "AlignForm.razor"
                      },
                       Name="Form",
                        Title="表单对齐"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "HiddenFormItem.razor"
                      },
                       Name="Form",
                        Title="隐藏表单项"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "InlineForm.razor"
                      },
                       Name="Form",
                        Title="行内表单"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "transfer",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicTransfer.razor"
                      },
                       Name="Transfer",
                        Title="基础用法"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "FormTransfer.razor"
                      },
                       Name="Transfer",
                        Title="表单提交"
                 }
                }
            });
            demoPages.Add(new DemoPageModel()
            {
                Name = "upload",
                Demos = new List<DemoInfoModel>() {
                 new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BasicUpload.razor"
                      },
                       Name="Upload",
                        Title="基础用法"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "BUploadLimit.razor"
                      },
                       Name="Upload",
                        Title="限制上传"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "ImageUpload.razor"
                      },
                       Name="Upload",
                        Title="图片上传"
                 },new DemoInfoModel
                 {
                      Files=new List<string>(){
                      "UploadForm.razor"
                      },
                       Name="Upload",
                        Title="表单上传"
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
                    Type = "Element.ClientRender.Demo." + item.Name + "." + item.Files.FirstOrDefault().Replace(".razor", string.Empty),
                    Title = item.Title
                };
                demos.Add(demoModel);
            }
            return demos;
        }
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
            await jSRuntime.InvokeVoidAsync("renderHightlight", ((BTabPanel)tab).TabContainer.Content);
        }
    }
}
