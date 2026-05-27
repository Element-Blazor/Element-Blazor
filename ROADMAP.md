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
| Mention | 缺少 `ElMention` | 🔴 缺失 | 实现触发字符、候选项和文本区域集成。 |
| Radio | `ElRadio`、`ElRadioGroup`、`ElRadioButton` | 🟡 部分对齐 | 补齐禁用、尺寸、边框和键盘行为。 |
| Rate | `ElRate` | 🟡 部分对齐 | 已实现评分、半星、颜色、文本/分值、清除、禁用和表单校验联动；继续补齐自定义图标插槽和测试。 |
| Select | `ElSelect`、`ElOption` | 🟡 部分对齐 | 补齐多选、远程搜索、过滤、分组和虚拟化边界。 |
| Virtualized Select | 缺少 `ElSelectV2` | 🔴 缺失 | 设计大数据虚拟列表。 |
| Slider | `ElSlider`、`SliderMark` | 🟡 部分对齐 | 已实现拖拽、范围、步长、停点、marks、输入框联动和表单校验联动；继续补齐 range 双滑块、tooltip 和测试。 |
| Switch | `ElSwitch` | 🟡 部分对齐 | 补齐 active/inactive text/value、loading、before-change。 |
| Time Picker | 缺少 `ElTimePicker` | 🔴 缺失 | 实现时间面板、范围和格式。 |
| Time Select | `ElTimeSelect` | 🟡 部分对齐 | 已实现固定步长时间选择、禁用时间范围、清空、尺寸和表单校验联动；继续补齐键盘导航和测试。 |
| Transfer | `ElTransfer` | 🟡 部分对齐 | 补齐过滤、插槽、方向文案和全选逻辑。 |
| TreeSelect | `ElTreeSingleSelect`，缺少官方命名入口 | 🟡 部分对齐 | 新增 `ElTreeSelect` 并对齐 Select + Tree API。 |
| Upload | `ElUpload` | 🟡 部分对齐 | 补齐列表类型、拖拽、限制、钩子和请求定制。 |

### Data 数据展示

| Element Plus | Element-Blazor 入口/现状 | 状态 | 下一步 |
| --- | --- | --- | --- |
| Avatar | 缺少 `ElAvatar` | 🔴 缺失 | 实现图片、图标、文字、尺寸和形状。 |
| Badge | `ElBadge` | 🟡 部分对齐 | 补 max、dot、hidden、offset、slot。 |
| Calendar | 缺少 `ElCalendar` | 🔴 缺失 | 实现日期单元格模板和月份切换。 |
| Card | `ElCard` | 🟡 部分对齐 | 补 body-style、shadow、header/footer 插槽。 |
| Carousel | 缺少 `ElCarousel` | 🔴 缺失 | 实现轮播、指示器、箭头和 autoplay。 |
| Collapse | 缺少 `ElCollapse` | 🔴 缺失 | 实现折叠面板和手风琴模式。 |
| Descriptions | 缺少 `ElDescriptions` | 🔴 缺失 | 实现描述列表、边框、列数和响应式。 |
| Empty | 仅有内部 `EmptyRender` | 🔴 缺失 | 提供公开 `ElEmpty`。 |
| Image | 缺少 `ElImage` | 🔴 缺失 | 实现 fit、lazy、preview 和错误占位。 |
| Infinite Scroll | 缺少公开能力 | 🔴 缺失 | 设计 Blazor 事件指令或组件包装。 |
| Pagination | `ElPagination` | 🟡 部分对齐 | 补齐布局、尺寸、背景、跳转和页大小。 |
| Progress | 缺少 `ElProgress` | 🔴 缺失 | 实现 line、circle、dashboard 和状态。 |
| Result | 缺少 `ElResult` | 🔴 缺失 | 实现图标、标题、描述和 extra。 |
| Skeleton | 缺少 `ElSkeleton` | 🔴 缺失 | 实现骨架屏、模板和 loading。 |
| Table | `ElTable` 与列组件 | 🟡 部分对齐 | 补齐排序、过滤、固定列、展开、树形、汇总和虚拟化边界。 |
| Virtualized Table | 缺少 `ElTableV2` | 🔴 缺失 | 设计虚拟表格架构。 |
| Tag | `ElTag` | 🟡 部分对齐 | 补 effect、round、hit、disable-transitions 和事件命名。 |
| Timeline | 缺少 `ElTimeline` | 🔴 缺失 | 实现时间线和节点插槽。 |
| Tour | 缺少 `ElTour` | 🔴 缺失 | 实现引导步骤、遮罩和定位。 |
| Tree | `ElTree`、`ElTreeItem` | 🟡 部分对齐 | 补懒加载、过滤、勾选、拖拽和节点模板。 |
| Virtualized Tree | 缺少 `ElTreeV2` | 🔴 缺失 | 设计虚拟树。 |
| Statistic | 缺少 `ElStatistic` | 🔴 缺失 | 实现数字格式、前后缀和倒计时扩展。 |
| Segmented | 缺少 `ElSegmented` | 🔴 缺失 | 实现分段控制器。 |

### Navigation 导航

| Element Plus | Element-Blazor 入口/现状 | 状态 | 下一步 |
| --- | --- | --- | --- |
| Affix | 缺少 `ElAffix` | 🔴 缺失 | 实现固定定位、offset 和 target。 |
| Anchor | 缺少 `ElAnchor` | 🔴 缺失 | 实现锚点列表、滚动监听和 hash。 |
| Backtop | CSS 存在，缺少公开入口 | 🔴 缺失 | 提供 `ElBacktop` 和滚动目标。 |
| Breadcrumb | `ElBreadcrumb`、`ElBreadcrumbItem` | 🟡 部分对齐 | 补 separator icon、replace 和路由行为。 |
| Dropdown | `ElDropdown`、`ElDropdownItem` | 🟡 部分对齐 | 补触发方式、分裂按钮、禁用、命令事件。 |
| Menu | `ElMenu`、`ElSubMenu`、`ElMenuItem` | 🟡 部分对齐 | 补 collapse、router、popper、键盘和主题。 |
| Page Header | CSS 存在，缺少公开入口 | 🔴 缺失 | 提供 `ElPageHeader`。 |
| Steps | 缺少 `ElSteps` | 🔴 缺失 | 实现步骤条、方向、状态和图标。 |
| Tabs | `ElTabs`、`ElTabPane` | 🟡 部分对齐 | 补 closable、editable、stretch、before-leave 和键盘。 |

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
