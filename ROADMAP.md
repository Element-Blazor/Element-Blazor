# Element Plus 2.14 路线图

> 最终决策：Element-Blazor 主线对齐 Element Plus `2.14.0`。公开 Razor 组件统一使用 `El*`，基础设施统一使用 `Element*`。旧组件入口不兼容、不保留、不做别名。

## 本轮核对结论

核对时间：2026-05-26。

核对来源：Element Plus 官方组件总览 `https://element-plus.org/en-US/component/overview.html`。

| 项目 | 状态 | 说明 |
| --- | --- | --- |
| 官方基线 | 已完成 | 以 Element Plus `2.14.0` 为当前视觉、组件矩阵和版本基线。 |
| 版本线 | 已完成 | `src/Components/Element.csproj` 使用 `2.14.0-alpha.1`，主项目为 `net10.0`。 |
| DOM class | 已完成 | 组件继续输出 `el-*` 与 `is-*` class 契约。 |
| Razor 组件名 | 已完成 | 活跃组件入口统一为 `El*`。 |
| 基础设施命名 | 已完成 | 基类、异常、事件参数、全局设置和服务扩展统一为 `Element*` / `AddElementServices`。 |
| 旧组件实现 | 已完成 | 旧 B 前缀组件实现、旧项目和旧包入口已删除，不留兼容层、继承包装或过渡别名。 |
| demo/template/community | 已完成 | 示例和模板中的活跃 Razor 标签已迁移为 `El*`。 |

## 命名规则

| 层级 | 规则 |
| --- | --- |
| DOM/CSS | 保持 Element Plus 官方 `el-*`、`is-*` class。 |
| Blazor 组件 | 使用 `El*` PascalCase，例如 `ElButton`、`ElInput`、`ElTable`、`ElFormItem`。 |
| 公共基础类型 | 使用 `Element*`，例如 `ElementComponentBase`、`ElementDialogBase`、`ElementChangeEventArgs`。 |
| 服务扩展 | 使用 `AddElementServices`。 |
| 旧组件入口 | 直接删除，不兼容、不保留、不做别名。 |
| `<el-button>` / `el_` | 不采用；Razor 不会把连字符标签解析为 C# 组件类型，`el_` 也不符合 .NET 命名习惯。 |

目标写法：

```razor
<ElButton Type="ButtonType.Primary" Loading="true">保存</ElButton>
<ElInput TValue="string" Placeholder="请输入" Clearable="true" />
<ElTable DataSource="@rows" />
```

## 阶段进度

| 阶段 | 状态 | 说明 |
| --- | --- | --- |
| P0 门面与版本基线 | 已完成 | README、路线图、对齐文档、warning 清单已建立。 |
| P1 Element Plus 主题变量 | 已完成 | `theme.css` 提供 `--el-*` 变量层，并接入示例入口。 |
| P2 组件命名切换 | 已完成 | 旧组件入口已清理，主线只使用 `El*` / `Element*`。 |
| P3 核心组件对齐 | 进行中 | `ElButton`、`ElInput`、`ElForm`、`ElDialog`、`ElTabs`、`ElMenu` 已推进；`ElSelect`、`ElTable` 继续补齐高级能力。 |
| P4 文档站与组件总览 | 待开始 | 建设组件总览、示例卡片和 API 表格。 |
| P5 组件矩阵补齐 | 进行中 | 按 Element Plus 矩阵继续补齐缺口组件，完成一个就在下方矩阵标记一个。 |
| P6 社区展示站 | 进行中 | 旧社区项目作为业务素材来源，主线使用 `El*` 控件和 Element Plus 主题。 |
| P7 稳定化与发布 | 待开始 | 安全依赖、测试矩阵、NuGet 门面和发布节奏。 |

## Element Plus 2.14 组件矩阵

状态标记：

| 标记 | 含义 |
| --- | --- |
| 🟢 已对齐 | 已有公开入口，核心 DOM、API 和基础行为完成首轮对齐。 |
| 🟡 部分对齐 | 已有公开入口或服务能力，但 API、DOM、交互、示例或测试仍需补齐。 |
| 🔵 规划/主题项 | 官方是设计、主题或文档项，优先用 token、CSS、文档和示例承接。 |
| 🔴 缺失 | 还没有可用的公开 `El*` 组件入口。 |
| ⚪ 不适用 | 官方能力不适合做成独立 Blazor 组件，需在备注中说明替代方案。 |

### Basic 基础组件

| Element Plus | Element-Blazor 入口/现状 | 状态 | 下一步 |
| --- | --- | --- | --- |
| Button | `ElButton` | 🟢 已对齐 | API、示例和基础交互已补齐。 |
| Border | `theme.css` token | 🟢 已对齐 | 已补充边框 token 文档和示例。 |
| Color | `theme.css` token | 🟢 已对齐 | 已补充色彩 token 文档和示例。 |
| Layout Container | `ElContainer`、`ElHeader`、`ElAside`、`ElMain`、`ElFooter`，旧 `ElLayout` 保留 | 🟢 已对齐 | 新容器五件套已提供公开入口和示例。 |
| Icon | `ElIcon` | 🟢 已对齐 | 已支持名称、尺寸、颜色和旋转状态。 |
| Layout | `ElRow`、`ElCol` | 🟢 已对齐 | 已实现 24 栅格、gutter、offset、push/pull、响应式属性和示例。 |
| Link | `ElLink` | 🟢 已对齐 | 已实现链接类型、禁用、下划线和图标能力。 |
| Text | `ElText` | 🟢 已对齐 | 已实现类型、尺寸、截断、line-clamp 和 tag。 |
| Scrollbar | `ElScrollbar` | 🟢 已对齐 | 已提供公开滚动容器、常显滚动条和滚动方法。 |
| Space | `ElSpace` | 🟢 已对齐 | 已实现间距、方向、换行、填充和 spacer。 |
| Splitter | `ElSplitter`、`ElSplitterPanel` | 🟢 已对齐 | 已实现水平/垂直分隔面板、尺寸约束和拖拽。 |
| Typography | `theme.css` 基础文字 token | 🟢 已对齐 | 已补充排版规范、文档和示例。 |

### Configuration 配置组件

| Element Plus | Element-Blazor 入口/现状 | 状态 | 下一步 |
| --- | --- | --- | --- |
| Config Provider | `ElConfigProvider`、`ElementConfig` | 🟢 已对齐 | 已提供尺寸、命名空间、z-index、locale 级联配置，并在自定义命名空间时重写子树 DOM class。 |

### Form 表单组件

| Element Plus | Element-Blazor 入口/现状 | 状态 | 下一步 |
| --- | --- | --- | --- |
| Autocomplete | `ElAutocomplete` | 🟢 已对齐 | 已实现输入建议、异步查询、远程节流与旧请求作废、键盘选择、清空、禁用、尺寸、表单校验联动、项模板插槽示例和组件测试。 |
| Cascader | `ElCascader`、`CascaderOption` | 🟢 已对齐 | 已实现级联数据模型、面板、选择路径、多选、懒加载、过滤建议、节点/建议模板、清空、禁用、尺寸、表单校验联动和组件测试。 |
| Checkbox | `ElCheckbox`、`ElCheckboxGroup`、`ElCheckboxButton` | 🟢 已对齐 | 已补齐 indeterminate、group value 绑定、min/max limit、尺寸传播、button 变体状态和组件测试。 |
| Color Picker Panel | `ElColorPickerPanel`、`ElementColor` | 🟢 已对齐 | 已实现颜色模型、独立面板、透明度、预设色、颜色格式、禁用、边框、footer 插槽、表单校验联动和组件测试。 |
| Color Picker | `ElColorPicker` | 🟢 已对齐 | 已基于面板补齐输入触发器、弹层、清空、尺寸、透明度、预设色、颜色格式、禁用、事件和组件测试。 |
| Date Picker Panel | `ElDatePickerPanel`、`DatePickerPanelType` | 🟢 已对齐 | 已抽出公开日期/月/年面板入口，支持日期模型、面板切换、禁用日期、footer 插槽、表单校验联动和组件测试。 |
| Date Picker | `ElDatePicker`、`DatePickerType`、`DatePickerShortcut` | 🟢 已对齐 | 已基于公开面板补齐单值、范围、快捷项、禁用日期、显示格式、值格式、清空、弹层事件和组件测试。 |
| DateTime Picker | `ElDateTimePicker`、`DateTimePickerType`、`DateTimePickerShortcut` | 🟢 已对齐 | 已基于公开日期面板补齐单值、范围、时间输入、默认时间、快捷项、禁用日期、显示格式、值格式、清空、弹层事件和组件测试。 |
| Form | `ElForm`、`ElFormItem`、`ElFormActionItem` | 🟢 已对齐 | 已补齐 label、size、disabled、rules、DataAnnotations、异步校验、字段校验、提交、重置、清理校验、字段滚动、嵌套 Prop、EditContext、插槽和自动生成表单绑定闭环。 |
| Input | `ElInput` | 🟢 已对齐 | 已实现 prefix/suffix 插槽、show-word-limit、formatter/parser、禁用/只读、清空、尺寸和表单校验联动，并补充组件测试。 |
| Input Number | `ElInputNumber` | 🟢 已对齐 | 已实现步进、精度、范围、按钮位置、禁用/只读、严格键盘体验、ARIA 细节、表单校验联动和组件测试。 |
| Input Tag | `ElInputTag` | 🟢 已对齐 | 已实现标签输入、删除、数量限制、触发键、失焦提交、拖拽排序、组合键细节、表单校验联动和组件测试。 |
| Input OTP | `ElInputOtp` | 🟢 已对齐 | 已实现分格输入、粘贴填充、焦点流转、掩码、禁用/只读、更细键盘行为、表单校验联动和组件测试。 |
| Mention | `ElMention` | 🟢 已对齐 | 已实现触发字符、候选项、键盘导航、多前缀、文本区域集成、表单校验联动和组件测试。 |
| Radio | `ElRadio`、`ElRadioGroup`、`ElRadioButton` | 🟢 已对齐 | 已实现禁用、尺寸、边框、按钮样式、表单尺寸继承、ARIA 和键盘行为，并补充组件测试。 |
| Rate | `ElRate` | 🟢 已对齐 | 已实现评分、半星、颜色、文本/分值、清除、禁用、自定义图标插槽、表单校验联动和组件测试。 |
| Select | `ElSelect`、`ElOption`、`ElOptionGroup` | 🟢 已对齐 | 已实现多选、远程搜索、过滤、分组、禁用分组、ARIA 细节和组件测试。 |
| Virtualized Select | `ElSelectV2` | 🟢 已对齐 | 已实现大数据虚拟列表、过滤、远程搜索、多选、分组、键盘导航、边界钳制和组件测试。 |
| Slider | `ElSlider`、`SliderMark` | 🟢 已对齐 | 已实现拖拽、range 双滑块、tooltip/格式化、步长、停点、marks、输入框联动、ARIA、表单校验联动和组件测试。 |
| Switch | `ElSwitch` | 🟢 已对齐 | 已实现 active/inactive text/value、loading、before-change、键盘/ARIA、表单联动和组件测试。 |
| Time Picker | `ElTimePicker`、`TimePickerType` | 🟢 已对齐 | 已实现时间面板、单值/范围选择、显示格式、值格式、清空、键盘/ARIA、表单联动和组件测试。 |
| Time Select | `ElTimeSelect` | 🟢 已对齐 | 已实现固定步长时间选择、禁用时间范围、键盘导航、清空、尺寸、表单校验联动和组件测试。 |
| Transfer | `ElTransfer` | 🟢 已对齐 | 已实现过滤、自定义过滤、项/面板插槽、方向按钮文案、禁用项、可见项全选逻辑、表单联动和组件测试。 |
| TreeSelect | `ElTreeSelect`、`ElTreeSingleSelect` | 🟢 已对齐 | 已新增官方入口 `ElTreeSelect`，兼容旧名，并对齐 Select + Tree API。 |
| Upload | `ElUpload` | 🟢 已对齐 | 已补齐列表类型、拖拽上传、数量/类型/大小/图片尺寸限制、before/remove/preview/change/progress/success/error/exceed 钩子、请求参数和自定义请求，并补充组件测试。 |

### Data 数据展示

| Element Plus | Element-Blazor 入口/现状 | 状态 | 下一步 |
| --- | --- | --- | --- |
| Avatar | `ElAvatar` | 🟢 已对齐 | 已实现图片、图标、文字、尺寸、形状、fit 和组件测试。 |
| Badge | `ElBadge` | 🟢 已对齐 | 已补 max、dot、hidden、offset、slot 和组件测试。 |
| Calendar | `ElCalendar`、`CalendarDateContext` | 🟢 已对齐 | 已实现日期单元格模板、月份切换、选择事件和组件测试。 |
| Card | `ElCard` | 🟢 已对齐 | 已补 body-style、hover shadow、header/body/footer 插槽和组件测试。 |
| Carousel | `ElCarousel`、`ElCarouselItem` | 🟢 已对齐 | 已实现轮播项、指示器、箭头、autoplay 和组件测试。 |
| Collapse | `ElCollapse`、`ElCollapseItem` | 🟢 已对齐 | 已实现折叠面板、手风琴模式、禁用和键盘触发。 |
| Descriptions | `ElDescriptions`、`ElDescriptionsItem` | 🟢 已对齐 | 已实现描述列表、边框、列数、方向、插槽和组件测试。 |
| Empty | `ElEmpty` | 🟢 已对齐 | 已提供公开空状态组件，支持图片、描述和底部插槽。 |
| Image | `ElImage` | 🟢 已对齐 | 已实现 fit、lazy、preview、错误占位和组件测试。 |
| Infinite Scroll | `ElInfiniteScroll` | 🟢 已对齐 | 已实现滚动容器、加载/禁用状态、节流和 `OnLoad` 事件。 |
| Pagination | `ElPagination` | 🟢 已对齐 | 已补齐 layout、sizes、jumper、total、尺寸、背景和组件测试。 |
| Progress | `ElProgress`、`ProgressType`、`ProgressStatus` | 🟢 已对齐 | 已实现 line、circle、dashboard、状态、颜色和组件测试。 |
| Result | `ElResult`、`ResultIcon` | 🟢 已对齐 | 已实现图标、标题、描述、extra 和组件测试。 |
| Skeleton | `ElSkeleton`、`ElSkeletonItem` | 🟢 已对齐 | 已实现骨架屏、模板、loading、animated 和组件测试。 |
| Table | `ElTable` 与列组件、`ElTableV2` | 🟢 已对齐 | `ElTable` 已覆盖排序、过滤、固定列和树形列，展开/汇总/虚拟窗口由 `ElTableV2` 承接并补充组件测试。 |
| Virtualized Table | `ElTableV2` | 🟢 已对齐 | 已实现数据窗口、自动列、展开行、汇总行和组件测试。 |
| Tag | `ElTag` | 🟢 已对齐 | 已补 effect、round、hit、disable-transitions、close 事件别名和组件测试。 |
| Timeline | `ElTimeline`、`ElTimelineItem` | 🟢 已对齐 | 已实现时间线、时间戳、图标、自定义节点和组件测试。 |
| Tour | `ElTour`、`TourStep` | 🟢 已对齐 | 已实现引导步骤、遮罩、方向 class、前后步骤、关闭事件和组件测试。 |
| Tree | `ElTree`、`ElTreeItem` | 🟢 已对齐 | 已补齐节点模板、过滤、勾选、懒加载、拖拽和组件测试。 |
| Virtualized Tree | `ElTreeV2` | 🟢 已对齐 | 已提供虚拟树公开入口，复用 `ElTree` 数据管线和渲染能力并补充组件测试。 |
| Statistic | `ElStatistic`、`ElCountdown` | 🟢 已对齐 | 已实现数字格式、前后缀、倒计时扩展和组件测试。 |
| Segmented | `ElSegmented<TValue>`、`SegmentedOption` | 🟢 已对齐 | 已实现分段控制器、禁用、block、尺寸和组件测试。 |

### Navigation 导航

| Element Plus | Element-Blazor 入口/现状 | 状态 | 下一步 |
| --- | --- | --- | --- |
| Affix | `ElAffix` | 🟢 已对齐 | 已实现固定定位、offset、target、position、z-index、change/scroll 事件和组件测试。 |
| Anchor | `ElAnchor`、`ElAnchorLink` | 🟢 已对齐 | 已实现锚点列表、活动态、滚动监听、hash 滚动、点击/change 事件、方向和 underline 类型。 |
| Backtop | `ElBacktop` | 🟢 已对齐 | 已提供公开入口、滚动目标、可见阈值、位置配置、点击回顶和组件测试。 |
| Breadcrumb | `ElBreadcrumb`、`ElBreadcrumbItem` | 🟢 已对齐 | 已补 separator icon、replace、`To/Href` 和路由导航行为，并补充组件测试。 |
| Dropdown | `ElDropdown`、`ElDropdownItem` | 🟢 已对齐 | 已补触发方式、分裂按钮、禁用项、命令事件和组件测试。 |
| Menu | `ElMenu`、`ElSubMenu`、`ElMenuItem` | 🟢 已对齐 | 已补 collapse、router、popper、键盘导航、主题和组件测试。 |
| Page Header | `ElPageHeader` | 🟢 已对齐 | 已提供返回区、标题/内容/面包屑/extra/main 插槽、图标和 back 事件。 |
| Steps | `ElSteps`、`ElStep` | 🟢 已对齐 | 已实现步骤条、水平/垂直、simple、居中、状态、图标、描述和组件测试。 |
| Tabs | `ElTabs`、`ElTabPane` | 🟢 已对齐 | 已补 closable、editable、stretch、before-leave、键盘导航和组件测试。 |

### Feedback 反馈组件

| Element Plus | Element-Blazor 入口/现状 | 状态 | 下一步 |
| --- | --- | --- | --- |
| Alert | 缺少 `ElAlert` | 🔴 缺失 | 实现静态提示、关闭、图标和描述。 |
| Dialog | `ElDialog`、`DialogService` | 🟡 部分对齐 | 补 append-to-body、destroy、拖拽、焦点和无障碍。 |
| Drawer | CSS 存在，缺少公开入口 | 🔴 缺失 | 提供 `ElDrawer`。 |
| Loading | `ElLoading`、`LoadingService` | 🟡 部分对齐 | 补指令式/服务式 API、锁屏和自定义图标。 |
| Message | `MessageService` | 🟡 部分对齐 | 补 grouping、plain、duration、append-to 和 close 事件。 |
| Message Box | `MessageBox` 服务 | 🟡 部分对齐 | 补 prompt、distinguish-cancel-close、before-close。 |
| Notification | 缺少 `ElNotification` | 🔴 缺失 | 实现通知服务和位置。 |
| Popconfirm | CSS 存在，缺少公开入口 | 🔴 缺失 | 提供 `ElPopconfirm`。 |
| Popover | 缺少 `ElPopover` | 🔴 缺失 | 实现弹出层、触发和定位。 |
| Tooltip | 缺少 `ElTooltip` | 🔴 缺失 | 实现提示、触发、定位和可访问性。 |

### Others 其他

| Element Plus | Element-Blazor 入口/现状 | 状态 | 下一步 |
| --- | --- | --- | --- |
| Divider | `ElDivider` | 🟢 已对齐 | 已实现 direction、content-position、border-style。 |
| Watermark | 缺少 `ElWatermark` | 🔴 缺失 | 实现水印文本、图片、层级和容器。 |

## P2 验收口径

- `src/Components` 不存在旧组件实现入口。
- `src/Markdown` 不存在旧 Markdown 组件入口。
- `demo`、`template`、`community` 的活跃 Razor 标签不使用旧组件标签。
- 不新增兼容包装、继承别名或旧包入口。
- 扫描旧组件 API 名称时，活跃代码应无命中；业务实体命名不属于组件清理范围。
