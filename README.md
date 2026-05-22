# Element-Blazor

[![NuGet](https://img.shields.io/nuget/dt/Element.svg)](https://www.nuget.org/packages/Element/)

Element-Blazor 是一个面向 Blazor 的 UI 组件库。当前主线以 [element-plus/element-plus](https://github.com/element-plus/element-plus) 为主要设计、组件矩阵和版本基线。

## 项目定位

- 面向业务后台和真实产品界面的 Blazor 组件集。
- 当前对齐版本线为 Element Plus `2.14.0`。
- Element UI `2.15.x` 仅作为历史参考，不再作为目标路线。
- `demo`、`template`、`community` 是 Git 子模块，分别承载示例、模板和社区展示站。

## 当前状态

- 组件库主项目：`src/Components/Element.csproj`
- Markdown 项目：`src/Markdown/Element.Markdown.csproj`
- 当前主线框架：`.NET 10`
- 当前版本线：`2.14.0-alpha.1`
- 示例与演示：`demo`、`template/Samples`
- 社区展示：`community`

## 命名策略

- DOM/CSS class 保持 Element Plus 官方契约，例如 `el-button`、`el-input`、`el-table`、`is-disabled`、`el-button--primary`。
- Razor 组件统一使用 Element Plus 风格的 PascalCase `El*` 名称，例如 `<ElButton>`、`<ElInput>`、`<ElTable>`。
- 公共类型和基础设施使用 `Element*` 名称，例如 `ElementComponentBase`、`ElementDialogBase`、`ElementChangeEventArgs`。
- 旧的无前缀公共组件名已删除，不保留兼容层、不保留过渡别名。
- 旧的 B 前缀组件实现已删除，不保留继承包装、不保留别名、不作为内部实现继续存在。
- 旧包入口已切换到 `Element` / `Element.Markdown` / `Element.Admin` 系列。

## 快速开始

### 初始化子模块

```powershell
git clone --recurse-submodules https://github.com/Element-Blazor/Element-Blazor.git
```

如果已经克隆过仓库：

```powershell
git submodule sync --recursive
git submodule update --init --recursive
```

### 构建组件库

```powershell
dotnet restore src/Components/Element.csproj
dotnet build src/Components/Element.csproj
```

### 构建 Markdown

```powershell
dotnet build src/Markdown/Element.Markdown.csproj
```

## 文档

- 路线图：`ROADMAP.md`
- Element Plus 对齐说明：`docs/element-plus-alignment.md`
- 组件入口：`docs/p2-el-component-entrypoints.md`
- 社区重建计划：`docs/community-rebuild-plan.md`

