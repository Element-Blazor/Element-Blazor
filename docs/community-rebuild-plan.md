# Community 重构蓝图

> 最终决策：`community` 子模块不做旧项目兼容升级，而是作为业务素材库，按 Element Plus 2.14、Element-Blazor `El*` 控件命名和现代 .NET 主线彻底重构。旧 UI、旧组件名、旧包名、旧启动方式、旧 `B*` 写法不保留。

## 审计结论

| 区域 | 当前事实 | 新主线处理 |
| --- | --- | --- |
| 项目年代 | 主体为 `netcoreapp3.1`、`netstandard2.1`、Blazor WASM 3.2 | 新建现代 .NET 目标框架项目，不在旧项目上修修补补。 |
| 前台 | `BlazorCommunity.App` Server-side Blazor，含首页、搜索、帖子详情、发帖、账号页、移动页 | 作为页面与业务流程参考，重建新前台。 |
| WASM | `BlazorCommunity.Api` 托管 `BlazorCommunity.WasmApp` | 是否保留 WASM 形态重新评估；第一阶段只建一个可运行主展示面。 |
| 后台 | `BlazorCommunity.Admin` 依赖 `Element.Admin` 与旧 `BAdmin` | 不保留旧 Admin 框架，重建 Element Plus 风格管理后台。 |
| API | `BlazorCommunity.Api` 有 client/admin 控制器，覆盖用户、主题、回复、关注、横幅、版本、上传 | 保留接口语义作为参考，重建 API 边界和 DTO。 |
| 数据库 | MySQL，含 `BZTopic`、`BZReply`、`BZUser`、`BZFollow`、`BZVersion`、`BzBanner` 等模型 | 保留领域模型方向，重新设计实体、迁移、种子数据。 |
| 登录 | Server 前台使用 ASP.NET Identity cookie；WASM/API 使用 token/localStorage；后台另有 IdentityUser | 统一认证方案，先实现本地账号 cookie/JWT 二选一，再扩展。 |
| 配置 | 硬编码 MySQL 地址、旧域名、旧包名 | 全部迁移到本地安全配置、样例配置和启动脚本。 |
| 组件 | 大量 `BButton`、`BForm`、`BTable`、`BMarkdownEditor` | 全部迁移为 `El*`，旧组件名不出现。 |

## 新目标

把 `community` 打造成类似 DiscuzX 的 Element-Blazor 真实业务展示站：

- 首页：版块导航、主题流、置顶/精华/热门、搜索、右侧信息栏。
- 版块：分类、主题列表、排序、分页、过滤。
- 帖子详情：楼主内容、回复列表、引用回复、编辑、关闭、收藏。
- 发帖：标题、分类、版本/标签、Markdown 内容、附件/图片上传。
- 用户中心：资料、头像、签名、我的主题、我的回复、我的收藏、安全设置。
- 通知：回复通知、系统消息、审核结果。
- 管理后台：用户、角色、主题、回复、版块、横幅、版本、举报/审核。
- 文档展示：该站本身作为 Element-Blazor 2.14 业务样板。

## 新架构建议

| 层 | 建议项目 | 说明 |
| --- | --- | --- |
| Host | `ElementCommunity.App` | 首阶段推荐 Blazor Server 或 Blazor Web App，先跑通一个主站。 |
| Components | `ElementCommunity.Components` | 社区专属组件，如 TopicList、ReplyList、UserCard，只使用 `El*`。 |
| Api | `ElementCommunity.Api` | 如果采用前后端分离或 WASM，再独立 API；第一阶段可与 Host 合并。 |
| Domain | `ElementCommunity.Domain` | Topic、Reply、UserProfile、Forum、Notification 等领域实体。 |
| Infrastructure | `ElementCommunity.Infrastructure` | EF Core、文件存储、邮件、缓存、搜索。 |
| Admin | `ElementCommunity.Admin` | 第二阶段拆出后台或作为 Host 内 `/admin` 区域。 |
| Seed | `ElementCommunity.Seed` | 本地演示数据、账号、主题、回复、版块。 |

第一阶段不要同时铺开 Server、WASM、Admin、IdentityServer。先把“可运行社区主站 + SQLite/MySQL 本地数据 + `El*` UI”跑起来。

## 启动链路规划

### 前台链路

1. `/` 首页读取版块、主题流、热门主题、活跃用户。
2. `/forum/{slug}` 查看版块主题列表。
3. `/topic/{id}` 查看帖子详情和回复。
4. `/topic/new` 登录后发帖。
5. `/search?q=` 搜索主题。
6. `/user/{id}` 用户公开主页。
7. `/account/signin`、`/account/register` 登录注册。
8. `/me` 用户中心。

### API 链路

| 能力 | 路由建议 |
| --- | --- |
| 主题 | `GET /api/topics`、`GET /api/topics/{id}`、`POST /api/topics`、`PUT /api/topics/{id}` |
| 回复 | `GET /api/topics/{id}/replies`、`POST /api/topics/{id}/replies` |
| 版块 | `GET /api/forums`、`POST /api/admin/forums` |
| 用户 | `GET /api/users/{id}`、`PUT /api/me/profile` |
| 收藏/关注 | `POST /api/topics/{id}/favorite`、`DELETE /api/topics/{id}/favorite` |
| 上传 | `POST /api/uploads/images` |
| 后台 | `GET /api/admin/topics`、`PATCH /api/admin/topics/{id}/status` |

### 数据库链路

首阶段实体：

- `Forum`：版块。
- `Topic`：主题帖。
- `Reply`：回复。
- `UserProfile`：用户扩展资料。
- `TopicFavorite`：收藏。
- `Notification`：通知。
- `Attachment`：附件/图片。

第二阶段实体：

- `Role`、`Permission`、`AuditLog`、`Banner`、`Tag`、`TopicView`、`Report`。

### 登录链路

第一阶段：

- 使用 ASP.NET Core Identity。
- 本地注册、登录、退出、当前用户。
- 种子账号：管理员、普通用户、版主。
- 发帖、回帖、收藏、后台入口需要认证。

第二阶段：

- 加入 JWT 或 BFF/API 分离。
- 邮箱验证、找回密码、第三方登录。

## 分阶段任务

### C0 决策与清场 🟡

| 任务 | 验收 |
| --- | --- |
| 明确旧 `community` 为素材库，不作为兼容升级目标 | 文档写明不保留旧组件名、旧包名、旧启动方式。 |
| 选定首阶段技术形态 | 推荐 Blazor Server/Web App + EF Core + SQLite 本地基线。 |
| 建立新项目命名 | `ElementCommunity.*`，避免继续使用 `BlazorCommunity` 历史包名。 |
| 冻结旧项目改动 | 旧项目只读参考，不继续修小问题。 |

### C1 可运行骨架 🔵

| 任务 | 验收 |
| --- | --- |
| 新建 Host/Domain/Infrastructure/Components | `dotnet build` 通过。 |
| 接入 Element-Blazor 2.14 本地项目引用 | 页面只使用 `El*` 控件名。 |
| 建立首页、主题详情、发帖页、登录页空壳 | 可本地启动并导航。 |
| 建立本地启动脚本 | 一条命令启动主站，输出 URL。 |

### C2 数据与账号 🔵

| 任务 | 验收 |
| --- | --- |
| 建立 EF Core DbContext 与迁移 | SQLite/MySQL 二选一先跑通。 |
| 加入 Identity 与种子账号 | 管理员、版主、普通用户可登录。 |
| 种子版块/主题/回复 | 首页和详情页有真实数据。 |
| 发帖/回帖最小闭环 | 登录用户能发帖、回帖。 |

### C3 Element Plus 主题融合 🔵

| 任务 | 验收 |
| --- | --- |
| 引入 Element Plus 2.14 主题变量 | 社区使用统一 `--el-*` token。 |
| 建立社区布局组件 | Header、ForumNav、TopicList、TopicCard、ReplyList、UserCard。 |
| 替换所有旧样式口径 | 不出现 `BButton`、`BForm`、`BTable` 等旧写法。 |
| 视觉对齐 | 后台产品密度、边框、圆角、hover、active 与 Element Plus 接近。 |

### C4 DiscuzX 信息架构 🔵

| 任务 | 验收 |
| --- | --- |
| 版块首页 | 版块、主题数、回复数、最后回复。 |
| 主题列表 | 置顶、精华、问答/分享、排序、分页。 |
| 帖子详情 | 楼层、回复、引用、编辑、关闭、收藏。 |
| 用户中心 | 资料、头像、我的帖子、我的回复、我的收藏。 |
| 搜索 | 标题搜索、分页、高亮或摘要。 |

### C5 管理后台 🔵

| 任务 | 验收 |
| --- | --- |
| 后台布局 | Element Plus 风格侧栏、顶部栏、面包屑。 |
| 用户管理 | 查询、禁用、角色、重置密码。 |
| 内容管理 | 主题、回复、置顶、加精、关闭、删除/恢复。 |
| 版块管理 | 新增、排序、隐藏、权限。 |
| 审核与日志 | 举报、操作日志、审核状态。 |

### C6 API/WASM 扩展 🔵

| 任务 | 验收 |
| --- | --- |
| 是否拆 API | 根据主站稳定度决定是否拆 `ElementCommunity.Api`。 |
| 是否保留 WASM | 作为移动/轻客户端演示，而不是第一阶段目标。 |
| OpenAPI | 后台和前台接口有 Swagger。 |
| 上传服务 | 图片/附件本地存储，可替换云存储。 |

### C7 稳定化 🔵

| 任务 | 验收 |
| --- | --- |
| Playwright 冒烟测试 | 首页、登录、发帖、回帖、后台登录。 |
| 本地演示数据重置 | 一条命令重建数据库和种子数据。 |
| 文档 | README 写清启动、账号、端口、数据库。 |
| 发布 | 可作为 Element-Blazor 门面示例站部署。 |

## 立即执行顺序

1. C0-1：在仓库中冻结旧 `community`，把它标为参考素材。
2. C1-1：创建新的 `community-next` 或 `community/src/ElementCommunity.*` 骨架。
3. C1-2：接入当前 Element-Blazor 项目引用，确保只使用 `El*`。
4. C1-3：做静态首页、主题详情、发帖页三个页面。
5. C2-1：加入 DbContext、Identity、种子数据。
6. C2-2：跑通登录、发帖、回帖闭环。
7. C3-1：统一 Element Plus 2.14 主题变量和社区布局。
8. C5-1：再做管理后台，不先动旧 `BlazorCommunity.Admin`。

## 任务提示词

### C1 新骨架

```text
请在 community 内建立新的 ElementCommunity 主线骨架，不复用旧 BlazorCommunity 项目文件。目标是现代 .NET、Element-Blazor 2.14、公开控件只使用 `El*`。先创建可运行 Host、Domain、Infrastructure、Components 基础结构，做首页、帖子详情、发帖页静态骨架，并提供本地启动命令。
```

### C2 数据闭环

```text
请为新的 ElementCommunity 骨架加入 EF Core DbContext、Identity、SQLite 本地开发数据库和种子数据。首批实体包括 Forum、Topic、Reply、UserProfile、TopicFavorite、Notification、Attachment。跑通注册/登录、发帖、回帖、首页列表、帖子详情。
```

### C3 主题融合

```text
请把新的 ElementCommunity 主站接入 Element Plus 2.14 主题变量和 Element-Blazor `El*` 控件。要求页面里不出现旧 `B*`/无前缀旧组件名；Header、ForumNav、TopicList、TopicCard、ReplyList、UserCard 统一使用 `El*` 和 `el-*` DOM class。
```

### C5 管理后台

```text
请在新的 ElementCommunity 主线中实现管理后台，不复用旧 BlazorCommunity.Admin。后台使用 Element Plus 风格布局和 `El*` 控件，实现用户、角色、版块、主题、回复、举报/审核、操作日志的第一版页面骨架。
```
