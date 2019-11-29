# blazui

#### 介绍
Element的blazor版本

API 模仿 Element，CSS 直接使用 Element的样式，HTML 结构直接使用 Element 的 HTML 结构

Element 地址：https://element.eleme.cn/#/zh-CN/component/layout

Blazui 演示地址：http://blazui.com:9000

如果该地址不能访问了请发 Issue 告诉我， **目前版本不稳定** 

目前演示服务器配置较低，点击过快可能会有问题

#### 使用前提
参考Blazor使用的前提条件

1. 安装 .Net Core 3.0
2. 安装 VS2019

#### 贡献说明

拉取代码，用 VS2019 打开，直接启动 Blazui.ServerRender 项目

#### 使用说明

基本组件已开发完成

1. 新建 Blazor 服务器端渲染应用
2. 安装 Nuget 包 Blazui.Component
3. 修改 Pages 文件夹下的 _Host.cshtml 为以下内容

```
@page "/"
@namespace Blazui.ServerRender.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html lang="zh-cn">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Blazui, Element的blazor版本，用 .Net 写前端的 UI 框架，开箱即用</title>
    <base href="~/" />
    <link href="css/site.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css" />
    <link rel="stylesheet" href="/_content/Blazui.Component/css/fix.css" />
</head>
<body>
    <app>
        @(await Html.RenderComponentAsync<App>(RenderMode.ServerPrerendered))
    </app>

    <script src="_content/Blazui.Component/js/dom.js"></script>
    <script src="_framework/blazor.server.js"></script>
</body>
</html>

```
4. 在 _Imports.razor 文件中添加以下代码

```
@using Blazui.Component.Container
@using Blazui.Component.Button
@using Blazui.Component.Dom
@using Blazui.Component.Dynamic
@using Blazui.Component.NavMenu
@using Blazui.Component.Input
@using Blazui.Component.Radio
@using Blazui.Component.Select
@using Blazui.Component.CheckBox
@using Blazui.Component.Switch
@using Blazui.Component.Table
@using Blazui.Component.Popup
@using Blazui.Component.Pagination
@using Blazui.Component.Form
```
在 Startup.cs 的 ConfigureServices 方法中添加以下代码

```
services.AddBlazuiServices();
```
为了使弹窗类组件生效，需要将 MainLayout.razor 的内容改为如下

```
@inherits LayoutComponentBase
<BPopup></BPopup>

<div class="sidebar">
    <NavMenu />
</div>

<div class="main">
    @Body
</div>
```


5. 在任意一个页面输入以下代码，运行可看效果

```
<BButton Type="@ButtonType.Primary">主要按钮</BButton>
```
6. 根据演示页面的示例代码写出您想要的效果

7. 有可能会遇到一个由NavigtaionManager抛出的异常，忽略即可

#### 计划

###### 类似于LayAdmin的后台管理模板 https://gitee.com/wzxinchen/BlazAdmin [ ]
###### nuget 包 [√]
###### 开发文档 [ ]
###### 树形表格 [ ]
###### 文件上传 [ ]
###### 图片上传 [ ]
###### 类似于EasyUI的布局面板 [√]

#### 可用组件列表

###### 表格组件 [√] 
###### 消息提示组件 [√] 
###### Loading组件 [√] 
###### 下拉菜单组件 [√] 
###### 对话框组件 [√] 
###### 表单组件 [√] 
###### 按钮组件 [√] 
###### 输入框组件 [√] 
###### Radio组件 [√] 
###### 复选框组件 [√] 
###### Switch组件 [√] 
###### 导航菜单组件 [√] 
###### 标签页组件 [√] 
###### 分页组件 [√] 
###### 消息框组件 [√] 
###### 日期选择器组件 [√] 

#### 关注与讨论

加入QQ群：74522853