# P0 项目基线

本文记录 Element-Blazor 2.14 主线的 P0 决策。后续路线图、README、对齐说明和任务清单必须与这里保持一致。

## 完成状态

| 项目 | 状态 | 结论 |
| --- | --- | --- |
| 版本基线 | 🟢 已完成 | Element-Blazor 当前进入 `2.14.0-alpha.1` 线，对齐 Element Plus `2.14.0`。 |
| 文档门面 | 🟢 已完成 | `README.md`、`README.en.md`、`ROADMAP.md`、`docs/element-plus-alignment.md` 已说明 Element Plus 2.14 主线、`El*` 命名和子模块定位。 |
| 构建基线 | 🟢 已完成 | `dotnet build src/Components/Element.csproj` 可通过。 |
| 警告清单 | 🟢 已完成 | 详见 [p0-warning-inventory.md](p0-warning-inventory.md)。 |
| 进度标记约定 | 🟢 已完成 | 所有文档中代表完成状态的任务必须使用 `🟢 已完成` 标记。 |

## .NET 10 主线决策

当前不需要先把整个仓库一次性全面升级到 .NET 10 才能继续。

| 范围 | 决策 |
| --- | --- |
| `src/Components/Element.csproj` | 保持 `net10.0`，这是组件库主线和后续新增代码的目标框架基线。 |
| 新增项目 | 新文档站、新 `ElementCommunity.*` 主线、新示例优先使用 .NET 10。 |
| `demo`、`template`、`community` 历史项目 | 不做原地大规模升级；它们作为迁移目标或业务素材，等 `El*` 控件入口与主题变量层稳定后再迁移。 |
| 依赖升级 | 先处理安全与最小风险 warning，再单独规划 Microsoft/ASP.NET Core 依赖向 10.x 对齐。 |

选择 .NET 10 的原因：

- 组件库主项目已经是 `net10.0`，且当前构建可通过。
- .NET 10 是 LTS 版本，Microsoft 官方支持策略显示其支持阶段为 Active，支持结束日期为 2028-11-14，适合作为 2.14 新主线和新增项目基线。
- 大规模 TFM/依赖升级和 P1/P2 的主题、控件命名切换应拆开处理，避免一个 PR 混入过多风险。

参考来源：[Microsoft .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy)。

## 当前执行顺序

1. 🟢 已完成：P0 版本基线、文档门面、构建基线和 warning 清单。
2. 🔵 待开始：P1 新增 Element Plus 2.14 CSS 变量层。
3. 🔵 待开始：P2 破坏性删除旧公开组件名，建立 `El*` 控件名。
4. 🔵 待开始：P3 先从 `ElButton`、`ElInput`、`ElForm` 做视觉和 API 对齐。
