# Element Plus 设计对齐说明

本文用于约束 Element-Blazor 后续改造。当前主线对齐 Element Plus `2.14.0`，旧 Element UI 仅作为历史参考。

## 基线判断

| 项目 | 结论 |
| --- | --- |
| 目标仓库 | `element-plus/element-plus` |
| 当前基线 | `2.14.0` |
| Element-Blazor 版本线 | `2.14.0-alpha.1` |
| .NET 主线 | `src/Components/Element.csproj` 使用 `net10.0` |
| 历史关系 | Element UI `2.15.x` 不再作为目标路线 |

## 设计原则

| 原则 | 要求 |
| --- | --- |
| 一致性 | 同类组件共享尺寸、颜色、圆角、阴影、图标、文字和状态命名。 |
| 反馈 | hover、focus、active、loading、disabled、success、warning、danger 状态必须可感知。 |
| 效率 | 默认密度适合后台系统，API 简洁，常用场景少写代码。 |
| 可控 | 输入型组件支持受控值、事件回调、禁用、清空、校验和扩展模板。 |

## 命名策略

| 层级 | 规则 |
| --- | --- |
| DOM/CSS class | 保持 Element Plus 官方 `el-*` 与 `is-*`，例如 `el-button`、`el-input__inner`、`el-table__row`、`is-disabled`。 |
| Razor 公开组件 | 统一使用 PascalCase `El*`，例如 `ElButton`、`ElInput`、`ElTable`。 |
| 基础类型 | 统一使用 `Element*`，例如 `ElementComponentBase`、`ElementDialogBase`、`ElementException`。 |
| 旧组件处理 | 旧组件实现和旧包入口直接删除，不兼容、不保留、不做过渡别名。 |
| 不采用方案 | 不使用 `<el-button>` 作为 Blazor 组件；不使用 `el_` 命名。 |

示例目标：

```razor
<ElButton Type="ButtonType.Primary" Loading="true">保存</ElButton>
```

生成 DOM 仍应保持：

```html
<button class="el-button el-button--primary is-loading">...</button>
```

## 仓库现状

| 模块 | 观察 |
| --- | --- |
| `src/Components` | 活跃组件入口已统一到 `El*` / `Element*`。 |
| `src/Markdown` | Markdown 组件入口已统一到 `ElMarkdown*` / `Element.Markdown`。 |
| `demo`、`template`、`community` | 活跃 Razor 标签迁移为 `El*`。 |
| `test` | 后续测试应继续以稳定 `el-*` DOM 契约为依据。 |

## 完成定义

| 条件 | 要求 |
| --- | --- |
| 视觉 | 默认样式、尺寸、颜色、状态接近 Element Plus 2.14。 |
| DOM | `el-*`、`is-*` class 契约稳定。 |
| 交互 | 点击、键盘、焦点、禁用、加载、清空等行为可验证。 |
| API | 公开入口全部为 `El*` / `Element*`，旧组件名不存在。 |
| 文档 | 示例使用 `El*`，不展示旧组件写法。 |
| 测试 | 覆盖关键渲染结构和交互路径。 |

