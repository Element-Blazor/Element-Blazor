# Element Plus 设计对齐说明

本文用于指导 Element-Blazor 后续改造。原则是复用现有 Blazor 组件实现，同时把视觉、DOM 约定、交互状态和文档体验向 Element Plus 2.14.0 靠齐。

## 设计原则

| 原则 | 对组件库的要求 |
| --- | --- |
| 一致 Consistency | 同类组件共享尺寸、颜色、圆角、阴影、图标、文字和状态命名。 |
| 反馈 Feedback | hover、focus、active、loading、disabled、success、warning、danger 状态必须可感知。 |
| 效率 Efficiency | 默认密度适合后台系统，API 简洁，常用场景少写代码。 |
| 可控 Controllability | 每个输入型组件支持受控值、事件回调、禁用、清空、校验和可扩展模板。 |

## 视觉令牌

| Token | Element Plus 默认值 | 用途 |
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

## 本仓库现状

| 模块 | 观察 |
| --- | --- |
| `src/Components` | 组件主体，已大量使用 `el-*` 类名；存在 `B*` 实现组件和无前缀兼容组件。 |
| `src/Components/wwwroot/css/index.css` | 当前为压缩后的旧 Element 样式，缺少 Element Plus 2.x 变量化结构。 |
| `src/Components/wwwroot/css/fix.css` | 项目自定义修补样式，混合布局、表格、Ribbon、表单补丁。 |
| `demo`、`template`、`community` | 作为子模块登记；`community` 用于承载类似 DiscuzX 的社区产品门面。 |
| `test` | Playwright/xUnit 测试依赖现有 `el-*` 选择器，需要保留 DOM 契约。 |

## 改造原则

1. 保持 `el-*` class 选择器稳定，避免破坏测试和下游样式。
2. 优先增加 CSS 变量和兼容层，再逐组件替换硬编码样式。
3. 公开 API 尽量向 Element Plus 语义靠拢，但保留现有参数作为兼容别名。
4. 输入型组件统一支持 `Value`、`ValueChanged`、`IsDisabled`、`Size`、`Clearable`、校验状态。
5. 浮层组件统一 z-index、阴影、边框、圆角、关闭策略和键盘行为。
6. 每个组件必须同步 demo、API 文档和测试，避免只改样式。

## 第一批要改的文件区域

| 区域 | 动作 |
| --- | --- |
| `src/Components/wwwroot/css` | 新增或整理主题变量文件，逐步弱化旧 `index.css` 的不可维护性。 |
| `src/Components/BButton.*`、`Button*.cs` | 统一按钮尺寸、状态、loading、plain、round、circle。 |
| `src/Components/BInput.*` | 统一高度、前后缀、清空、disabled、textarea、自适应状态。 |
| `src/Components/BForm*` | 统一 label 对齐、校验提示、inline、disabled 继承。 |
| `src/Components/BSelect*`、`BDropDown*`、`BPopup*` | 统一浮层视觉和关闭行为。 |
| `src/Components/BTable*`、`BPagination*` | 统一数据展示密度、空状态、loading、滚动和边框。 |
| `README.md`、`README.en.md`、`CHANGELOG.md` | 保持门面、路线、构建状态同步。 |

## 完成定义

一个组件标记为完成时，需要同时满足：

| 条件 | 要求 |
| --- | --- |
| 视觉 | 默认样式、尺寸、颜色、状态接近 Element Plus。 |
| 交互 | 点击、键盘、焦点、禁用、加载、清空等行为可验证。 |
| API | Blazor 参数语义清晰，兼容历史用法。 |
| 文档 | 有基础示例、状态示例、API 表格。 |
| 测试 | 至少覆盖渲染结构和关键交互。 |

## 下一步建议

先做 P0：修复子模块和构建基线。随后做 P1：引入 CSS 变量层，再从 Button/Input/Form 三个最基础组件开始。这个顺序能最快让项目门面和实际体验一起变稳。
