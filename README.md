# blazui

#### 介绍
Element的blazor版本

API 模仿 Element，CSS 直接使用 Element的样式，HTML 结构直接使用 Element 的 HTML 结构

Element 地址：https://element.eleme.cn/#/zh-CN/component/layout

Blazui 演示地址：http://blazui.com:9000

主要更新仓库：https://github.com/wzxinchen/Blazui

如果该地址不能访问了请发 Issue 告诉我， **目前版本不稳定** 

#### 关注与讨论

使用遇到问题请加入QQ群：74522853

#### 使用前提
参考Blazor使用的前提条件

1. 安装 .Net Core 3.1
2. 安装 VS2019，更新到最新版

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
    <link rel="stylesheet" href="/_content/Blazui.Component/css/index.css" />
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
@using Blazui.Component.Upload
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

###### [x] 类似于LayAdmin的后台管理模板 https://github.com/wzxinchen/BlazAdmin
###### 开发文档 [ ]

#### 组件列表


| 组件名       | 调用Demo                           | 功能支持     | 下一步计划   |
| :----------- | :----------:                       | -----------  | -----------  |
| 按钮         | http://blazui.com:9000/button      | [√] 常规支持 |              |
| 输入框       | http://blazui.com:9000/input       | [√] 常规支持 |              |
| 单选框       | http://blazui.com:9000/radio       | [√] 常规支持 <br /> [√] 按钮单选框 <br /> [√] 单选框组 <br /> [√] 按钮单选框组 <br /> [√] 带边框的单选框    |          |    
| 复选框       | http://blazui.com:9000/checkbox    | [√] 常规支持 <br /> [√] 按钮复选框 <br /> [√] 复选框组 <br /> [√] 按钮复选框组 |              |          
| 下拉框       | http://blazui.com:9000/select      | [√] 常规支持 <br /> [√] 选项可禁用 |              |
| 切换组件     | http://blazui.com:9000/switch      | [√] 常规支持 <br /> [√] 自定义状态文本 |              |
| 菜单         | http://blazui.com:9000/menu        | [√] 常规支持 <br /> [√] 横向菜单 <br /> [√] 坚向菜单 <br /> [√] 二级菜单 <br /> [√] 自定义背景色 <br /> [×] 多级菜单 |             |
| 标签页       | http://blazui.com:9000/tabs        | [√] 常规支持 <br /> [√] 自定义选项卡样式 <br /> [√] 自定义卡片位置 <br /> [√] 可移除新增 |       |
| 表格         | http://blazui.com:9000/table       | [√] 常规支持 <br /> [√] 自动生成列 <br /> [√] 斑马条纹 <br /> [√] 分页 <br /> [√] 自定义列内容 <br /> [√] 表头锁定 <br /> [√] 复选框列 <br /> [√] 表格边框 <br /> [√] 自适应宽度高度 | [√] 树形表格      |
| 消息         | http://blazui.com:9000/message     | [√] 常规支持 <br /> [√] 四种消息类型 |       |
| 分页         | http://blazui.com:9000/pagination  | [√] 常规支持 |       |
| 加载中       | http://blazui.com:9000/loading     | [√] 常规支持 <br /> [√] 自定义背景颜色、图标、文字 <br /> [√] 全屏加载 <br /> [√] 部分加载 |       |
| 消息弹窗     | http://blazui.com:9000/messagebox  | [√] 常规支持 <br /> [√] Alert弹窗 <br /> [√] Confirm 弹窗 <br /> [√] 无回调 |       |
| 对话框       | http://blazui.com:9000/dialog      | [√] 常规支持 <br /> [√] 嵌套弹窗 <br /> [√] 指定宽度 <br /> [√] 无回调 |       |
| 日期选择器   | http://blazui.com:9000/datepicker  | [√] 常规支持 <br /> [√] 指定日期格式 |       |
| Form 表单    | http://blazui.com:9000/form        | [√] 常规支持 <br /> [√] 三种对齐方式 <br /> [√] 单行表单 |       |
| 布局面板     | http://blazui.com:9000/layout      | [√] 常规支持 <br /> [√] 嵌套布局 |       |
| 文件上传     |      | |[√] 常规支持 <br /> [√] 限制文件大小 <br /> [√] 限制文件类型 <br /> [√] 图片预览 <br /> [√] 多文件上传

#### 感谢

- 测试组件库功能的稳定性和完整度  @deathvicky
- 宣传组件库  https://github.com/zaranetCore  @zaranetCore