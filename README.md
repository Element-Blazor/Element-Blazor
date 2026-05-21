# Element-Blazor

[![NuGet](https://img.shields.io/nuget/dt/Element.svg)](https://www.nuget.org/packages/Element/)

Element-Blazor 是一个基于 Blazor 的 UI 组件库。当前主线目标已经切换为对齐 [element-plus/element-plus](https://github.com/element-plus/element-plus)：复刻 Element Plus 的后台产品设计语言、组件结构、交互反馈和文档体验。

## 项目定位

- 面向中文业务后台场景的 Blazor 组件集
- 优先保证常用组件可用性与开发效率
- 以示例、文档站和真实业务站点共同驱动组件演进
- 以 Element Plus `2.14.0` 作为当前视觉、组件矩阵和版本基线
- 旧 Element UI `2.15.x` 仅作为历史参考，不再作为目标路线

当前版本仍处于持续整理和升级阶段，详见 [ROADMAP.md](ROADMAP.md)。

## 当前状态

- 组件库主项目：`src/Components/Element.csproj`
- 当前目标框架：`.NET 10`
- 当前对齐版本线：`2.14.0-alpha.1`
- 示例与演示：`template/Samples`、`demo`
- 社区展示站：`community`，定位为类似 DiscuzX 的 Blazor 社区网站，用来展示 Element-Blazor 在真实业务站点中的使用方式
- 测试工程：`test/Element.Test`
- 当前仓库中 `demo`、`template` 与 `community` 是 Git 子模块；未初始化时只能构建组件库本体，完整解决方案会因示例项目缺失而失败。

## 命名策略

- DOM/CSS class 必须保持 Element Plus 官方契约：`el-button`、`el-input`、`el-table`、`is-disabled`、`el-button--primary`。
- 公开 Razor 组件统一使用 Element Plus 风格的 PascalCase `El*` 名称，例如 `<ElButton>`、`<ElInput>`、`<ElTable>`。
- `<el-button>` 带连字符，在 Razor 中会被当作普通 HTML/custom element，不适合作为 Blazor 组件类型；`el_` 也不符合 .NET 命名习惯，因此不采用。
- 旧公开组件名 `Button`、`Input`、`Table` 等直接删除，不保留兼容、不保留过渡别名。
- `B*` 仅允许作为临时内部实现名存在；路线目标是逐步内聚或重命名为 `El*` 实现。

## 快速开始

### 初始化子模块

本仓库已将 `demo`、`template` 与 `community` 拆分为 Git 子模块。

- `demo`：组件示例与在线文档数据源
- `template`：项目模板与示例宿主
- `community`：类 DiscuzX 社区网站，包含前台、后台、API、WASM 等项目

首次克隆请使用：

```powershell
git clone --recurse-submodules https://github.com/Element-Blazor/Element-Blazor.git
```

如果你已经克隆过仓库，请执行：

```powershell
git submodule sync --recursive
git submodule update --init --recursive
```

### 构建组件库本体

如果你只需要验证组件库本体，可以先运行：

```powershell
dotnet restore src/Components/Element.csproj
dotnet build src/Components/Element.csproj
```

### 运行本地示例与完整解决方案

1. 使用 Visual Studio 2022 或 `dotnet` CLI 打开仓库
2. 确认 `demo`、`template` 与 `community` 子模块已初始化
3. 还原依赖并构建解决方案
4. 启动示例项目（推荐从 `Element.ServerRender` 开始）

```powershell
dotnet restore
dotnet build Element-Blazor.sln
```

### 引用组件库（NuGet）

```powershell
dotnet add package Element --prerelease
```

## 文档与示例

- Element Plus 官方组件总览：https://element-plus.org/zh-CN/component/overview
- 在线示例（GitHub）：https://element-blazor.github.io/
- 在线示例（Gitee）：https://element-blazor.gitee.io/
- 演示源码：`demo`
- 社区源码：`community`

## 版本与计划

- 版本变更：见 [CHANGELOG.md](CHANGELOG.md)
- 发展计划：见 [ROADMAP.md](ROADMAP.md)
- Element Plus 对齐说明：见 [docs/element-plus-alignment.md](docs/element-plus-alignment.md)

## 当前路线

- 🟡 P0：门面、构建基线、子模块、版本基线和警告清单
- 🔵 P1：Element Plus 2.14 设计令牌、CSS 变量和核心组件对齐
- 🔵 P2：删除旧公开组件名，切换到 Element Plus `El*` 控件命名
- 🔵 P3：文档站与组件总览
- 🔵 P4：DiscuzX 类社区门面与真实业务示例
- 🔵 P5：补齐 Element Plus 组件矩阵缺口
- 🔵 P6：测试、发布和长期维护

## 参与贡献

欢迎提交 Issue 和 PR：

- GitHub Issues：https://github.com/Element-Blazor/Element-Blazor/issues
- 贡献指南：见 `CONTRIBUTING.md`

建议在提交 PR 前完成：

1. 最小化改动范围，避免无关重构
2. 为新增功能补充示例和必要测试
3. 更新对应文档和变更说明

## 致谢

- Element Plus 设计思想、组件规范与交互模式
- Element UI 历史生态提供的参考基础
- 所有历史贡献者与社区用户
