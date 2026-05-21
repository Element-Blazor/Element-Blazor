# Element Plus 对齐路线图

> 目标：让 Element-Blazor 在 Blazor 生态中尽量复刻 Element Plus 的设计语言、组件结构、交互反馈和文档体验。对齐基准为 Element Plus 2.14.0。

## 进度图例

- 🟢 已完成
- 🟡 进行中
- 🔵 待开始
- 🟣 设计确认
- 🟠 需要技术验证
- 🔴 阻塞

## 当前结论

| 项目 | 状态 | 说明 |
| --- | --- | --- |
| 组件库本体 | 🟢 | `dotnet build src/Components/Element.csproj` 可通过，当前有 28 个警告。 |
| 解决方案构建 | 🟢 | `dotnet build Element-Blazor.sln` 可通过，当前有 174 个警告；`community` 作为 Solution Items 入口，不参与主解决方案构建。 |
| Element 类名体系 | 🟢 | 大量组件已使用 `el-*` BEM 类名，具备继续对齐 Element Plus 的基础。 |
| 样式基线 | 🟠 | 当前打包的是旧版 Element UI 风格 CSS，尚未系统引入 Element Plus CSS 变量体系。 |
| 组件覆盖 | 🟡 | 已有 Button、Input、Form、Table、Menu、Tabs、Tree、Upload 等核心组件；缺少多项 Element Plus 2.x 新组件与反馈组件。 |
| 文档门面 | 🟡 | README 已有基础说明；本路线图补齐后，下一步需要可运行文档站与组件总览页。 |
| 社区展示站 | 🟡 | `community` 已作为 Git 子模块接入，用于承载类似 DiscuzX 的 Blazor 社区网站；后续需要升级依赖、统一主题和建立启动脚本。 |

## 设计语言摘要

Element Plus 的语言不是“装饰感”，而是业务后台的可预测性：一致、反馈、效率、可控。落到实现上，需要统一这些层面：

| 层面 | Element Plus 特征 | Element-Blazor 需要跟进 |
| --- | --- | --- |
| 颜色 | 主色 `#409EFF`，成功 `#67C23A`，警告 `#E6A23C`，危险 `#F56C6C`，信息 `#909399`，并提供 light/dark 阶梯。 | 建立 `--el-color-*` CSS 变量，替代散落的硬编码颜色。 |
| 字体 | 14px 基准，12/13/16/18/20 层级，中文后台优先可读性。 | 在全局样式中集中定义字体、字号、行高。 |
| 边框与圆角 | 1px 边框，4px 基础圆角，2px 小圆角，20px round，100% circle。 | 把 Button、Input、Card、Tag、Popover 等圆角统一到变量。 |
| 间距 | 小步长、高密度、可扫描。表单控件默认 32px，高大尺寸 40px，小尺寸 24px。 | 梳理 Size 枚举和组件高度，统一 large/default/small。 |
| 阴影 | overlay 与弹层使用轻量阴影，强调层级不制造厚重感。 | Dialog、Dropdown、Select、Popup、Popover 统一阴影变量。 |
| 交互 | hover、focus、active、disabled、loading 状态清晰。 | 逐组件补齐状态样式和键盘可访问性。 |
| 命名 | BEM 风格 `el-block__element--modifier`，状态类 `is-*`。 | 保留 `el-*` DOM 契约，迁移到 Element Plus 2.x 变量。 |

## 组件覆盖矩阵

| 分类 | Element Plus 组件 | 当前情况 | 优先级 |
| --- | --- | --- | --- |
| Basic | Button、Container、Icon、Layout | 🟡 已有 Button、Icon、Layout；缺 Link、Text、Scrollbar、Space、Typography、Splitter | P1/P3 |
| Config | Config Provider | 🔵 缺全局配置组件 | P1 |
| Form | Form、Input、Radio、Checkbox、Select、Switch、DatePicker、Transfer、Upload | 🟡 已有核心组件；缺 Autocomplete、Cascader、InputNumber、Slider、Rate、TimePicker、TreeSelect、ColorPicker 等 | P1/P2 |
| Data | Table、Tag、Badge、Card、Tree、Pagination | 🟡 已有核心组件；缺 Avatar、Calendar、Carousel、Collapse、Descriptions、Empty、Image、Progress、Result、Skeleton、Timeline、Statistic、Segmented 等 | P2 |
| Navigation | Menu、Tabs、Breadcrumb、Dropdown、Steps | 🟡 已有大部分；Steps 当前被项目排除；缺 Affix、Anchor、Backtop、PageHeader | P1/P2 |
| Feedback | Loading、Message、MessageBox、Dialog | 🟡 有服务和弹层基础；缺 Alert、Drawer、Notification、Popconfirm、Popover、Tooltip 的完整组件面 | P1/P2 |
| Others | Divider、Watermark | 🔵 缺 | P3 |

## 阶段规划

### P0 门面与基线 🟡

目标：先让项目“看起来可信、跑得起来、知道往哪走”。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 新增路线图和 Element Plus 对齐说明 | 🟡 | `ROADMAP.md`、`docs/element-plus-alignment.md` 完整存在。 |
| 修正 README 的目标、构建和子模块说明 | 🟢 | 新用户能区分组件库本体构建和完整解决方案构建。 |
| 纳入 `community` 子模块 | 🟢 | `.gitmodules` 包含 `community`，解决方案中提供社区入口。 |
| 初始化/修复 demo 与 template 子模块 | 🟢 | `dotnet build Element-Blazor.sln` 不再因项目缺失失败。 |
| 建立警告清单 | 🔵 | NuGet 漏洞、过期依赖、Blazor analyzer 警告可追踪。 |

### P1 设计令牌与核心组件 🔵

目标：把 Element Plus 的视觉根基变成我们自己的稳定基础。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 引入 Element Plus 2.x CSS 变量层 | 🔵 | `--el-color-*`、`--el-font-size-*`、`--el-border-*`、`--el-box-shadow-*` 可用。 |
| Button/Input/Form/Select/Radio/Checkbox/Switch 样式对齐 | 🔵 | 默认、尺寸、禁用、加载、聚焦、错误状态与 Element Plus 接近。 |
| Table/Pagination/Tag/Card/Tabs/Menu/Breadcrumb 对齐 | 🔵 | DOM 契约不破坏现有测试，视觉与交互统一到变量。 |
| Feedback 基础补齐 | 🔵 | Alert、Tooltip、Popover、Notification 至少具备基础 API 和 demo。 |

### P2 文档站与组件总览 🔵

目标：复刻 Element Plus 文档的信息架构，先让开发者能系统理解组件。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 建立 Blazor 文档站门面 | 🔵 | 顶部导航、侧栏分类、组件总览、搜索入口、版本标签。 |
| 为 P1 组件补 demo | 🔵 | 每个组件有基础、禁用、尺寸、状态、事件示例。 |
| 增加 API 表格模板 | 🔵 | 属性、事件、插槽/RenderFragment、方法、类型定义结构统一。 |
| 增加视觉回归截图 | 🔵 | 至少覆盖 Button、Input、Select、Table、Dialog、Tabs。 |

### P3 社区门面与真实业务示例 🔵

目标：把 `community` 打造成类似 DiscuzX 的展示站，让 Element-Blazor 不只是一组控件，而能展示完整社区产品体验。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 社区项目基线审计 | 🔵 | 明确 `BlazorCommunity.App`、`Admin`、`Api`、`WasmApp` 的启动方式、依赖版本、数据库和配置。 |
| 依赖与目标框架升级方案 | 🔵 | 从 .NET Core 3.1 / netstandard2.1 迁移到当前仓库可维护的目标框架，列出风险。 |
| Element Plus 主题融合 | 🔵 | 社区前台、后台、移动端统一使用 Element-Blazor 新主题变量。 |
| DiscuzX 类信息架构 | 🔵 | 首页、版块、主题列表、帖子详情、回复、用户中心、通知、管理后台流程完整。 |
| 本地一键启动 | 🔵 | 提供开发启动脚本和 README，说明 API、前台、后台、数据库初始化顺序。 |

### P4 组件扩展 🔵

目标：覆盖 Element Plus 组件总览中的中高频缺口。

| 分类 | 任务 |
| --- | --- |
| Basic | Link、Text、Scrollbar、Space、Typography、Splitter |
| Form | InputNumber、Slider、Rate、TimePicker、TreeSelect、Cascader、Autocomplete |
| Data | Empty、Progress、Result、Skeleton、Avatar、Descriptions、Collapse、Timeline、Segmented |
| Navigation | Affix、Anchor、Backtop、PageHeader、Steps 恢复 |
| Feedback | Drawer、Popconfirm、Popover、Tooltip、Notification |

### P5 稳定化与发布 🔵

目标：把组件库从“能用”推进到“可持续发布”。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 依赖升级和安全修复 | 🔵 | 消除已知 NuGet 安全警告。 |
| 测试矩阵 | 🔵 | 单元测试、Playwright 交互测试、视觉截图测试分层。 |
| NuGet 包门面 | 🔵 | README、icon、license、symbols、source link 完整。 |
| 版本策略 | 🔵 | 每个阶段发布 alpha/beta/stable，CHANGELOG 可追踪。 |

## 执行顺序

1. 🟡 P0-1：完成路线图、设计对齐说明、README 门面。
2. 🔵 P0-2：修复子模块或提供本地最小 demo，恢复解决方案构建。
3. 🔵 P0-3：整理构建警告和依赖安全清单。
4. 🔵 P1-1：建立 Element Plus 2.x CSS 变量层。
5. 🔵 P1-2：从 Button、Input、Form 开始统一尺寸/状态。
6. 🔵 P1-3：迁移 Select、Dropdown、Popup 这类浮层组件。
7. 🔵 P1-4：迁移 Table、Pagination、Tabs、Menu 这类高曝光组件。
8. 🔵 P2-1：建设组件总览和文档站模板。
9. 🔵 P3-1：审计并升级 `community`，形成 DiscuzX 类社区展示站。
10. 🔵 P4：按缺口矩阵补组件。
11. 🔵 P5：测试、发布、稳定化。

## 任务提示词

下面提示词可直接用于后续逐步开工。

### P0-2 子模块与构建

```text
请检查 Element-Blazor 仓库的 demo、template 与 community 子模块状态，恢复或替代缺失项目，使 `dotnet build Element-Blazor.sln` 不再因项目文件缺失失败。不要改动组件实现；若必须调整解决方案，请说明原因并保留组件库本体可独立构建。
```

### P0-3 警告清单

```text
请运行 `dotnet build src/Components/Element.csproj`，整理所有 warning，按 NuGet 安全、过期依赖、C# 编译、Blazor analyzer、文档注释分类，创建可执行修复清单。先不要大规模升级依赖，只给出最小风险修复顺序。
```

### P1-1 CSS 变量层

```text
请以 Element Plus 2.14.0 的 `:root --el-*` 设计令牌为基准，在 Element-Blazor 中新增或整理主题变量层。要求不破坏现有 `el-*` 类名和测试选择器，把颜色、字体、圆角、边框、阴影、组件尺寸集中到可覆盖的 CSS 变量，并更新 README 的引用说明。
```

### P1-2 Button/Input/Form

```text
请对齐 Element Plus 的 Button、Input、Form 视觉和交互。检查 Razor DOM、参数 API、CSS class、disabled/loading/focus/error/size 状态。保持现有公开 API 兼容，必要时新增别名参数，并为每个组件补最小示例和测试。
```

### P1-3 Select/Dropdown/Popup

```text
请对齐 Element Plus 的 Select、Dropdown、Popup 浮层行为。重点检查弹层定位、z-index、阴影、hover/selected/disabled 状态、键盘可访问性、点击外部关闭、清空按钮和 loading 状态。不要引入与现有 ElementJS 冲突的定位方案。
```

### P1-4 Table/Pagination/Tabs/Menu

```text
请对齐 Element Plus 的 Table、Pagination、Tabs、Menu。保留现有 Playwright 选择器可用，优先修复视觉密度、边框、hover、active、empty、loading、禁用和滚动行为。每个组件至少补一个基础示例和一个状态示例。
```

### P2 文档站

```text
请为 Element-Blazor 建立 Element Plus 风格的文档站骨架：顶部导航、版本标签、左侧组件分类、组件总览、示例卡片、代码区、API 表格。第一屏直接展示组件总览，不做营销页。文档站要能本地运行，并给出启动命令。
```

### P3 社区展示站

```text
请审计 `community` 子模块，把它规划为类似 DiscuzX 的 Element-Blazor 社区展示站。先梳理前台、后台、API、WASM、数据库、登录、发帖、回帖、用户中心和管理后台的启动链路；再给出 .NET 版本升级、Element Plus 主题融合、本地一键启动和展示页改造的分阶段任务。不要直接大规模重写，先建立可运行基线。
```

### P4 新组件

```text
请从路线图 P4 的组件缺口中选择一个中等复杂度组件实现。先比对 Element Plus API，再设计 Blazor 参数、事件和 RenderFragment 扩展点；实现后补 demo、API 文档和测试。保持 `el-*` BEM 类名一致。
```

### P5 发布稳定化

```text
请为 Element-Blazor 做发布稳定化：升级存在安全警告的依赖，检查 NuGet 包元数据、README、CHANGELOG、license、icon、symbols/source link，建立发布前 checklist，并验证组件库本体和完整解决方案构建。
```

## 参考来源

- Element Plus 组件总览：https://element-plus.org/zh-CN/component/overview
- Element Plus 设计原则：https://element-plus.org/zh-CN/guide/design.html
- Element Plus 主题说明：https://element-plus.org/zh-CN/guide/theming.html
- Element Plus 2.14.0 CSS 包：https://unpkg.com/element-plus@2.14.0/dist/index.css
