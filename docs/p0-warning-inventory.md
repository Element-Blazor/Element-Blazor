# P0 Warning 清单

采集时间：2026-05-22，Asia/Shanghai。

采集命令：

```powershell
dotnet build src/Components/Element.csproj
dotnet list src/Components/Element.csproj package --outdated
dotnet list src/Components/Element.csproj package --vulnerable --include-transitive
```

## 构建结果

| 项目 | 状态 | 说明 |
| --- | --- | --- |
| 组件库构建 | 🟢 已完成 | `dotnet build src/Components/Element.csproj` 成功，输出 `src/Components/bin/Debug/net10.0/Element.dll`。 |
| NuGet 包生成 | 🟢 已完成 | 成功生成 `Element.2.14.0-alpha.1.nupkg`。 |
| warning 归档 | 🟢 已完成 | build 汇总打印 28 个 warning；其中 NuGet warning 因 restore/build/pack 阶段重复出现，去重后有 18 个构建 warning 需要处理。 |

## NuGet 安全

| 优先级 | 项 | 当前 | 风险 | 最小风险修复 |
| --- | --- | --- | --- | --- |
| P0-1 | `Microsoft.AspNetCore.Components` | `7.0.2` | `NU1902`，Moderate，CVE-2023-36558 / GHSA-3fx3-85r4-8j3w | 先升到修复线 `7.0.14` 或更高 7.0 patch，并同步验证 `Microsoft.AspNetCore.Components.Web` 是否需要同线 patch；不要在同一步把所有 Microsoft 包大规模升到 10.x。 |

说明：项目目标框架已经是 `net10.0`，但 P0 的安全修复仍应先做最小补丁验证。Microsoft/ASP.NET Core 包全面对齐 10.x 应作为独立升级任务进入 P7 或单独 PR。

## 过期依赖

`dotnet list package --outdated` 当前列出以下顶级包：

| 包 | 当前 | 最新 | 建议顺序 |
| --- | --- | --- | --- |
| `Microsoft.AspNetCore.Components` | `7.0.2` | `10.0.8` | 已被安全项覆盖；先最小安全 patch，再单独评估 10.x。 |
| `Microsoft.AspNetCore.Components.Web` | `7.0.2` | `10.0.8` | 与 `Microsoft.AspNetCore.Components` 成组处理。 |
| `Microsoft.Extensions.Configuration.Abstractions` | `7.0.0` | `10.0.8` | 等安全项和 `NU1510` 清理后，再成组升级到 10.x。 |
| `Microsoft.Extensions.Http` | `7.0.0` | `10.0.8` | 等安全项和 `NU1510` 清理后，再成组升级到 10.x。 |
| `System.Net.Http.Json` | `7.0.0` | `10.0.8` | 同时命中 `NU1510`，优先验证能否删除显式引用。 |
| `CompareNETObjects` | `4.79.0` | `4.84.0` | 非主线安全项，放在 Microsoft 包之后小步升级并跑构建。 |

## NuGet 引用清理

| warning | 包 | 位置 | 修复建议 |
| --- | --- | --- | --- |
| `NU1510` | `Microsoft.CSharp` | `src/Components/Element.csproj` | 验证源码没有显式依赖后删除 PackageReference。 |
| `NU1510` | `System.Collections` | `src/Components/Element.csproj` | `System.Collections` 命名空间来自目标框架，不需要 NuGet 包；删除 PackageReference 后构建。 |
| `NU1510` | `System.Net.Http.Json` | `src/Components/Element.csproj` | 当前 `BTable`、`BTree` 使用 `GetFromJsonAsync`；在 `net10.0` 下通常由共享框架提供。先删除引用并构建验证。 |
| `NU1510` | `System.Reflection.Extensions` | `src/Components/Element.csproj` | 验证无显式依赖后删除 PackageReference。 |

## C# 编译与代码质量

| warning | 位置 | 原因 | 最小风险修复 |
| --- | --- | --- | --- |
| `CS8632` | `src/Components/BTableColumn.razor.cs:14` | 在未启用 nullable 上下文的项目里使用 `string?`。 | 局部去掉 `?`，或单独评估启用 nullable；P0 建议局部去掉。 |
| `CS0108` | `src/Components/BTable.razor.cs:322` | `BTable.LoadingService` 隐藏基类 `BComponentBase.LoadingService`。 | 删除重复注入，直接使用基类属性；若确实需要隐藏则显式 `new`，但删除更小风险。 |
| `CS0659` | `src/Components/TableHeader.cs:34` | `IntString` 重写 `Equals` 但未重写 `GetHashCode`。 | 按 `StringValue`、`IntValue`、`numberValue` 增加 `GetHashCode`。 |
| `CS0661` | `src/Components/TableHeader.cs:34` | `IntString` 定义 `==`/`!=` 但未重写 `GetHashCode`。 | 与 `CS0659` 同步修复。 |
| `CS0649` | `src/Components/BMenuItem.razor.cs:46` | `currentRoute` 只读未赋值。 | 在导航初始化/LocationChanged 中赋值，或确认无用后删除字段并调整 `OldValue` 来源。 |
| `CA2200` | `src/Components/BInput.razor.cs:154` | `throw e;` 会重置堆栈。 | 改为 `throw;`。 |

## Blazor Analyzer

| warning | 位置 | 原因 | 最小风险修复 |
| --- | --- | --- | --- |
| `BL0007` | `src/Components/BCheckBox.razor.cs:159` | `[Parameter] IsDisabled` 不是 auto-property，setter 带副作用。 | 改为 auto-property，并把 `is-disabled` class 派生逻辑移到渲染/计算属性。 |
| `BL0005` | `src/Components/ControlRenders/TableRender.cs:68` | 从组件外部直接设置 `arg.Table.DataSource` 参数。 | 给 `BTable` 增加内部方法更新数据源，或把该场景改为 `DataSourceChanged` 驱动。 |
| `BL0005` | `src/Components/BTable.razor.cs:578` | 从组件外部直接设置 `BButton.IsLoading` 参数。 | 给 `BButton` 增加内部 loading 状态方法，或改为局部状态驱动渲染。 |
| `BL0005` | `src/Components/BTable.razor.cs:583` | 同上。 | 与上一项同批处理。 |
| `BL0005` | `src/Components/BTable.razor.cs:624` | 从组件外部直接设置 `BButton.IsLoading` 参数。 | 与按钮 loading 状态方案同批处理。 |
| `BL0005` | `src/Components/BTable.razor.cs:628` | 同上。 | 与按钮 loading 状态方案同批处理。 |

## 文档注释

| warning | 位置 | 原因 | 最小风险修复 |
| --- | --- | --- | --- |
| `CS1573` | `src/Components/BTree.razor.cs:151` | `ExpandAsync(TreeItemBase treeItem, bool autoRefresh = true)` 的 XML 注释缺少 `autoRefresh`。 | 补充 `<param name="autoRefresh">...</param>`。 |

## 文档门面与 NuGet 包提示

| 项 | 状态 | 修复建议 |
| --- | --- | --- |
| package readme | 🔵 待开始 | build/pack 输出提示 NuGet 包缺少自述文件。建议先复用仓库 `README.md`，在 `Element.csproj` 中增加 `PackageReadmeFile` 并把 README 作为 pack 内容。 |

## 最小风险修复顺序

1. P0-1：安全最小补丁。只处理 `Microsoft.AspNetCore.Components` 的 `NU1902`，优先 patch 到已修复 7.0 线，并同步验证 `Components.Web`。
2. P0-2：删除 `NU1510` 指出的四个不必要 PackageReference，每删一组跑一次 `dotnet build src/Components/Element.csproj`。
3. P0-3：修低风险代码 warning：`CA2200`、`CS1573`、`CS8632`、`CS0108`。
4. P0-4：修 `IntString.GetHashCode` 与 `currentRoute`，这两项需要补充轻量行为验证。
5. P0-5：处理 Blazor analyzer。先修 `BCheckBox.IsDisabled`，再设计 `BTable/BButton` 内部状态更新方式，避免外部直接写组件参数。
6. P0-6：处理 package readme 门面。
7. P7 独立任务：Microsoft/ASP.NET Core/Extensions 包统一评估升级到 10.x，不与 P1 CSS 变量层、P2 控件命名切换混做。

