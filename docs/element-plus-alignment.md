# Element Plus 设计对齐说明

本文用于指导 Element-Blazor 后续改造。原则是复用现有 Blazor 组件实现，同时把视觉、DOM 约定、交互状态、组件矩阵和文档体验向 Element Plus `2.14.0` 靠齐。

## 基线判断

| 项目 | 结论 |
| --- | --- |
| 目标仓库 | `element-plus/element-plus` |
| 当前最新基线 | `2.14.0` |
| Element-Blazor 版本线 | `2.14.0-alpha.1`，P0 已定稿 |
| .NET 主线 | `src/Components/Element.csproj` 保持 `net10.0`；新增项目使用 .NET 10，历史子模块不阻塞 P1/P2 |
| 官网组件总数 | Basic 12、Configuration 1、Form 25、Data 23、Navigation 9、Feedback 10、Others 2 |
| 历史关系 | Element UI `2.15.x` 是 Vue 2 时代旧线，只作为历史参考，不作为当前目标 |
| 版本策略 | Element-Blazor 进入 `2.14.0-alpha.*` 线，命名与组件矩阵完整对齐后再发布 beta/stable |

## 设计原则

| 原则 | 对组件库的要求 |
| --- | --- |
| 一致 Consistency | 同类组件共享尺寸、颜色、圆角、阴影、图标、文字和状态命名。 |
| 反馈 Feedback | hover、focus、active、loading、disabled、success、warning、danger 状态必须可感知。 |
| 效率 Efficiency | 默认密度适合后台系统，API 简洁，常用场景少写代码。 |
| 可控 Controllability | 每个输入型组件支持受控值、事件回调、禁用、清空、校验和可扩展模板。 |

## 视觉令牌

Element Plus 2.x 使用 `--el-*` CSS 变量作为主题覆盖入口；这和 Element UI 2.x 的 Sass 变量编译产物不同。Element-Blazor 后续应优先建立 CSS 变量兼容层。

| Token | 默认值 | 用途 |
| --- | --- | --- |
| `--el-color-primary` | `#409eff` | 主按钮、选中态、链接、focus 边框 |
| `--el-color-success` | `#67c23a` | 成功提示、成功标签 |
| `--el-color-warning` | `#e6a23c` | 警告提示、警告标签 |
| `--el-color-danger` | `#f56c6c` | 危险操作、错误校验 |
| `--el-color-info` | `#909399` | 次要信息 |
| `--el-text-color-primary` | `#303133` | 重要文本 |
| `--el-text-color-regular` | `#606266` | 正文文本 |
| `--el-text-color-secondary` | `#909399` | 辅助文本 |
| `--el-border-color` | `#dcdfe6` | 默认边框 |
| `--el-border-radius-base` | `4px` | 默认圆角 |
| `--el-component-size` | `32px` | 默认控件高度 |
| `--el-component-size-large` | `40px` | 大尺寸控件高度 |
| `--el-component-size-small` | `24px` | 小尺寸控件高度 |

## 命名策略

| 层级 | 规则 |
| --- | --- |
| DOM/CSS class | 保持 Element Plus 官方 `el-*` 与 `is-*`：例如 `el-button`、`el-input__inner`、`el-table__row`、`is-disabled`。 |
| Razor 公开组件 | 统一使用 PascalCase `El*`：`ElButton`、`ElInput`、`ElTable`。 |
| 旧组件处理 | 直接删除 `Button`、`Input`、`Table` 等旧公开组件名，不兼容、不保留、不做过渡别名。 |
| 内部实现 | `B*` 只允许临时作为内部实现层，后续逐步内聚或重命名。 |
| 不采用方案 | 不采用 `<el-button>` 作为 Blazor 组件，因为 Razor 会把它当普通 custom element；不采用 `el_`，因为不符合 .NET 命名习惯。 |

示例目标：

```razor
<ElButton Type="ButtonType.Primary" Loading="true">保存</ElButton>
```

生成 DOM 仍应保持：

```html
<button class="el-button el-button--primary is-loading">...</button>
```

## 本仓库现状

| 模块 | 观察 |
| --- | --- |
| `src/Components` | 组件主体，已大量使用 `el-*` 类名；存在 `B*` 实现组件和无前缀旧公开组件，需要破坏性命名切换。 |
| `src/Components/wwwroot/css/index.css` | 当前更接近旧 Element UI/theme-chalk 打包样式，需要迁移到 Element Plus CSS 变量兼容层。 |
| `src/Components/wwwroot/css/fix.css` | 项目自定义修补样式，混合布局、表格、Ribbon、表单补丁，需要逐步拆分。 |
| `demo`、`template`、`community` | 作为子模块登记；`community` 用于承载类似 DiscuzX 的社区产品门面。 |
| `test` | Playwright/xUnit 测试依赖现有 `el-*` 选择器，需要保留 DOM 契约。 |

## 改造原则

1. 保持 `el-*` class 选择器稳定，避免破坏测试和下游样式。
2. 优先增加 Element Plus CSS 变量兼容层，再逐组件替换硬编码样式。
3. 公开 API 向 Element Plus 语义靠拢；旧公开组件名直接移除，不建立兼容别名。
4. 输入型组件统一支持 `Value`、`ValueChanged`、`Disabled`/`IsDisabled`、`Size`、`Clearable`、校验状态。
5. 浮层组件统一 z-index、阴影、边框、圆角、关闭策略和键盘行为。
6. 每个组件必须同步 demo、API 文档和测试，避免只改样式。

## P0 交付状态

| 项目 | 状态 | 文档 |
| --- | --- | --- |
| 版本基线 | 🟢 已完成 | [p0-project-baseline.md](p0-project-baseline.md) |
| 文档门面 | 🟢 已完成 | README、README.en、ROADMAP 与本文已同步。 |
| warning 清单 | 🟢 已完成 | [p0-warning-inventory.md](p0-warning-inventory.md) |
| 进度标记约定 | 🟢 已完成 | 已完成项统一使用 `🟢 已完成`。 |

## 第一批要改的文件区域

| 区域 | 动作 |
| --- | --- |
| `src/Components/wwwroot/css` | 新增或整理主题变量文件，逐步弱化旧 `index.css` 的不可维护性。 |
| `src/Components/BButton.*`、旧 `Button*` | 建立 `ElButton`，删除旧 `Button` 公开入口。 |
| `src/Components/BInput.*`、旧 `Input` | 建立 `ElInput`，删除旧 `Input` 公开入口。 |
| `src/Components/BForm*`、旧 `FormItem` | 建立 `ElForm`、`ElFormItem`，删除旧公开入口。 |
| `src/Components/BSelect*`、`BDropDown*`、`BPopup*` | 统一浮层视觉和关闭行为。 |
| `src/Components/BTable*`、`BPagination*` | 统一数据展示密度、空状态、loading、滚动和边框。 |
| `README.md`、`README.en.md`、`CHANGELOG.md` | 保持门面、路线、构建状态同步。 |

## 完成定义

一个组件标记为完成时，需要同时满足：

| 条件 | 要求 |
| --- | --- |
| 视觉 | 默认样式、尺寸、颜色、状态接近 Element Plus 2.14。 |
| DOM | `el-*`、`is-*` class 契约稳定。 |
| 交互 | 点击、键盘、焦点、禁用、加载、清空等行为可验证。 |
| API | Blazor 参数语义清晰，公开入口全部为 `El*`，旧组件名不存在。 |
| 文档 | 有基础示例、状态示例、API 表格。 |
| 测试 | 至少覆盖渲染结构和关键交互。 |

## 下一步建议

P0：版本基线、文档门面和警告清单已经 🟢 已完成。下一步进入 P1：新增 Element Plus CSS 变量层；随后进入 P2：破坏性删除旧公开组件名并建立 `El*` 控件名；第三步从 ElButton、ElInput、ElForm 三个最基础组件开始做视觉和 API 对齐。
