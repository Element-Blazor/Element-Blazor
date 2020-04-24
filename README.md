# Blazui
![LOGO][1]

[加入群聊【.NET Blazor  大本营】][2]

---

## 介绍
Element的blazor版本

API 模仿 Element，CSS 直接使用 Element的样式，HTML 结构直接使用 Element 的 HTML 结构

Element 地址：https://element.eleme.cn/#/zh-CN/component/layout

## 演示地址及相关案例
|地址 | 内容 | 服务器位置|
| :----------- | :----------                       | --|
|https://blazorwasm.github.io|Blazor WebAssembly 渲染版本|国外|
|https://pwawasm.github.io| Blazor WebAssembly 渲染版本 PWA模式|国外|
|https://www.blazor.group | 基于 Blazui 开发的 Blazor 社区，WASM渲染 |国内| 
|http://blazui.com:9000| Blazor Server 渲染版本 | 国内 |
|https://github.com/caopengfei/BlazorECharts|BlazorECharts 是 Blazor 版的 Echarts 组件，它本身没有依赖 Blazui，但它的 Demo 使用了 Blazui||

## 目录作用及对应 nuget 包
| 一级目录 | 二级目录|三级目录|四级目录|描述|Nuget包
| :----------- | :---------- |--|--|
|[src][3]||||存放源码|
||[Admin][4]|||基于 Blazui 开发的后台管理模板|
|||[Admin.ClientRender][5]||模板的 WebAssembly 渲染版（未完成）|
|||[Admin.ServerRender][6]||模板的 Server 渲染版|[![Nuget](https://img.shields.io/nuget/dt/Blazui.Admin.ServerRender.svg)](https://www.nuget.org/packages/Blazui.Admin.ServerRender/)
|||[Admin][7]||仅为模板的界面框架|[![Nuget](https://img.shields.io/nuget/dt/Blazui.Admin.svg)](https://www.nuget.org/packages/Blazui.Admin/)
||[Components][8]|||Blazui 组件源码|[![Nuget](https://img.shields.io/nuget/dt/Blazui.Component.svg)](https://www.nuget.org/packages/Blazui.Component/)|             
|||[Lang][9]||多语言功能源码|
||| ...||组件源码|
||[MarkDown][10]|||基于 Blazui 开发的 MarkDown 编辑器|[![Nuget](https://img.shields.io/nuget/dt/Blazui.MarkDown.svg)](https://www.nuget.org/packages/Blazui.MarkDown/)
|||[IconHandlers][11]|| MarkDown 编辑器图标的处理程序
||| ...||编辑器源码
||[Samples][12]|||所有组件的演示代码|
|||[Admin][13]||Blazui.Admin 的演示代码
||||[Blazui.Admin.Sample.ClientRender.PWA][14]|PWA WASM 模式，没有服务端，无法运行
||||[Blazui.Admin.Sample.ServerRender][15]|Server 模式
|||[Blazui][16]||Blazui 的演示代码，包括 MarkDown 编辑器
||||[Blazui.ClientRender.PWA][17]| PWA WASM 模式
||||[Blazui.ClientRender][18]| WASM 模式
||||[Blazui.ServerRender][19]| Server模式

## 主要更新仓库

https://github.com/wzxinchen/Blazui

## 贡献说明

基于 develop 分支拉出新分支，在这个分支上开发，开发完成后发起PR合并至develop

## 使用文档

https://www.blazor.group/topic/be1450fd-1703-4a21-b6c9-d2ed442e4db1

## 更新日志

### 版本 0.0.7.0，2020.04.24发布
1. 调整 [将 Blazui 基础组件, BlazAdmin, MarkDown Editor 移动到一个仓库][20]
2. 修复 [Table checkbox-SelectedRowsChanged 外部传入EventCallBack后，会使得列表checkbox无法勾选][21]
3. 增加 [Dialog全屏弹窗扩展][22]
4. 增加 [国际化支持(Internationalization support) ][23]
5. 修复 [BSubMenuBase 的 SemaphoreSlim 属性应该是只读属性，同时应该释放][24]
6. 修复 [BPaginationBase在PageSize修改后最大页码不会跟着更新][25]
7. 修复 [BInput InputType=TextArea 渲染无效][26]
8. 增加 [wasm的支持][27]
9. 增加 [BTable增加Attribute忽略项][28]
10. 修复 [MessageService.Show连续使用只显示第一个][29]
11. 修复 [BTable无数据提示][30]
12. 修复 [Dialog叉号关闭窗口报错][31]
13. 修复 [DialogService.ShowDialogAsync 弹出一个窗口关闭后无法紧接着弹出第二个][32]
14. 添加 [BInput控件，当设置的数据类型是数字时，输入字母后，后面的清空按钮失效了][33]

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


  [1]: https://github.com/wzxinchen/Blazui/blob/master/logo.png
  [2]: https://jq.qq.com/?_wv=1027&k=5jdzC6m
  [3]: https://github.com/wzxinchen/Blazui/tree/master/src
  [4]: https://github.com/wzxinchen/Blazui/tree/master/src/Admin
  [5]: https://github.com/wzxinchen/Blazui/tree/master/src/Admin/Admin.ClientRender
  [6]: https://github.com/wzxinchen/Blazui/tree/master/src/Admin/Admin.ServerRender
  [7]: https://github.com/wzxinchen/Blazui/tree/master/src/Admin/Admin
  [8]: https://github.com/wzxinchen/Blazui/tree/master/src/Components
  [9]: https://github.com/wzxinchen/Blazui/tree/master/src/Components/Lang
  [10]: https://github.com/wzxinchen/Blazui/tree/master/src/Markdown
  [11]: https://github.com/wzxinchen/Blazui/tree/master/src/Markdown/IconHandlers
  [12]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples
  [13]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Admin
  [14]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Admin/Blazui.Admin.Sample.ClientRender.PWA
  [15]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Admin/Blazui.Admin.Sample.ServerRender
  [16]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Blazui
  [17]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Blazui/Blazui.ClientRender.PWA
  [18]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Blazui/Blazui.ClientRender
  [19]: https://github.com/wzxinchen/Blazui/tree/master/src/Samples/Blazui/Blazui.ServerRender
  [20]: https://github.com/wzxinchen/Blazui/issues/90
  [21]: https://github.com/wzxinchen/Blazui/issues/87
  [22]: https://github.com/wzxinchen/Blazui/issues/88
  [23]: https://github.com/wzxinchen/Blazui/issues/89
  [24]: https://github.com/wzxinchen/Blazui/issues/85
  [25]: https://github.com/wzxinchen/Blazui/issues/79
  [26]: https://github.com/wzxinchen/Blazui/issues/80
  [27]: https://github.com/wzxinchen/Blazui/issues/83
  [28]: https://github.com/wzxinchen/Blazui/issues/75
  [29]: https://github.com/wzxinchen/Blazui/issues/77
  [30]: https://github.com/wzxinchen/Blazui/issues/78
  [31]: https://github.com/wzxinchen/Blazui/issues/74
  [32]: https://github.com/wzxinchen/Blazui/issues/73
  [33]: https://github.com/wzxinchen/Blazui/issues/71
