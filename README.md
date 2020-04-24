# Blazui
![](https://github.com/wzxinchen/Blazui/blob/master/logo.png )
---

## 介绍
Element的blazor版本

API 模仿 Element，CSS 直接使用 Element的样式，HTML 结构直接使用 Element 的 HTML 结构

Element 地址：https://element.eleme.cn/#/zh-CN/component/layout

Blazui 演示地址：http://pwawasm.github.io/

社区地址：https://www.blazor.group/ 

主要更新仓库：https://github.com/wzxinchen/Blazui

如果该地址不能访问了请发 Issue 告诉我， **目前版本不稳定** 

## 关注与讨论

使用遇到问题请加入QQ群：74522853

## 使用前提
参考Blazor使用的前提条件

1. 安装 .Net Core 3.1
2. 安装 VS2019，更新到最新版

## 源码说明

拉取代码，用 VS2019 打开，直接启动 Blazui.ServerRender 项目

## 使用文档

Blazor 组件库 Blazui 开发入门 https://www.cnblogs.com/wzxinchen/p/12096092.html

类似于LayAdmin的后台管理模板 
https://github.com/wzxinchen/Blazui/tree/master/src/BlazAdmin

---

BlazorECharts 是 Blazor 版的 Echarts 组件，它本身没有依赖 Blazui，但它的 Demo 使用了 Blazui

地址：https://github.com/caopengfei/BlazorECharts

## 组件列表 
#### (需要先访问https://pwawasm.github.io)

| 组件名       | 调用Demo                           | 功能支持     | 下一步计划   |
| :----------- | :----------                       | -----------  | -----------  |
| 按钮         | https://pwawasm.github.io/button      | [√] 常规支持 |              |
| 输入框       | https://pwawasm.github.io/input       | [√] 常规支持 |              |
| 单选框       | https://pwawasm.github.io/radio       | [√] 常规支持 <br /> [√] 按钮单选框 <br /> [√] 单选框组 <br /> [√] 按钮单选框组 <br /> [√] 带边框的单选框    |          |    
| 复选框       | https://pwawasm.github.io/checkbox    | [√] 常规支持 <br /> [√] 按钮复选框 <br /> [√] 复选框组 <br /> [√] 按钮复选框组 |              |          
| 下拉框       | https://pwawasm.github.io/select      | [√] 常规支持 <br /> [√] 选项可禁用 |              |
| 切换组件     | https://pwawasm.github.io/switch      | [√] 常规支持 <br /> [√] 自定义状态文本 |              |
| 菜单         | https://pwawasm.github.io/menu        | [√] 常规支持 <br /> [√] 横向菜单 <br /> [√] 坚向菜单 <br /> [√] 二级菜单 <br /> [√] 自定义背景色 <br /> [×] 多级菜单 |             |
| 标签页       | https://pwawasm.github.io/tabs        | [√] 常规支持 <br /> [√] 自定义选项卡样式 <br /> [√] 自定义卡片位置 <br /> [√] 可移除新增 |       |
| 表格         | https://pwawasm.github.io/table       | [√] 常规支持 <br /> [√] 自动生成列 <br /> [√] 斑马条纹 <br /> [√] 分页 <br /> [√] 自定义列内容 <br /> [√] 表头锁定 <br /> [√] 复选框列 <br /> [√] 表格边框 <br /> [√] 自适应宽度高度 | [√] 树形表格      |
| 消息         | https://pwawasm.github.io/message     | [√] 常规支持 <br /> [√] 四种消息类型 |       |
| 分页         | https://pwawasm.github.io/pagination  | [√] 常规支持 |       |
| 加载中       | https://pwawasm.github.io/loading     | [√] 常规支持 <br /> [√] 自定义背景颜色、图标、文字 <br /> [√] 全屏加载 <br /> [√] 部分加载 |       |
| 消息弹窗     | https://pwawasm.github.io/messagebox  | [√] 常规支持 <br /> [√] Alert弹窗 <br /> [√] Confirm 弹窗 <br /> [√] 无回调 |       |
| 对话框       | https://pwawasm.github.io/dialog      | [√] 常规支持 <br /> [√] 嵌套弹窗 <br /> [√] 指定宽度 <br /> [√] 无回调 |       |
| 日期选择器   | https://pwawasm.github.io/datepicker  | [√] 常规支持 <br /> [√] 指定日期格式 |       |
| Form 表单    | https://pwawasm.github.io/form        | [√] 常规支持 <br /> [√] 三种对齐方式 <br /> [√] 单行表单 |       |
| 布局面板     | https://pwawasm.github.io/layout      | [√] 常规支持 <br /> [√] 嵌套布局 |       |
| 文件上传     |      | |[√] 常规支持 <br /> [√] 限制文件大小 <br /> [√] 限制文件类型 <br /> [√] 图片预览 <br /> [√] 多文件上传
| Markdown 编辑器 |
| 下拉菜单     | https://pwawasm.github.io/dropdown    | [√] 常规支持 |       |
| 穿梭     | https://pwawasm.github.io/transfer    | [√] 常规支持 |       |
| 多语言     | https://pwawasm.github.io/lang    | [√] 常规支持 |    [√] 中英文切换    |

## 项目目录
```
 |-- blazui
    |-- Admin
    |   |-- Admin.ClientRender
    |   |-- Admin.ServerRender
    |   |-- Admin1
    |   |-- Blazui.Admin
    |-- BlazAdmin
    |   |-- BlazAdmin.ServerRender
    |-- Components
    |-- Markdown
    |-- Samples
        |-- BlazAdmin
        |   |-- BlazAdmin.Docs.ClientRender.Server
        |   |-- BlazAdmin.Docs.ServerRender
        |-- Blazui
            |-- Blazui.ClientRender
            |-- Blazui.ClientRender.PWA
            |-- Blazui.ServerRender

```
## 更新日志

### 版本 0.0.7.0，2020.04.24发布
1. 调整 [将 Blazui 基础组件, BlazAdmin, MarkDown Editor 移动到一个仓库][1]
2. 修复 [Table checkbox-SelectedRowsChanged 外部传入EventCallBack后，会使得列表checkbox无法勾选][2]
3. 增加 [Dialog全屏弹窗扩展][3]
4. 增加 [国际化支持(Internationalization support) ][4]
5. 修复 [BSubMenuBase 的 SemaphoreSlim 属性应该是只读属性，同时应该释放][5]
6. 修复 [BPaginationBase在PageSize修改后最大页码不会跟着更新][6]
7. 修复 [BInput InputType=TextArea 渲染无效][7]
8. 增加 [wasm的支持][8]
9. 增加 [BTable增加Attribute忽略项][9]
10. 修复 [MessageService.Show连续使用只显示第一个][10]
11. 修复 [BTable无数据提示][11]
12. 修复 [Dialog叉号关闭窗口报错][12]
13. 修复 [DialogService.ShowDialogAsync 弹出一个窗口关闭后无法紧接着弹出第二个][13]
14. 添加 [BInput控件，当设置的数据类型是数字时，输入字母后，后面的清空按钮失效了][14]

## 感谢

- 测试组件库功能的稳定性和完整度  @deathvicky
- 宣传组件库  https://github.com/zaranetCore  @zaranetCore


  [1]: https://github.com/wzxinchen/Blazui/issues/90
  [2]: https://github.com/wzxinchen/Blazui/issues/87
  [3]: https://github.com/wzxinchen/Blazui/issues/88
  [4]: https://github.com/wzxinchen/Blazui/issues/89
  [5]: https://github.com/wzxinchen/Blazui/issues/85
  [6]: https://github.com/wzxinchen/Blazui/issues/79
  [7]: https://github.com/wzxinchen/Blazui/issues/80
  [8]: https://github.com/wzxinchen/Blazui/issues/83
  [9]: https://github.com/wzxinchen/Blazui/issues/75
  [10]: https://github.com/wzxinchen/Blazui/issues/77
  [11]: https://github.com/wzxinchen/Blazui/issues/78
  [12]: https://github.com/wzxinchen/Blazui/issues/74
  [13]: https://github.com/wzxinchen/Blazui/issues/73
  [14]: https://github.com/wzxinchen/Blazui/issues/71
