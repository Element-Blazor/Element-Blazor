# Element Plus 2.14 对齐路线图

> 最终决策：Element-Blazor 主线切换到 [element-plus/element-plus](https://github.com/element-plus/element-plus)，当前基线为 Element Plus `2.14.0`。旧 Element UI `2.15.x` 只作为历史参考。公开控件名称统一采用 Element Plus 风格 `El*`；旧 `Button/Input/Table` 等无前缀公开组件名不兼容、不保留、不做过渡。

## 进度图例

- 🟢 已完成
- 🟡 进行中
- 🔵 待开始
- 🟣 设计确认
- 🟠 需要技术验证
- 🔴 阻塞

文档约定：所有文档中代表完成状态的任务，统一使用 `🟢 已完成`。`任务提示词` 只保留未执行任务；完成一个任务提示词，就从本节删除一个。

## 本轮核对结论

核对时间：2026-05-22。基于本地代码、`npm view element-plus version` 和组件库构建结果。

| 项目 | 状态 | 说明 |
| --- | --- | --- |
| 官方基线 | 🟢 已完成 | npm `element-plus` 当前版本为 `2.14.0`，路线保持 Element Plus 2.14。 |
| 版本线 | 🟢 已完成 | `src/Components/Element.csproj` 为 `2.14.0-alpha.1`，主项目为 `net10.0`。 |
| 构建基线 | 🟢 已完成 | `dotnet build src/Components/Element.csproj` 通过；仍有 NuGet/编译/analyzer warning。 |
| DOM class | 🟢 已完成 | 组件输出继续使用 `el-*` 与 `is-*`，例如 `el-button`、`el-input__inner`、`el-table__row`。 |
| Razor 控件名 | 🟢 已完成 | 已有核心和多数现有组件面的 `El*` Razor 入口。 |
| 旧无前缀 Razor 标签 | 🟢 已完成 | `demo`、`template`、`community` 中未扫描到 `<Button>`、`<Input>`、`<Table>` 等旧无前缀 Razor 标签。 |
| 内部实现 | 🟡 进行中 | `El*` 多数仍继承 `B*`，例如 `ElButton : BButton`。短期可接受，后续需内聚或内部化。 |
| 样式基线 | 🟢 已完成 | `theme.css` 已建立 Element Plus 2.14 `--el-*` 变量层，模板已按 `fix.css` -> `index.css` -> `theme.css` 加载；旧 community 入口也已追加 `theme.css`，完整融合仍归 P6。 |
| 文档与 demo | 🟡 进行中 | P1/P2 文档已补齐；文档站总览和 API 表格仍待 P4。 |
| community 主线 | 🟡 进行中 | 旧项目已大面积使用 `El*` 标签，但仍保留 `BlazorCommunity.*`/`Blazui.*` 历史架构；新 `ElementCommunity.*` 主线未建。 |

## 命名规则

| 层级 | 最终规则 | 处理方式 |
| --- | --- | --- |
| DOM/CSS | `el-*`、`is-*` | 完全保留 Element Plus 官方 class 契约。 |
| Blazor 控件 | `El*` PascalCase | `ElButton`、`ElInput`、`ElTable`、`ElFormItem`。 |
| 旧无前缀控件 | 删除 | 不再提供 `Button.razor`、`Input.razor`、`Table.razor` 等公开入口。 |
| `B*` 控件 | 内部化路线 | 当前仍作为实现层；后续逐步改 internal、重命名或合并到 `El*`。 |
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
| Basic | Button、Border、Color、Container、Icon、Layout、Link、Text、Scrollbar、Space、Splitter、Typography | 🟡 已有 Button/Icon/Layout；Border/Color 有 token demo；缺 Link、Text、Scrollbar、Space、Splitter、Typography | P5 |
| Configuration | Config Provider | 🔵 缺全局配置组件 | P5 |
| Form | Autocomplete、Cascader、Checkbox、Color Picker Panel、Color Picker、Date Picker Panel、Date Picker、DateTime Picker、Form、Input、Input Number、Input Tag、Input OTP、Mention、Radio、Rate、Select、Virtualized Select、Slider、Switch、Time Picker、Time Select、Transfer、TreeSelect、Upload | 🟡 已有 Checkbox、DatePicker、Form、Input、Radio、Select、Switch、Transfer、Upload；缺多数新组件与虚拟化组件 | P3/P5 |
| Data | Avatar、Badge、Calendar、Card、Carousel、Collapse、Descriptions、Empty、Image、Infinite Scroll、Pagination、Progress、Result、Skeleton、Table、Virtualized Table、Tag、Timeline、Tour、Tree、Virtualized Tree、Statistic、Segmented | 🟡 已有 Badge、Card、Pagination、Table、Tag、Tree；缺 Empty、Progress、Skeleton、Virtualized Table/Tree 等 | P5 |
| Navigation | Affix、Anchor、Backtop、Breadcrumb、Dropdown、Menu、Page Header、Steps、Tabs | 🟡 已有 Breadcrumb、Dropdown、Menu、Tabs；Steps 当前被排除；缺 Affix、Anchor、Backtop、Page Header | P5 |
| Feedback | Alert、Dialog、Drawer、Loading、Message、Message Box、Notification、Popconfirm、Popover、Tooltip | 🟡 有 Dialog、Loading、Message、MessageBox 基础；缺 Alert、Drawer、Notification、Popconfirm、Popover、Tooltip 完整组件面 | P3/P5 |
| Others | Divider、Watermark | 🔵 缺 | P5 |

## 阶段路线

### P0 门面与版本基线 🟢 已完成

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 切换目标为 `element-plus/element-plus` | 🟢 已完成 | README、路线图、对齐文档均写明 Element Plus 2.14。 |
| 设置 `2.14.0-alpha.1` 版本线 | 🟢 已完成 | `Element.csproj` 进入 2.14 alpha。 |
| 确认 .NET 10 主线 | 🟢 已完成 | 组件库主项目保持 `net10.0`。 |
| 纳入 `community` 子模块 | 🟢 已完成 | `.gitmodules` 包含 `demo`、`template`、`community`。 |
| 验证组件库构建 | 🟢 已完成 | `dotnet build src/Components/Element.csproj` 成功。 |
| 建立 warning 清单 | 🟢 已完成 | 见 [docs/p0-warning-inventory.md](docs/p0-warning-inventory.md)。 |

### P1 Element Plus 主题与设计令牌 🟢 已完成

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 新增 Element Plus CSS 变量层 | 🟢 已完成 | `src/Components/wwwroot/css/theme.css` 提供 `--el-color-*`、`--el-font-size-*`、`--el-border-*`、`--el-box-shadow-*`、`--el-component-size-*`。见 [docs/p1-theme-variables.md](docs/p1-theme-variables.md)。 |
| 梳理旧 theme-chalk 与 fix.css | 🟢 已完成 | 已明确 `index.css`、`fix.css`、`theme.css` 分工和加载顺序，见 [docs/p1-style-inventory.md](docs/p1-style-inventory.md)。 |
| 建立基础 token demo | 🟢 已完成 | 新增 `demo/Theme/DesignTokens.razor` 和 `/theme` 示例页，见 [docs/p1-token-demo.md](docs/p1-token-demo.md)。 |
| 浮层层级规范 | 🟢 已完成 | `theme.css` 定义 `--el-index-*` 和 `--el-box-shadow-*`，并覆盖 Dialog、Select dropdown、Popper、Menu popup 等现有浮层样式。 |

### P2 破坏性控件命名切换 🟢 已完成

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 建立核心 `El*` 控件 | 🟢 已完成 | `ElButton`、`ElInput`、`ElForm`、`ElFormItem`、`ElSelect`、`ElOption`、`ElTable`、`ElTableColumn` 均存在并可编译。见 [docs/p2-el-component-entrypoints.md](docs/p2-el-component-entrypoints.md)。 |
| 删除旧无前缀入口 | 🟢 已完成 | `src/Components` 未发现 `Button.razor`、`Input.razor`、`Table.razor` 等无前缀公开入口。 |
| 迁移内部引用 | 🟢 已完成 | 组件库内部不再引用旧无前缀公开组件名；仍保留 `B*` 内部实现引用。 |
| 迁移 demo/template | 🟢 已完成 | 未扫描到旧无前缀 Razor 标签；模板加载 `theme.css`。 |
| 迁移 community | 🟢 已完成 | 旧社区项目 Razor 标签已迁移为 `El*`；新社区主线另属 P6。 |
| 更新文档示例 | 🟢 已完成 | P1/P2 文档示例使用 `El*`。 |

### P3 核心组件视觉与 API 对齐 🟡 进行中

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| ElButton | 🟢 已完成 | 已对齐 type、size、plain、round、circle、loading、disabled、icon、loading slot、link/text/bg、native type、ButtonGroup 继承与基础可访问性；保留视觉回归覆盖。 |
| ElInput | 🟢 已完成 | 已对齐 clearable、prefix/suffix、textarea、disabled/size 继承、formatter/parser、focus/blur、show-password、word-limit、ARIA、composition/input/change/clear 事件与公开 focus/blur/select/clear 方法。 |
| ElForm/ElFormItem | 🟢 已完成 | 已对齐 label-position、inline、required/error/status、disabled/size 继承、Rules/Validations 校验、ValidateField/ClearValidate/ResetFields、OnValidate、ScrollToError 与 label/错误消息 ARIA 关联。 |
| ElSelect/ElOption | 🟡 进行中 | 已补齐 clearable、filterable、multiple、collapse-tags、remote filter、disabled、loading、empty/no-match、keyboard、visible-change、end-reached 与 ARIA 基础；Virtualized Select 仍归 P5。 |
| ElTable/ElTableColumn | 🟡 进行中 | 已补齐 border、stripe、empty、loading、height/max-height、selection、current-row、sortable、filters、fixed class、分页和滚动同步；Virtualized Table、复杂固定列布局和远程 sort/filter 仍归 P5/P4 测试覆盖。 |
| ElDialog | 🟢 已完成 | 已有 ModelValue/Visible 双向绑定、modal、z-index、ESC/遮罩关闭、destroy-on-close、header/footer slot、open/close 生命周期与基础 ARIA。 |
| ElTabs/ElTabPane | 🟢 已完成 | 已有 type、position、active value、closable/addable、disabled、keyboard、ARIA tablist/tab/tabpanel 与动态 DataSource。 |
| ElMenu/ElSubMenu/MenuItem | 🟢 已完成 | 已有 horizontal/vertical、default-active、disabled、active/hover、submenu popup/inline、route 激活、keyboard 与基础 ARIA。 |

### P4 文档站与组件总览 🔵 待开始

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 文档站骨架 | 🔵 待开始 | 顶部导航、版本标签、左侧分类、组件总览、搜索入口。 |
| 示例卡片 | 🔵 待开始 | 每个组件有基础、禁用、尺寸、状态、事件示例。 |
| API 表格 | 🔵 待开始 | 属性、事件、RenderFragment、方法、类型定义结构统一。 |
| 视觉回归 | 🔵 待开始 | 至少覆盖 ElButton、ElInput、ElSelect、ElTable、ElDialog、ElTabs。 |

### P5 组件矩阵补齐 🔵 待开始

| 分类 | 第一批任务 |
| --- | --- |
| Basic | ElLink、ElText、ElScrollbar、ElSpace、ElSplitter、ElTypography |
| Form | ElInputNumber、ElSlider、ElRate、ElTimePicker、ElAutocomplete、ElCascader、ElColorPicker |
| Data | ElEmpty、ElProgress、ElResult、ElSkeleton、ElAvatar、ElDescriptions、ElCollapse、ElTimeline、ElStatistic、ElSegmented |
| Navigation | ElAffix、ElAnchor、ElBacktop、ElPageHeader、ElSteps |
| Feedback | ElAlert、ElDrawer、ElPopconfirm、ElPopover、ElTooltip、ElNotification |
| Others | ElDivider、ElWatermark |

### P6 社区展示站 🟡 进行中

目标：把 `community` 按 [community-rebuild-plan.md](docs/community-rebuild-plan.md) 重写成类似 DiscuzX 的真实业务展示站，不保留旧 UI、旧组件名、旧包名、旧启动方式。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 社区基线审计 | 🟢 已完成 | 已明确旧 `BlazorCommunity.*` 为 .NET Core 3.1/netstandard2.1 时代项目，只作为业务参考。 |
| 旧社区 Razor 标签迁移 | 🟢 已完成 | 旧社区 Razor 文件未扫描到 `<B*>` 或旧无前缀公开标签。 |
| 新主线骨架 | 🔵 待开始 | 新建 `ElementCommunity.*`，先跑通前台主站、主题详情、发帖页。 |
| 数据与账号 | 🔵 待开始 | 新建 DbContext、Identity、种子数据，跑通登录、发帖、回帖。 |
| Element Plus 主题融合 | 🟡 进行中 | 旧 community 入口已加载 Element-Blazor 2.14 `theme.css`，但前台、后台、移动端的信息架构和视觉统一仍待新主线完成。 |
| DiscuzX 信息架构 | 🔵 待开始 | 首页、版块、主题列表、帖子详情、回复、用户中心、通知、管理后台完整。 |
| 本地一键启动 | 🔵 待开始 | 提供启动脚本和说明，能本地跑通核心流程。 |

### P7 稳定化与发布 🔵 待开始

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 依赖安全修复 | 🔵 待开始 | 处理 `Microsoft.AspNetCore.Components 7.0.2` 等已知安全 warning。 |
| NuGet 包门面 | 🔵 待开始 | README、icon、license、symbols、source link、package readme 完整。 |
| 测试矩阵 | 🔵 待开始 | 单元测试、Playwright 交互测试、视觉截图测试分层。 |
| 发布节奏 | 🔵 待开始 | `2.14.0-alpha.*`、`beta`、`stable` 每阶段有明确验收。 |

### P8 Blazor Element Plus Admin 复刻 🟡 进行中

目标：把 [kailong321200875/vue-element-plus-admin](https://github.com/kailong321200875/vue-element-plus-admin) 按 Blazor 原生方式完整复刻到 [Element-Blazor/blazor-element-plus-admin](https://github.com/Element-Blazor/blazor-element-plus-admin)，作为本仓库子模块维护。

| 任务 | 状态 | 验收 |
| --- | --- | --- |
| 接入子模块 | 🟢 已完成 | `.gitmodules` 已包含 `blazor-element-plus-admin`。 |
| 源项目基线核对 | 🟢 已完成 | 源项目 `2.9.0`、HEAD `38047fba67ea1e0fac9d576caf0facd39c96d235`、MIT 许可证已记录。 |
| 复刻路线图 | 🟢 已完成 | 见 [blazor-element-plus-admin/ROADMAP.md](blazor-element-plus-admin/ROADMAP.md)。 |
| Blazor 工程骨架 | 🔵 待开始 | .NET 10 Blazor 项目可启动并引用本地 Element-Blazor。 |
| Admin Shell MVP | 🔵 待开始 | 登录、主布局、菜单、TagsView、设置、Dashboard 首屏跑通。 |
| 全量页面复刻 | 🔵 待开始 | Dashboard、Components、Function、Hooks、Level、Example、Error、Authorization、Personal 路由矩阵完整。 |

## 执行顺序

1. 🟢 已完成 P0-1：路线图、README、对齐文档、版本线定稿。
2. 🟢 已完成 P0-2：整理 warning 清单和依赖安全清单。
3. 🟢 已完成 P1：Element Plus 2.14 CSS 变量层、样式分层、token demo、浮层层级规范。
4. 🟢 已完成 P2：核心 `El*` 入口、旧无前缀入口删除、demo/template/community Razor 标签迁移。
5. 🟢 已完成 P3-1：ElButton、ElInput、ElForm/ElFormItem 对齐完成。
6. 🟡 进行中 P3-2：ElDialog、ElTabs、ElMenu 已完成；ElSelect/ElTable 高频能力已补齐，虚拟化与高级远程能力留待 P5/P4。
7. 🔵 待开始 P4-1：建设组件总览和文档站模板。
8. 🔵 待开始 P5：按组件矩阵补齐缺口。
9. 🔵 待开始 P6：按重构蓝图新建社区展示站主线，旧 `BlazorCommunity.*` 只读参考。
10. 🔵 待开始 P7：测试、NuGet、发布稳定化。
11. 🟡 进行中 P8：`blazor-element-plus-admin` 子模块已接入，下一步建立 Blazor 工程骨架与 Admin Shell MVP。

## 任务提示词

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

### P8 Blazor Admin MVP

```text
请在 blazor-element-plus-admin 子模块内创建 .NET 10 Blazor 工程，引用当前仓库的 Element-Blazor 组件库，并按 ROADMAP.md 实现 Admin Shell MVP：登录页、主布局、侧边栏菜单、顶部工具栏、面包屑、TagsView、设置面板、Dashboard Analysis/Workplace 和错误页。页面必须使用 El* 组件，不得嵌入 Vue 运行时。
```

## 参考来源

- Element Plus 仓库：https://github.com/element-plus/element-plus
- Element Plus 组件总览：https://element-plus.org/zh-CN/component/overview
- Element Plus Changelog：https://element-plus.org/en-US/guide/changelog
- Element Plus Release：https://github.com/element-plus/element-plus/releases
- Element Plus 2.14.0 CSS 包：https://unpkg.com/element-plus@2.14.0/dist/index.css
