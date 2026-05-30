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
| 🟡 | BubbleList | `ElXBubbleList` | X1 | 已支持列表、空状态、加载占位、模板和基础测试；自动滚动等体验细节还需继续打磨。 |
| 🟡 | Conversations | `ElXConversations` | X1 | 已支持列表、分组、置顶、激活态和选择事件；新建、重命名、删除等完整会话操作仍需补齐。 |
| ✅ | Welcome | `ElXWelcome` | X1 | 已支持标题、描述、图标/头像、操作区和推荐内容。 |
| ✅ | Prompts | `ElXPrompts` | X1 | 已支持分组、图标、描述、禁用、选择事件和响应式布局。 |
| ✅ | FilesCard | `ElXFilesCard` | X2 | 已支持文件名、大小、类型图标、状态、进度、预览、删除、重试和错误展示。 |
| 🟡 | Attachments | `ElXAttachments` | X2 | 已基于 `ElUpload` 支持选择、拖拽、列表、限制和事件；上传状态映射、模板扩展和异常链路仍需增强。 |
| 🟡 | Sender / XSender | `ElXSender` | X2 | 已支持多行输入、发送/停止、加载态、工具区、附件区和回车提交；清空、快捷键细节和无障碍仍需补齐。 |
| 🟡 | MentionSender | `ElXMentionSender` | X2 | 已组合 `ElMention`/`ElXSender` 并支持基础提交；多前缀、候选选择回调和指令联动还没有完全闭环。 |
| ✅ | Thinking | `ElXThinking` | X2 | 已支持思考中/完成状态、折叠展开、持续时间和内容模板，并有 bUnit 覆盖。 |
| ✅ | ThoughtChain | `ElXThoughtChain` | X2 | 已基于现有时间线风格支持节点、状态、耗时和展开内容，并有 bUnit 覆盖。 |
| ⬜ | useRecord | `ElementXRecordService` | X3 | 尚未落地录音服务或浏览器能力降级。 |
| ⬜ | useXStream | `ElementXStreamService` | X3 | 尚未落地独立流式服务；demo 目前仍是本地模拟回复。 |
| ⬜ | useSend / XRequest | `ElementXRequestService` | X3 | 尚未落地请求服务、会话上下文、取消控制和服务层测试。 |

## Demo 复刻范围

| 状态 | 目标 | 路由/位置 | 当前判断 |
| --- | --- | --- | --- |
| 🟡 | X 组件文档 | `demo` 的 X 分类 | 已有 X 分类、`BasicX` 和 `AiWorkspace` 示例；还没有做到每个 `ElX*` 都有基础、组合、可控/事件示例。 |
| 🟡 | ruoyi-element-ai 演示 | `demo/XAi` 或同等路由 | `demo/X/AiWorkspace.razor` 已有离线工作台骨架；真实的左侧会话操作、顶部工具、流式回复、停止生成和消息操作还未完整复刻。 |
| 🟡 | 模拟服务 | `demo` 内本地类 | 当前示例可离线运行，但模拟逻辑仍写在页面内，尚未沉淀为本地服务层。 |
| 🟡 | 视觉首页 | demo 首页与站点样式 | 已有 X 入口和局部样式；尚未完成对 `v2.element-plus-x.com/zh/` 的首页/文档站级截图验收。 |

## 阶段计划

| 状态 | 阶段 | 目标 | 当前判断 |
| --- | --- | --- | --- |
| ✅ | X0 路线固化 | 固化本文档、主路线图入口、依赖禁令和组件矩阵。 | 已完成路线图入口和依赖约束；当前拆包后没有引入第三方 UI 依赖。 |
| 🟡 | X1 核心展示组件 | `Typewriter`、`Bubble`、`BubbleList`、`Conversations`、`Welcome`、`Prompts`。 | 首批组件、CSS、demo 示例和 bUnit 测试已落地；`BubbleList` 自动滚动、`Conversations` 完整会话操作和视觉对齐仍需补齐。 |
| 🟡 | X2 输入与思考组件 | `FilesCard`、`Attachments`、`ElXSender`、`ElXMentionSender`、`Thinking`、`ThoughtChain`。 | 首版组件入口和离线 AI 工作台组合已落地；上传状态、指令选择、清空/快捷键和流式联动仍需继续。 |
| ⬜ | X3 流式服务 | `ElementXStreamService`、`ElementXRequestService`、可选 `ElementXRecordService`。 | 未彻底开始：仓库内尚无对应服务类和服务层单元测试。 |
| 🟡 | X4 AI 模板复刻 | 用 `ElX*` 组件复刻 `ruoyi-element-ai` 主要页面。 | 已有 `AiWorkspace` 离线骨架；新建会话、流式回复、附件、历史切换、停止生成和消息操作还未完整走通。 |
| ⬜ | X5 文档站视觉 | 复刻 v2 Element Plus X 文档站首页和整体文档风格。 | 尚未做完整首页/导航/组件页/暗亮主题/响应式截图验收。 |
| 🟡 | X6 发布门槛 | 稳定 API、测试矩阵、包说明。 | `Element.X` 独立包、README、NuGet readme、release workflow、静态资源路径和基础测试已完成；API 稳定性、完整测试矩阵和发布前验收还需继续。 |

## 当前结论

🟡 Element.X 拆包与首批组件能力已经基本成型，但路线图没有彻底完成。当前最明确的缺口是 X3 服务层、完整 AI 模板复刻、每个组件的完整 demo 文档、视觉站点截图验收，以及若干组件的交互细节闭环。

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
