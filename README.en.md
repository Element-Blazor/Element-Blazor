# Blazui
![LOGO][1]

[中文][37] - [English][36] 

[Join QQ group chat【.NET Blazor  base camp 74522853】][2]

[Join Blazor community][3]

[Visit the Blazor community management website, username: admin, password: 88888888][4]

## Introduction
A blazor component library using Element UI

API imitates Element, CSS directly uses Element's style, HTML structure directly uses Element's HTML structure

Element Address：https://element.eleme.cn/#/zh-CN/component/layout

## Demo address and related cases
| Address | Content | Server Location |
| :----------- | :----------                       | --|
|https://blazorwasm.github.io|Blazor WebAssembly Version |Gtihub|
|https://pwawasm.github.io| Blazor WebAssembly Version PWA Mode|Github|
|https://www.blazor.group | A Blazor community, based on Blazui development, Server rendering |China| 
|http://blazui.com| Blazor Server Version | China |
|https://admin.blazor.group| Blazor community management website, based on Blazui.Admin |China|
## Directory function and corresponding nuget package

| First-level directory | Second-level directory | Third-level directory | Fourth-level directory | Description |                                                                                                                         |
|----------|------------------|-------------------------|--------------------------------------------|---------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------|
| [src][5] |                  |                         |                                            | source code                               |                                                                                                                                      |
|          | [Admin][6]       |                         |                                            | Management website template based on Blazui        |                                                                                                                                      |
|          |                  | [Admin.ClientRender][7] |                                            | WebAssembly rendering of the template (unfinished)    |                                                                                                                                      |
|          |                  | [Admin.ServerRender][8] |                                            | Server rendering of the template                | [![Nuget](https://img.shields.io/nuget/dt/Blazui.Admin.ServerRender.svg)](https://www.nuget.org/packages/Blazui.Admin.ServerRender/) |
|          |                  | [Admin][9]              |                                            | Template UI framework                      | [![Nuget](https://img.shields.io/nuget/dt/Blazui.Admin.svg)](https://www.nuget.org/packages/Blazui.Admin/)                           |
|          | [Components][10] |                         |                                            | Blazui component source code                       | [![Nuget](https://img.shields.io/nuget/dt/Blazui.Component.svg)](https://www.nuget.org/packages/Blazui.Component/)                   |
|          |                  | [Lang][11]              |                                            | Internationalization support                        |                                                                                                                                      |
|          |                  | ...                     |                                            | Component source code                               |                                                                                                                                      |
|          | [MarkDown][12]   |                         |                                            | MarkDown editor based on Blazui     | [![Nuget](https://img.shields.io/nuget/dt/Blazui.MarkDown.svg)](https://www.nuget.org/packages/Blazui.MarkDown/)                     |
|          |                  | [IconHandlers][13]      |                                            | Markdown editor icon handler         |                                                                                                                                      |
|          |                  | ...                     |                                            | Editor source code                             |                                                                                                                                      |
|          | [Samples][14]    |                         |                                            | Demo code for all components              |                                                                                                                                      |
|          |                  | [Admin][15]             |                                            | Demo code of Blazui.Admin      |                                                                                                                                      |
|          |                  |                         | [Blazui.Admin.Sample.ClientRender.PWA][16] | PWA WASM mode, no server, can not run    |                                                                                                                                      |
|          |                  |                         | [Blazui.Admin.Sample.ServerRender][17]     | Server mode                           |                                                                                                                                      |
|          |                  | [Blazui][18]            |                                            | Blazui demo code, including MarkDown editor |   |
|          |                  |                         | [Blazui.ClientRender.PWA][19]              | PWA WASM mode                          |                                                                                                                                      |
|          |                  |                         | [Blazui.ClientRender][20]                  | WASM mode                          |                                                                                                                                      |
|          |                  |                         | [Blazui.ServerRender][21]                  | Server Mode                             |                                                                                                                                      |


## 主要更新仓库

https://github.com/wzxinchen/Blazui

## 贡献说明

基于 develop 分支拉出新分支，在这个分支上开发，开发完成后发起PR合并至develop

## 使用文档

https://www.blazor.group/topic/be1450fd-1703-4a21-b6c9-d2ed442e4db1

## 更新日志

### 版本 0.0.7.3, 2020.05.06发布
1. BLayout 组件新增键盘事件
2. Extension 扩展类多语言默认为空以解决报错问题
    
### 版本 0.0.7.2, 2020.04.24发布
    Blazor 升级 preview 5

### 版本 0.0.7.0，2020.04.24发布
1. 调整 [将 Blazui 基础组件, BlazAdmin, MarkDown Editor 移动到一个仓库][22]
2. 修复 [Table checkbox-SelectedRowsChanged 外部传入EventCallBack后，会使得列表checkbox无法勾选][23]
3. 增加 [Dialog全屏弹窗扩展][24]
4. 增加 [国际化支持(Internationalization support) ][25]
5. 修复 [BSubMenuBase 的 SemaphoreSlim 属性应该是只读属性，同时应该释放][26]
6. 修复 [BPaginationBase在PageSize修改后最大页码不会跟着更新][27]
7. 修复 [BInput InputType=TextArea 渲染无效][28]
8. 增加 [wasm的支持][29]
9. 增加 [BTable增加Attribute忽略项][30]
10. 修复 [MessageService.Show连续使用只显示第一个][31]
11. 修复 [BTable无数据提示][32]
12. 修复 [Dialog叉号关闭窗口报错][33]
13. 修复 [DialogService.ShowDialogAsync 弹出一个窗口关闭后无法紧接着弹出第二个][34]
14. 添加 [BInput控件，当设置的数据类型是数字时，输入字母后，后面的清空按钮失效了][35]

### 版本 0.0.6.2 2020.01.06发布
1. 新增 文件上传组件
2. 新增 Markdown 编辑器 及 Markdown 渲染器
3. 修复 BDatePicker控件DateChanged有问题

### 版本 0.0.6.1，2020.01.03发布
1. 新增 BDropDown 下拉菜单组件
2. 修复 Table控件中 日期的格式化不起作用
3. 修复 table 全选
4. 增加 Input组件可否添加尺寸（Size
5. 修复 下拉弹出菜单频繁点击会出现多个
6. 修复 按钮不支持圆形
7. 修复 弹窗组件
8. 修复 BButton组件中Cls属性改为追加感觉更合理
9. 修复 \[Bug Report\] Checkbox 全选时选项没联动选中
10. 添加 about loading
11. 添加 是否可以让BMenuItem组件具有匹配路由然后选中的功能
12. 修复 表单下拉列表
13. 添加 组件可增加一个visible属性，用于隐藏或显示
14. 添加 弹窗-表单赋值的时候radio组件-枚举值无法绑定
15. 修复 tab标签页只能关闭当前的


  [1]: http://static.zybuluo.com/wzxinchen/c9qvwe1agljfwvqm1j34x1fw/BLAZUI%20128.png
  [2]: https://jq.qq.com/?_wv=1027&k=5jdzC6m
  [3]: https://www.blazor.group
  [4]: https://admin.blazor.group
  [5]: https://github.com/wzxinchen/Blazui/tree/master/src
  [6]: https://github.com/wzxinchen/Blazui/tree/master/src/Admin
  [7]: https://github.com/wzxinchen/Blazui/tree/master/src/Admin/Admin.ClientRender
  [8]: https://github.com/wzxinchen/Blazui/tree/master/src/Admin/Admin.ServerRender
  [9]: https://github.com/wzxinchen/Blazui/tree/master/src/Admin/Admin
  [10]: https://github.com/wzxinchen/Blazui/tree/master/src/Components
  [11]: https://github.com/wzxinchen/Blazui/tree/master/src/Components/Lang
  [12]: https://github.com/wzxinchen/Blazui/tree/master/src/Markdown
  [13]: https://github.com/wzxinchen/Blazui/tree/master/src/Markdown/IconHandlers
  [14]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples
  [15]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Admin
  [16]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Admin/Blazui.Admin.Sample.ClientRender.PWA
  [17]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Admin/Blazui.Admin.Sample.ServerRender
  [18]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Blazui
  [19]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Blazui/Blazui.ClientRender.PWA
  [20]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Blazui/Blazui.ClientRender
  [21]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Blazui/Blazui.ServerRender
  [22]: https://github.com/wzxinchen/Blazui/issues/90
  [23]: https://github.com/wzxinchen/Blazui/issues/87
  [24]: https://github.com/wzxinchen/Blazui/issues/88
  [25]: https://github.com/wzxinchen/Blazui/issues/89
  [26]: https://github.com/wzxinchen/Blazui/issues/85
  [27]: https://github.com/wzxinchen/Blazui/issues/79
  [28]: https://github.com/wzxinchen/Blazui/issues/80
  [29]: https://github.com/wzxinchen/Blazui/issues/83
  [30]: https://github.com/wzxinchen/Blazui/issues/75
  [31]: https://github.com/wzxinchen/Blazui/issues/77
  [32]: https://github.com/wzxinchen/Blazui/issues/78
  [33]: https://github.com/wzxinchen/Blazui/issues/74
  [34]: https://github.com/wzxinchen/Blazui/issues/73
  [35]: https://github.com/wzxinchen/Blazui/issues/71
  [36]: https://github.com/wzxinchen/Blazui/blob/master/README.en.md
  [37]: https://github.com/wzxinchen/Blazui/blob/master/README.md
