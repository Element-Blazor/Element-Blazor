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
| 本地承载 | `src/Components` 新增 `ElX*` 组件；`demo` 新增 X 组件文档、AI 模板演示和首页风格。 |
| 依赖边界 | 只依赖本仓库 Blazor Element 组件和 .NET/Blazor 基础能力。不得引入 Vue、Element Plus、Element Plus X npm 包、第三方 UI 组件库或外部前端运行时。 |

## 命名规则

- Razor 组件统一使用 `ElX*`，例如 `ElXBubble`、`ElXSender`、`ElXConversations`。
- 支撑模型统一使用 `X*`，例如 `XMessageItem`、`XConversationItem`、`XAttachmentItem`、`XPromptItem`。
- DOM/CSS class 使用 `el-x-*`，主题变量使用 `--el-x-*`，底层尺寸、颜色、圆角、阴影优先复用现有 `--el-*` token。
- 不提供 Vue 风格标签、npm 包别名或兼容包装。上游 Vue 代码只作为功能/API/交互参考，不复制实现。

## 组件矩阵

| 上游能力 | Element-Blazor 入口 | 阶段 | 验收口径 |
| --- | --- | --- | --- |
| Typewriter | `ElXTypewriter` | X1 | 支持纯文本/片段流式输出、打字速度、暂停/完成状态、手动完成。 |
| Bubble | `ElXBubble` | X1 | 支持用户/助手角色、头像、标题、时间、加载、打字、操作区、内容模板。 |
| BubbleList | `ElXBubbleList` | X1 | 支持消息列表、自动滚动、空状态、加载占位、消息模板、角色映射。 |
| Conversations | `ElXConversations` | X1 | 支持会话列表、新建/切换/重命名/删除、分组、置顶、激活态和操作插槽。 |
| Welcome | `ElXWelcome` | X1 | 支持标题、描述、图标/头像、操作区和推荐内容。 |
| Prompts | `ElXPrompts` | X1 | 支持提示词分组、图标、描述、禁用、点击选择、响应式布局。 |
| FilesCard | `ElXFilesCard` | X2 | 支持文件名、大小、类型图标、上传状态、进度、预览/删除/重试。 |
| Attachments | `ElXAttachments` | X2 | 基于本项目 `ElUpload` 实现附件选择、拖拽、列表、限制、事件和模板。 |
| Sender / XSender | `ElXSender` | X2 | 支持多行输入、发送/停止/清空、加载态、前后缀工具区、附件区、快捷键。 |
| MentionSender | `ElXMentionSender` | X2 | 基于 `ElMention`/`ElXSender` 支持 `/` 指令、候选列表、多前缀和选择事件。 |
| Thinking | `ElXThinking` | X2 | 支持思考中、已完成、折叠展开、持续时间、内容模板。 |
| ThoughtChain | `ElXThoughtChain` | X2 | 基于 `ElTimeline`/`ElSteps` 风格实现思考链节点、状态、耗时、展开内容。 |
| useRecord | `ElementXRecordService` | X3 | 作为 Blazor 服务/JS 轻封装暴露录音权限、开始、停止、结果事件；无浏览器能力时降级。 |
| useXStream | `ElementXStreamService` | X3 | 用 `HttpClient`/stream API 支持增量文本、取消、错误、完成回调。 |
| useSend / XRequest | `ElementXRequestService` | X3 | 提供发送请求、流式响应、会话上下文、取消控制；demo 默认使用模拟后端。 |

## Demo 复刻范围

| 目标 | 路由/位置 | 验收口径 |
| --- | --- | --- |
| X 组件文档 | `demo` 的 X 分类 | 每个 `ElX*` 至少有基础、组合、可控/事件示例，示例只使用本项目组件。 |
| ruoyi-element-ai 演示 | `demo/XAi` 或同等路由 | 左侧会话、顶部工具、聊天消息区、欢迎/提示词、附件、输入发送、流式回复、停止生成、消息操作均用 `ElX*` 组件搭建。 |
| 模拟服务 | `demo` 内本地类 | 默认不依赖真实 ruoyi-ai 后端；可选配置真实接口，但示例必须离线可运行。 |
| 视觉首页 | demo 首页与站点样式 | 首屏、导航、组件卡片、文档页面布局、色彩、间距和动效方向对齐 `v2.element-plus-x.com/zh/`，但实现使用 Blazor/CSS。 |

## 阶段计划

| 阶段 | 目标 | 完成标准 |
| --- | --- | --- |
| X0 路线固化 | 固化本文档、主路线图入口、依赖禁令和组件矩阵。 | 已完成：`ROADMAP.md` 引用本文档；新增 `.codex/x-component-guardrails.md`；不引入任何新包。 |
| X1 核心展示组件 | `Typewriter`、`Bubble`、`BubbleList`、`Conversations`、`Welcome`、`Prompts`。 | 进行中：首批组件、CSS、demo 示例、bUnit 测试已落地；继续补齐 API 细节和视觉对齐。 |
| X2 输入与思考组件 | `FilesCard`、`Attachments`、`ElXSender`、`ElXMentionSender`、`Thinking`、`ThoughtChain`。 | 进行中：已提供首版组件入口和离线 AI 工作台组合；后续补齐上传/指令/流式联动细节。 |
| X3 流式服务 | `ElementXStreamService`、`ElementXRequestService`、可选 `ElementXRecordService`。 | demo 可模拟 SSE/流式输出、取消和错误；服务层有单元测试。 |
| X4 AI 模板复刻 | 用 `ElX*` 组件复刻 `ruoyi-element-ai` 主要页面。 | 离线演示可完整走通：新建会话、发送、流式回复、附件、历史切换。 |
| X5 文档站视觉 | 复刻 v2 Element Plus X 文档站首页和整体文档风格。 | 首页、导航、组件页、暗色/亮色基础样式和响应式布局通过截图验收。 |
| X6 发布门槛 | 稳定 API、测试矩阵、包说明。 | 无新增第三方 UI 依赖；组件测试通过；README/PACKAGE_README 增加 X 说明。 |

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
