# Element Plus 2.14 对齐路线图

> 最终决策：Element-Blazor 主线切换到 [element-plus/element-plus](https://github.com/element-plus/element-plus)，当前基线为 Element Plus `2.14.0`。旧 Element UI `2.15.x` 只作为历史参考。公开控件名称全部抛弃旧名，统一采用 Element Plus 风格 `El*`；旧 `Button/Input/Table` 等公开组件名直接删除，不兼容、不保留、不做过渡。

## 进度图例

- 🟢 已完成
- 🟡 进行中
- 🔵 待开始
- 🟣 设计确认
- 🟠 需要技术验证
- 🔴 阻塞

文档约定：所有文档中代表完成状态的任务，统一使用 `🟢 已完成`；不要只写“完成”或只放无说明图标。`任务提示词` 只保留未执行任务；完成一个任务提示词，就从本节删除一个。

## 当前结论

| 项目 | 状态 | 说明 |
| --- | --- | --- |
| 官方基线 | 🟢 已完成 | npm `element-plus` latest 当前为 `2.14.0`，项目以它作为视觉、组件矩阵和版本基线。 |
| 版本线 | 🟢 已完成 | 组件库与 Element 相关包进入 `2.14.0-alpha.1`，后续按 alpha/beta/stable 推进。 |
| DOM class | 🟢 已完成 | 必须保持官方 `el-*` 与 `is-*`：例如 `el-button`、`el-input__inner`、`el-table__row`。 |
| Razor 控件名 | 🟣 | 公开控件统一为 `ElButton`、`ElInput`、`ElTable`。不采用 `<el-button>`，因为 Razor 会把它当普通 custom element。 |
| 旧公开组件 | 🔴 | `Button`、`Input`、`Table` 等旧公开名必须删除；demo/template/community 同步迁移。 |
| 内部实现 | 🟡 | `B*` 可短期作为内部实现层，但不再作为公开路线；后续逐步内聚或重命名。 |
| 样式基线 | 🟡 | 当前样式更接近旧 Element UI theme-chalk，需要迁移到 Element Plus 2.14 CSS 变量体系。 |
| 子模块 | 🟢 已完成 | `demo`、`template`、`community` 已作为 Git 子模块存在；`community` 走彻底重构主线，旧项目只作为业务素材库。 |
| 构建基线 | 🟢 已完成 | `dotnet build src/Components/Element.csproj` 可通过；当前 build 汇总打印 28 个 warning，去重后有 18 个构建 warning。 |
| .NET 主线 | 🟢 已完成 | `src/Components/Element.csproj` 保持 `net10.0`；新增项目使用 .NET 10，历史子模块不做原地全面升级。 |

## 命名规则

| 层级 | 最终规则 | 处理方式 |
| --- | --- | --- |
| DOM/CSS | `el-*`、`is-*` | 完全保留 Element Plus 官方 class 契约。 |
| Blazor 控件 | `El*` PascalCase | `ElButton`、`ElInput`、`ElTable`、`ElFormItem`。 |
| 旧无前缀控件 | 删除 | 删除 `Button.razor`、`Input.razor`、`Table.razor` 等旧入口。 |
| `B*` 控件 | 内部化 | 第一阶段可复用实现，第二阶段改 internal/重命名/合并到 `El*`。 |
| `el_` | 不采用 | 不符合 .NET/Razor 命名习惯。 |
| `<el-button>` | 不采用 | Razor 不会把连字符标签解析为 C# 组件类型。 |

目标写法：

```razor
<ElButton Type="ButtonType.Primary" Loading="true">保存</ElButton>
<ElInput TValue="string" Placeholder="请输入" Clearable="true" />
<ElTable DataSource="@rows" />
```

生成 DOM：

```html
<button class="el-button el-button--primary is-loading">...</button>
```

## Element Plus 2.14 组件矩阵

| 分类 | Element Plus 组件 | 当前状态 | 优先级 |
| --- | --- | --- | --- |
| Basic | Button、Border、Color、Container、Icon、Layout、Link、Text、Scrollbar、Space、Splitter、Typography | 🟡 已有 Button/Icon/Layout 基础；缺 Link、Text、Scrollbar、Space、Splitter、Typography；Border/Color 属文档与令牌页 | P1/P5 |
| Configuration | Config Provider | 🔵 缺全局配置组件 | P1 |
| Form | Autocomplete、Cascader、Checkbox、Color Picker Panel、Color Picker、Date Picker Panel、Date Picker、DateTime Picker、Form、Input、Input Number、Input Tag、Input OTP、Mention、Radio、Rate、Select、Virtualized Select、Slider、Switch、Time Picker、Time Select、Transfer、TreeSelect、Upload | 🟡 已有 Checkbox、DatePicker、Form、Input、Radio、Select、Switch、Transfer、Upload；缺多数新组件与虚拟化组件 | P1/P5 |
| Data | Avatar、Badge、Calendar、Card、Carousel、Collapse、Descriptions、Empty、Image、Infinite Scroll、Pagination、Progress、Result、Skeleton、Table、Virtualized Table、Tag、Timeline、Tour、Tree、Virtualized Tree、Statistic、Segmented | 🟡 已有 Badge、Card、Pagination、Table、Tag、Tree；缺 Empty、Progress、Skeleton、Virtualized Table/Tree 等 | P2/P5 |
| Navigation | Affix、Anchor、Backtop、Breadcrumb、Dropdown、Menu、Page Header、Steps、Tabs | 🟡 已有 Breadcrumb、Dropdown、Menu、Tabs；Steps 当前被排除；缺 Affix、Anchor、Backtop、Page Header | P2/P5 |
| Feedback | Alert、Dialog、Drawer、Loading、Message、Message Box、Notification、Popconfirm、Popover、Tooltip | 🟡 有 Dialog、Loading、Message、MessageBox 基础；缺 Alert、Drawer、Notification、Popconfirm、Popover、Tooltip 完整组件面 | P1/P2 |
| Others | Divider、Watermark | 🔵 缺 | P5 |

## 阶段路线

### P0 门面与版本基线 🟢 已完成

目标：把项目方向、版本线、子模块和构建状态定准。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 切换目标为 `element-plus/element-plus` | 🟢 已完成 | README、路线图、对齐文档均写明 Element Plus 2.14。 |
| 设置 `2.14.0-alpha.1` 版本线 | 🟢 已完成 | `Element.csproj` 与 Element Markdown 包进入 2.14 alpha。 |
| 确认 .NET 10 主线 | 🟢 已完成 | 组件库主项目保持 `net10.0`；新增项目以 .NET 10 为基线，历史子模块不阻塞 P1/P2。 |
| 纳入 `community` 子模块 | 🟢 已完成 | `community` 与 `demo`、`template` 同级作为 Git 子模块。 |
| 验证组件库构建 | 🟢 已完成 | `dotnet build src/Components/Element.csproj` 成功。 |
| 建立 warning 清单 | 🟢 已完成 | 已按 NuGet 安全、过期依赖、C# 编译、Blazor analyzer、文档注释分类，见 [docs/p0-warning-inventory.md](docs/p0-warning-inventory.md)。 |

### P1 Element Plus 主题与设计令牌 🔵

目标：先建立统一视觉地基，再迁移组件。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 新增 Element Plus CSS 变量层 | 🔵 | `--el-color-*`、`--el-font-size-*`、`--el-border-*`、`--el-box-shadow-*`、`--el-component-size-*` 可覆盖。 |
| 梳理旧 theme-chalk 与 fix.css | 🔵 | 明确哪些样式保留、迁移、删除；避免继续堆补丁。 |
| 建立基础 token demo | 🔵 | 文档站能展示颜色、字体、边框、阴影、尺寸。 |
| 浮层层级规范 | 🔵 | Dialog、Dropdown、Select、Popover、Tooltip 的 z-index 与阴影统一。 |

### P2 破坏性控件命名切换 🔵

目标：旧公开控件名直接删除，公开 API 完全切到 Element Plus 风格 `El*`。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 建立核心 `El*` 控件 | 🔵 | `ElButton`、`ElInput`、`ElForm`、`ElFormItem`、`ElSelect`、`ElOption`、`ElTable`、`ElTableColumn` 可用。 |
| 删除旧无前缀入口 | 🔵 | 删除 `Button.razor`、`Input.razor`、`Table.razor` 等旧公开组件。 |
| 迁移内部引用 | 🔵 | 组件库内部不再引用旧公开组件名。 |
| 迁移 demo/template | 🔵 | 示例与模板改用 `El*`。 |
| 迁移 community | 🔵 | 社区展示站改用 `El*`，并作为真实业务验证面。 |
| 更新文档示例 | 🔵 | 文档所有示例只出现 `El*`，不出现旧公开控件名。 |

### P3 核心组件视觉与 API 对齐 🔵

目标：先把最高频控件做成 Element Plus 2.14 体验。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| ElButton | 🔵 | type、size、plain、round、circle、loading、disabled、icon 与 Element Plus 对齐。 |
| ElInput | 🔵 | clearable、prefix/suffix、textarea、disabled、size、formatter、focus/blur 状态对齐。 |
| ElForm/ElFormItem | 🔵 | label-position、inline、rules、error、required、disabled 继承对齐。 |
| ElSelect/ElOption | 🔵 | clearable、filterable、multiple、disabled、loading、empty、keyboard 行为对齐。 |
| ElTable/ElTableColumn | 🔵 | border、stripe、fixed、selection、empty、loading、sort、filter、分页配合对齐。 |

### P4 文档站与组件总览 🔵

目标：形成类似 Element Plus 官网的组件入口，第一屏直接是组件总览，不做营销页。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 文档站骨架 | 🔵 | 顶部导航、版本标签、左侧分类、组件总览、搜索入口。 |
| 示例卡片 | 🔵 | 每个组件有基础、禁用、尺寸、状态、事件示例。 |
| API 表格 | 🔵 | 属性、事件、RenderFragment、方法、类型定义结构统一。 |
| 视觉回归 | 🔵 | 至少覆盖 ElButton、ElInput、ElSelect、ElTable、ElDialog、ElTabs。 |

### P5 组件矩阵补齐 🔵

目标：按 Element Plus 2.14 组件矩阵补齐中高频缺口。

| 分类 | 第一批任务 |
| --- | --- |
| Basic | ElLink、ElText、ElScrollbar、ElSpace、ElSplitter、ElTypography |
| Form | ElInputNumber、ElSlider、ElRate、ElTimePicker、ElAutocomplete、ElCascader、ElColorPicker |
| Data | ElEmpty、ElProgress、ElResult、ElSkeleton、ElAvatar、ElDescriptions、ElCollapse、ElTimeline、ElStatistic、ElSegmented |
| Navigation | ElAffix、ElAnchor、ElBacktop、ElPageHeader、ElSteps |
| Feedback | ElAlert、ElDrawer、ElPopconfirm、ElPopover、ElTooltip、ElNotification |
| Others | ElDivider、ElWatermark |

### P6 社区展示站 🔵

目标：把 `community` 按 [community-rebuild-plan.md](docs/community-rebuild-plan.md) 重写成类似 DiscuzX 的真实业务展示站，不保留旧 UI、旧组件名、旧包名、旧启动方式。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 社区基线审计 | 🟢 已完成 | 已明确旧 `BlazorCommunity.*` 为 .NET Core 3.1/netstandard2.1 时代项目，只作为业务参考。 |
| 新主线骨架 | 🔵 | 新建 `ElementCommunity.*`，先跑通前台主站、主题详情、发帖页。 |
| 数据与账号 | 🔵 | 新建 DbContext、Identity、种子数据，跑通登录、发帖、回帖。 |
| Element Plus 主题融合 | 🔵 | 前台、后台、移动端统一使用 Element-Blazor 2.14 主题和 `El*` 控件。 |
| DiscuzX 信息架构 | 🔵 | 首页、版块、主题列表、帖子详情、回复、用户中心、通知、管理后台完整。 |
| 本地一键启动 | 🔵 | 提供启动脚本和说明，能本地跑通核心流程。 |

### P7 稳定化与发布 🔵

目标：从 alpha 推进到可持续发布。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 依赖安全修复 | 🔵 | 处理 `Microsoft.AspNetCore.Components 7.0.2` 等已知安全 warning。 |
| NuGet 包门面 | 🔵 | README、icon、license、symbols、source link、package readme 完整。 |
| 测试矩阵 | 🔵 | 单元测试、Playwright 交互测试、视觉截图测试分层。 |
| 发布节奏 | 🔵 | `2.14.0-alpha.*`、`beta`、`stable` 每阶段有明确验收。 |

## 执行顺序

1. 🟢 已完成 P0-1：路线图、README、对齐文档、版本线定稿。
2. 🟢 已完成 P0-2：整理 warning 清单和依赖安全清单。
3. 🔵 P1-1：建立 Element Plus 2.14 CSS 变量层。
4. 🔵 P2-1：新增核心 `El*` 控件入口。
5. 🔵 P2-2：删除旧无前缀公开组件入口。
6. 🔵 P2-3：批量迁移 demo/template/community 到 `El*`。
7. 🔵 P3-1：对齐 ElButton、ElInput、ElForm。
8. 🔵 P3-2：对齐 ElSelect、ElTable、ElDialog、ElTabs、ElMenu。
9. 🔵 P4-1：建设组件总览和文档站模板。
10. 🔵 P5：按组件矩阵补齐缺口。
11. 🔵 P6：按重构蓝图新建社区展示站主线，旧 `BlazorCommunity.*` 只读参考。
12. 🔵 P7：测试、NuGet、发布稳定化。

## 任务提示词

### P1-1 CSS 变量层

```text
请以 Element Plus 2.14.0 的设计令牌和 `--el-*` CSS 变量为基准，在 Element-Blazor 中新增主题变量层。要求不破坏现有 `el-*` DOM class，把颜色、字体、圆角、边框、阴影、组件尺寸集中到可覆盖的 CSS 变量，并给出 Button/Input/Table 的迁移示例。
```

### P2-1 控件命名破坏性切换

```text
请把 Element-Blazor 的公开 Razor 控件名切换为 Element Plus 风格 `El*`。新增 `ElButton`、`ElInput`、`ElForm`、`ElFormItem`、`ElSelect`、`ElOption`、`ElTable`、`ElTableColumn` 等核心入口；删除旧公开组件 `Button`、`Input`、`Table`、`Select` 等无前缀入口，不保留兼容别名。确保组件库本体构建通过，并列出 demo/template/community 需要迁移的引用。
```

### P2-3 示例迁移

```text
请批量迁移 demo、template、community 中的旧 Element-Blazor 组件名到新的 `El*` 控件名。不要保留旧写法；迁移后运行组件库构建和能覆盖到的示例构建，列出暂时无法构建的项目及原因。
```

### P3-1 ElButton/ElInput/ElForm

```text
请对齐 Element Plus 2.14 的 ElButton、ElInput、ElForm 视觉和 API。检查 Razor DOM、参数命名、CSS class、disabled/loading/focus/error/size 状态。公开示例只能使用 `El*`，旧组件名不得出现。
```

### P3-2 高曝光组件

```text
请对齐 Element Plus 2.14 的 ElSelect、ElTable、ElDialog、ElTabs、ElMenu。重点检查浮层定位、z-index、键盘行为、empty/loading/disabled/active 状态、表格密度和滚动行为。公开组件名必须为 `El*`。
```

### P4 文档站

```text
请为 Element-Blazor 建立 Element Plus 风格的文档站骨架：顶部导航、版本标签、左侧组件分类、组件总览、示例卡片、代码区、API 表格。第一屏直接展示组件总览，不做营销页。所有示例统一使用 `El*` 控件名。
```

### P6 社区展示站

```text
请按 `docs/community-rebuild-plan.md` 在 community 内建立新的 ElementCommunity 主线。旧 BlazorCommunity 项目只作为业务素材参考，不保留旧 UI、旧组件名、旧包名、旧启动方式。第一步建立可运行 Host、Domain、Infrastructure、Components 骨架，页面只使用 `El*` 控件，先做首页、帖子详情、发帖页静态骨架。
```

### P7 发布稳定化

```text
请为 Element-Blazor 2.14 做发布稳定化：处理安全 warning，检查 NuGet 包元数据、README、CHANGELOG、license、icon、package readme、symbols/source link，建立发布前 checklist，并验证组件库本体和完整解决方案构建。
```

## 参考来源

- Element Plus 仓库：https://github.com/element-plus/element-plus
- Element Plus 组件总览：https://element-plus.org/zh-CN/component/overview
- Element Plus Changelog：https://element-plus.org/en-US/guide/changelog
- Element Plus Release：https://github.com/element-plus/element-plus/releases
- Element Plus 2.14.0 CSS 包：https://unpkg.com/element-plus@2.14.0/dist/index.css
