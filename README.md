# Blazui
---

## 介绍
Element的blazor版本

API 模仿 Element，CSS 直接使用 Element的样式，HTML 结构直接使用 Element 的 HTML 结构

Element 地址：https://element.eleme.cn/#/zh-CN/component/layout

Blazui 演示地址：http://blazui.com:9000

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

## 案例展示

地址：http://wandotnet.top/quartz
用户名密码：blazor,666666
作者：deathvicky

![image.png-58.1kB][1]

类似于LayAdmin的后台管理模板 https://github.com/wzxinchen/BlazAdmin

## 组件列表


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
| Markdown 编辑器 |
| 下拉菜单     | http://blazui.com:9000/dropdown    | [√] 常规支持 |       |
## 更新日志

### 版本 0.0.5.24，2020.01.02发布
1. 新增 [BDropDown 下拉菜单组件][2]
2. 修复 [Table控件中 日期的格式化不起作用][3]
3. 修复 [table 全选][4]
4. 增加 [Input组件可否添加尺寸（Size）][5]
5. 修复 [下拉弹出菜单频繁点击会出现多个][6]
6. 修复 [按钮不支持圆形][7]
7. 修复 [弹窗组件][8]
8. 修复 [BButton组件中Cls属性改为追加感觉更合理][9]
9. 修复 [\[Bug Report\] Checkbox 全选时选项没联动选中][10]
10. 添加 [about loading][11]
11. 添加 [是否可以让BMenuItem组件具有匹配路由然后选中的功能][12]
12. 修复 [表单下拉列表][13]
13. 添加 [组件可增加一个visible属性，用于隐藏或显示][14]
14. 添加 [弹窗-表单赋值的时候radio组件-枚举值无法绑定][15]

## 感谢

- 测试组件库功能的稳定性和完整度  @deathvicky
- 宣传组件库  https://github.com/zaranetCore  @zaranetCore


  [1]: http://static.zybuluo.com/wzxinchen/wt0rvk0k7eft66kfhu1zv89p/image.png
  [2]: http://blazui.com:9000/dropdown
  [3]: https://github.com/wzxinchen/Blazui/issues/47
  [4]: https://github.com/wzxinchen/Blazui/issues/46
  [5]: https://github.com/wzxinchen/Blazui/issues/45
  [6]: https://github.com/wzxinchen/Blazui/issues/44
  [7]: https://github.com/wzxinchen/Blazui/issues/42
  [8]: https://github.com/wzxinchen/Blazui/issues/41
  [9]: https://github.com/wzxinchen/Blazui/issues/40
  [10]: https://github.com/wzxinchen/Blazui/issues/38
  [11]: https://github.com/wzxinchen/Blazui/issues/31
  [12]: https://github.com/wzxinchen/Blazui/issues/30
  [13]: https://github.com/wzxinchen/Blazui/issues/28
  [14]: https://github.com/wzxinchen/Blazui/issues/14
  [15]: https://github.com/wzxinchen/Blazui/issues/13
