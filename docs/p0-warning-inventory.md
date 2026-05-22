# P0 Warning 清单

采集时间：2026-05-22，Asia/Shanghai。

## 构建结果

| 项目 | 状态 | 说明 |
| --- | --- | --- |
| 组件库构建 | 已验证 | `dotnet build src/Components/Element.csproj` 可构建，仍存在 NuGet、编译和 analyzer warning。 |
| Markdown 构建 | 已验证 | `dotnet build src/Markdown/Element.Markdown.csproj` 可构建，仍存在 warning。 |
| 命名清理 | 已完成 | warning 清单中的文件名和修复建议以当前 `El*` / `Element*` 主线为准。 |

## NuGet 安全

| 优先级 | 包 | 当前 | 风险 | 最小风险修复 |
| --- | --- | --- | --- | --- |
| P0-1 | `Microsoft.AspNetCore.Components` | `7.0.2` | `NU1902` | 先升级到已修复的 7.0 patch，后续再独立评估 10.x。 |

## 过期依赖

| 包 | 建议 |
| --- | --- |
| `Microsoft.AspNetCore.Components` / `Microsoft.AspNetCore.Components.Web` | 先处理安全 patch，再评估主版本升级。 |
| `Microsoft.Extensions.*` | 等安全项和 `NU1510` 清理后再成组升级。 |
| `System.Net.Http.Json` | 验证当前 `ElTable`、`ElTree` 使用场景后再决定是否删除显式引用。 |
| `CompareNETObjects` | 非主线安全项，放在 Microsoft 包之后小步升级。 |

## 编译与 Analyzer

| warning | 当前位置 | 修复方向 |
| --- | --- | --- |
| `CS8632` | `src/Components/ElTableColumn.razor.cs` | 局部去掉 nullable 标记，或单独评估启用 nullable。 |
| `CS0108` | `src/Components/ElTable.razor.cs` | 删除重复注入，直接使用基类属性。 |
| `CS0659` / `CS0661` | `src/Components/TableHeader.cs` | 为 `IntString` 补充 `GetHashCode`。 |
| `CS0649` | `src/Components/ElMenuItem.razor.cs` | 初始化导航状态，或删除无用字段。 |
| `CA2200` | `src/Components/ElInput.razor.cs` | 使用 `throw;` 保留堆栈。 |
| `BL0007` | `src/Components/ElCheckbox.razor.cs` | 参数改为 auto-property，将派生状态移到渲染逻辑。 |
| `BL0005` | `src/Components/ControlRenders/TableRender.cs`、`src/Components/ElTable.razor.cs` | 避免从组件外部直接写组件参数，改为内部状态或事件驱动。 |
| `CS1573` | `src/Components/ElTree.razor.cs` | 补充缺失 XML 参数注释。 |

## 修复顺序

1. 处理 `Microsoft.AspNetCore.Components` 安全 patch。
2. 删除 `NU1510` 指出的不必要 `PackageReference`，每组删除后构建验证。
3. 修复低风险编译 warning：`CA2200`、`CS1573`、`CS8632`、`CS0108`。
4. 修复 `IntString.GetHashCode` 与导航状态字段。
5. 处理 Blazor analyzer，避免外部直接写组件参数。
6. 补齐 NuGet package readme 门面。

