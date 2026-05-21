# Element-Blazor

[![NuGet](https://img.shields.io/nuget/dt/Element.svg)](https://www.nuget.org/packages/Element/)

Element-Blazor 是一个基于 Blazor 的 UI 组件库，目标是对齐 Element Plus 的后台产品设计语言、组件结构和交互体验。

## 项目定位

- 面向中文业务后台场景的 Blazor 组件集
- 优先保证常用组件可用性与开发效率
- 以示例驱动组件演进
- 以 Element Plus 2.14.0 作为当前视觉与组件矩阵对齐基准

当前版本仍处于持续整理和升级阶段，详见 [ROADMAP.md](ROADMAP.md)。

## 当前状态

- 组件库主项目：`src/Components/Element.csproj`
- 当前目标框架：`.NET 10`
- 示例与演示：`template/Samples`、`demo`
- 社区展示站：`community`，定位为类似 DiscuzX 的 Blazor 社区网站，用来展示 Element-Blazor 在真实业务站点中的使用方式
- 测试工程：`test/Element.Test`
- 当前仓库中 `demo`、`template` 与 `community` 是 Git 子模块；未初始化时只能构建组件库本体，完整解决方案会因示例项目缺失而失败。

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
dotnet add package Element
```

## 文档与示例

- 在线示例（GitHub）：https://element-blazor.github.io/
- 在线示例（Gitee）：https://element-blazor.gitee.io/
- 演示源码：`demo`
- 社区源码：`community`

## 版本与计划

- 版本变更：见 [CHANGELOG.md](CHANGELOG.md)
- 发展计划：见 [ROADMAP.md](ROADMAP.md)
- Element Plus 对齐说明：见 [docs/element-plus-alignment.md](docs/element-plus-alignment.md)

## 当前路线

- 🟡 P0：门面、构建基线、子模块和警告清单
- 🔵 P1：Element Plus 2.x 设计令牌与核心组件对齐
- 🔵 P2：文档站与组件总览
- 🔵 P3：DiscuzX 类社区门面与真实业务示例
- 🔵 P4：补齐组件矩阵缺口
- 🔵 P5：测试、发布和长期维护

## 参与贡献

欢迎提交 Issue 和 PR：

- GitHub Issues：https://github.com/Element-Blazor/Element-Blazor/issues
- 贡献指南：见 `CONTRIBUTING.md`

建议在提交 PR 前完成：

1. 最小化改动范围，避免无关重构
2. 为新增功能补充示例和必要测试
3. 更新对应文档和变更说明

## 致谢

- Element UI 设计思想与交互规范
- 所有历史贡献者与社区用户
