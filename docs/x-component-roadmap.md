# Element Plus X 复刻路线图

> 决策日期：2026-05-30。
>
> 目标：在 `src` 下新增 Element-Blazor X 组件能力，功能对齐 `element-plus-x/Element-Plus-X`，并在 `demo` 中使用本项目复刻组件实现 `element-plus-x/ruoyi-element-ai` 的 AI 应用演示，同时把演示站首页和整体风格对齐 `https://v2.element-plus-x.com/zh/`。

## 基线

| 项目 | 基线 |
| --- | --- |
| 上游组件库 | `element-plus-x/Element-Plus-X`，当前远端 tag 以 `v2.0.3` 为最新公开 tag。 |
| 上游模板 | `element-plus-x/ruoyi-element-ai`，当前远端 tag 为 `1.0`，README 描述为企业级 AI 应用模板。 |
| 视觉站点 | `https://v2.element-plus-x.com/zh/`。 |
| 本地承载 | `src/X` 承载独立 `Element.X` 包；`demo` 新增 X 组件文档、AI 模板演示和首页风格。 |
| 依赖边界 | 只依赖本仓库 Blazor Element 组件和 .NET/Blazor 基础能力。不得引入 Vue、Element Plus、Element Plus X npm 包、第三方 UI 组件库或外部前端运行时。 |

## 命名规则

- Razor 组件统一使用 `ElX*`，例如 `ElXBubble`、`ElXSender`、`ElXConversations`。
- 支撑模型统一使用 `X*`，例如 `XMessageItem`、`XConversationItem`、`XAttachmentItem`、`XPromptItem`。
- DOM/CSS class 使用 `el-x-*`，主题变量使用 `--el-x-*`，底层尺寸、颜色、圆角、阴影优先复用现有 `--el-*` token。
- 不提供 Vue 风格标签、npm 包别名或兼容包装。上游 Vue 代码只作为功能/API/交互参考，不复制实现。

## 组件矩阵

状态：✅ 已完成；🟡 部分完成；🚧 进行中；⬜ 未开始。

| 状态 | 上游能力 | Element-Blazor 入口 | 阶段 | 当前判断 |
| --- | --- | --- | --- | --- |
| ✅ | Typewriter | `ElXTypewriter` | X1 | 已有组件入口、打字控制、完成回调和 CSS。 |
| ✅ | Bubble | `ElXBubble` | X1 | 已支持角色、头像/标题/时间、加载/打字、操作区、附件和模板，并有 bUnit 覆盖。 |
| ✅ | BubbleList | `ElXBubbleList` | X1 | 已支持列表、空状态、加载占位、模板、自动滚动、用户滚动锁定和 bUnit 覆盖。 |
| ✅ | Conversations | `ElXConversations` | X1 | 已支持列表、分组、置顶、激活态、选择事件、内联新建、重命名、删除和 bUnit 覆盖。 |
| ✅ | Welcome | `ElXWelcome` | X1 | 已支持标题、描述、图标/头像、操作区和推荐内容。 |
| ✅ | Prompts | `ElXPrompts` | X1 | 已支持分组、图标、描述、禁用、选择事件和响应式布局。 |
| ✅ | FilesCard | `ElXFilesCard` | X2 | 已支持文件名、大小、类型图标、状态、进度、预览、删除、重试和错误展示。 |
| ✅ | Attachments | `ElXAttachments` | X2 | 已基于 `ElUpload` 支持选择、拖拽、列表、限制、事件、上传状态映射、模板扩展、异常链路和 bUnit 覆盖。 |
| ✅ | Sender / XSender | `ElXSender` | X2 | 已支持多行输入、发送/停止、加载态、工具区、附件区、清空、组合快捷键、Escape 行为、无障碍说明和 bUnit 覆盖。 |
| ✅ | MentionSender | `ElXMentionSender` | X2 | 已组合 `ElMention`/`ElXSender`，支持多前缀、候选选择回调、指令联动、停止/清空/Escape 和 bUnit 覆盖。 |
| ✅ | Thinking | `ElXThinking` | X2 | 已支持思考中/完成状态、折叠展开、持续时间和内容模板，并有 bUnit 覆盖。 |
| ✅ | ThoughtChain | `ElXThoughtChain` | X2 | 已基于现有时间线风格支持节点、状态、耗时和展开内容，并有 bUnit 覆盖。 |
| ✅ | useRecord | `ElementXRecordService` | X3 | 已落地录音服务入口、浏览器能力探测、开始/停止/取消 API、JS 静态资源和降级测试。 |
| ✅ | useXStream | `ElementXStreamService` | X3 | 已落地独立流式服务、离线流式响应、SSE 行解析、收集工具和服务层测试。 |
| ✅ | useSend / XRequest | `ElementXRequestService` | X3 | 已落地请求服务、会话上下文、消息生成、流式传输、取消控制和服务层测试。 |

## Demo 复刻范围

| 状态 | 目标 | 路由/位置 | 当前判断 |
| --- | --- | --- | --- |
| ✅ | X 组件文档 | `demo` 的 X 分类 | 已有 X 分类、`BasicX`、`FullX` 和 `AiWorkspace` 示例；覆盖每个 `ElX*` 组件、服务入口、组合和事件示例。 |
| ✅ | ruoyi-element-ai 演示 | `demo/XAi` 或同等路由 | `demo/X/AiWorkspace.razor` 已打通左侧会话操作、附件、流式回复、停止生成和本地服务联动。 |
| ✅ | 模拟服务 | `Element.X` 服务层 | 已沉淀为 `ElementXStreamService` 与 `ElementXRequestService`，demo 默认离线流式运行，可替换 transport 接真实后端。 |
| ✅ | 视觉首页 | demo 首页与站点样式 | demo 首页已补齐文档站首屏、搜索入口、版本/矩阵信号、X 入口和响应式布局，并参考 v2 Element Plus X 文档站的导航/入口结构完成本地验收。 |

## 阶段计划

| 状态 | 阶段 | 目标 | 当前判断 |
| --- | --- | --- | --- |
| ✅ | X0 路线固化 | 固化本文档、主路线图入口、依赖禁令和组件矩阵。 | 已完成路线图入口和依赖约束；当前拆包后没有引入第三方 UI 依赖。 |
| ✅ | X1 核心展示组件 | `Typewriter`、`Bubble`、`BubbleList`、`Conversations`、`Welcome`、`Prompts`。 | 首批组件、CSS、demo 示例、自动滚动、完整会话操作和 bUnit 测试已落地。 |
| ✅ | X2 输入与思考组件 | `FilesCard`、`Attachments`、`ElXSender`、`ElXMentionSender`、`Thinking`、`ThoughtChain`。 | 上传状态、异常链路、指令选择、清空/快捷键、无障碍和流式联动已补齐。 |
| ✅ | X3 流式服务 | `ElementXStreamService`、`ElementXRequestService`、可选 `ElementXRecordService`。 | 已新增服务类、JS 能力入口、取消控制、离线降级和服务层单元测试。 |
| ✅ | X4 AI 模板复刻 | 用 `ElX*` 组件复刻 `ruoyi-element-ai` 主要页面。 | `AiWorkspace` 已走通新建/重命名/删除会话、附件、流式回复、历史切换和停止生成。 |
| ✅ | X5 文档站视觉 | 复刻 v2 Element Plus X 文档站首页和整体文档风格。 | 已完成首页、导航、组件页、X 矩阵信号和响应式样式的本地实现；后续只保留真实浏览器截图归档。 |
| ✅ | X6 发布门槛 | 稳定 API、测试矩阵、包说明。 | `Element.X` 独立包、README、NuGet readme、release workflow、静态资源路径、服务 API、组件测试和服务测试已完成。 |

## 当前结论

✅ Element.X 组件矩阵已经完成首轮闭环：展示、输入、附件、思考、流式请求、录音降级入口、demo 文档、AI 工作台和发布说明均有公开 API、示例联动和测试覆盖。

## 防偏约束

- 任何 X 组件 PR 都必须说明使用了哪些现有 `El*` 组件和 `--el-*` token。
- 不允许把上游 Vue、Vite、Pinia、Element Plus、Element Plus X、Floating UI、UnoCSS、VueUse 等依赖加入本仓库。
- 不允许将上游项目整包复制到 `demo`；只能用 Blazor 组件重新实现交互和视觉。
- `demo` 的 AI 演示必须默认可离线运行；真实后端只能作为可配置增强项。
- 每个阶段完成后更新本文档矩阵状态，并至少运行对应项目的构建或组件测试。

## 后续执行顺序

1. 新增 `ElXTypewriter`、`ElXBubble`、`ElXBubbleList` 和最小 CSS，先跑通聊天消息展示。
2. 新增 `ElXConversations`、`ElXWelcome`、`ElXPrompts`，形成静态 AI 首页。
3. 新增 `ElXFilesCard`、`ElXAttachments`、`ElXSender`，打通输入、附件和发送事件。
4. 新增 `ElXThinking`、`ElXThoughtChain`、流式模拟服务，补齐 AI 对话体验。
5. 在 `demo` 中复刻 ruoyi-element-ai，并改造首页/文档站视觉。
