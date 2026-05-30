# ✨ Element-Blazor

[![NuGet Version](https://img.shields.io/nuget/v/Element?label=Element&logo=nuget&color=409eff)](https://www.nuget.org/packages/Element/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Element?label=downloads&logo=nuget&color=67c23a)](https://www.nuget.org/packages/Element/)
[![Markdown Version](https://img.shields.io/nuget/v/Element.Markdown?label=Element.Markdown&logo=nuget&color=e6a23c)](https://www.nuget.org/packages/Element.Markdown/)
[![GitHub Release](https://img.shields.io/github/v/release/Element-Blazor/Element-Blazor?label=release&logo=github)](https://github.com/Element-Blazor/Element-Blazor/releases)
[![License](https://img.shields.io/github/license/Element-Blazor/Element-Blazor?label=license)](LICENSE)

Element-Blazor 是一个对齐 [Element Plus](https://element-plus.org/) 设计语言的 Blazor UI 组件库。当前主线面向 `net10.0`，使用 `<ElButton>`、`<ElInput>`、`<ElTable>` 等 `El*` Razor 组件，让 .NET 团队可以直接用 Blazor 构建现代前端体验。

## 🚀 快速开始

安装主包：

```powershell
dotnet add package Element --prerelease
```

注册服务：

```csharp
builder.Services.AddElementServices();
```

加载静态资源：

```html
<link rel="stylesheet" href="/_content/Element/css/fix.css" />
<link rel="stylesheet" href="/_content/Element/css/index.css" />
<link rel="stylesheet" href="/_content/Element/css/theme.css" />
<script src="/_content/Element/js/dom.js"></script>
```

使用组件：

```razor
<ElButton Type="@ButtonType.Primary">Primary</ElButton>
<ElInput TValue="string" Placeholder="Search" Clearable="true" />
```

## 🧩 包与链接

| 包 | NuGet | 下载量 | 用途 |
| --- | --- | --- | --- |
| `Element` | [![NuGet](https://img.shields.io/nuget/v/Element?label=version)](https://www.nuget.org/packages/Element/) | [![Downloads](https://img.shields.io/nuget/dt/Element?label=downloads)](https://www.nuget.org/packages/Element/) | Element 风格 Blazor 组件库 |
| `Element.Markdown` | [![NuGet](https://img.shields.io/nuget/v/Element.Markdown?label=version)](https://www.nuget.org/packages/Element.Markdown/) | [![Downloads](https://img.shields.io/nuget/dt/Element.Markdown?label=downloads)](https://www.nuget.org/packages/Element.Markdown/) | Markdown 编辑器组件 |

## 🌈 项目状态

- 📦 主包：`Element`
- 🧱 主项目：`src/Components/Element.csproj`
- ✍️ Markdown 包：`Element.Markdown`
- ⚙️ 目标框架：`net10.0`
- 🏷️ 当前版本线：`2.14.0-alpha.1`
- 📜 开源协议：MIT
- 🧭 演示站：<https://element-blazor.github.io/>

## 🤖 X 组件与 AI Demo

项目路线已经把 Element Plus X 风格组件纳入长期规划，目标是只使用当前仓库内的 Blazor Element 组件实现 AI 对话、发送器、会话列表、思考过程、富内容和 RuoYi 风格 AI 工作台 demo。

## 🛠️ 构建

```powershell
dotnet restore Element-Blazor.sln
dotnet build Element-Blazor.sln -c Release
```

`demo`、`template`、`community` 是子模块或展示工作区，完整验证前请初始化：

```powershell
git submodule sync --recursive
git submodule update --init --recursive
```

## 🚢 发布

创建 `v*` tag 会触发组件发布 workflow：

```powershell
git tag v2.14.0-alpha.1
git push origin v2.14.0-alpha.1
```

流水线会根据 tag 去掉 `v` 后的版本号打包 `Element` 和 `Element.Markdown`，使用组织 Secret `NUGET_KEY` 发布到 NuGet，并同步创建 GitHub Release。

## 📚 文档

- 📝 Changelog: [`CHANGELOG.md`](CHANGELOG.md)
- 🛣️ Roadmap: [`ROADMAP.md`](ROADMAP.md)
- ✅ Release checklist: [`docs/release-checklist.md`](docs/release-checklist.md)
- 🎨 Element Plus alignment: [`docs/element-plus-alignment.md`](docs/element-plus-alignment.md)

## 💬 社区

欢迎在演示站 About 页面扫码加入企业微信交流群，也欢迎通过 GitHub Issues 一起推进组件、文档、主题和 AI 场景能力。
