# Element-Blazor

[![NuGet](https://img.shields.io/nuget/dt/Element.svg)](https://www.nuget.org/packages/Element/)

Element-Blazor 是一个基于 Blazor 的 UI 组件库，设计风格参考 Element。

## 项目定位

- 面向中文业务后台场景的 Blazor 组件集
- 优先保证常用组件可用性与开发效率
- 以示例驱动组件演进

当前版本仍处于持续整理和升级阶段，详见 [ROADMAP.md](ROADMAP.md)。

## 当前状态

- 组件库主项目：`src/Components/Element.csproj`
- 当前目标框架：`.NET 7`（后续将升级到 LTS）
- 示例与演示：`src/Samples`、`Element.Demo`
- 测试工程：`test/Element.Test`、`test/Blazui.Component.Test`

## 快速开始

### 运行本地示例

1. 使用 Visual Studio 2022 或 `dotnet` CLI 打开仓库
2. 还原依赖并构建解决方案
3. 启动示例项目（推荐从 `Element.ServerRender` 开始）

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
- 演示源码：`Element.Demo`

## 版本与计划

- 版本变更：见 [CHANGELOG.md](CHANGELOG.md)
- 发展计划：见 [ROADMAP.md](ROADMAP.md)

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
