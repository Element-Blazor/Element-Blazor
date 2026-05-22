# Element Plus 2.14 路线图

> 最终决策：Element-Blazor 主线对齐 Element Plus `2.14.0`。公开 Razor 组件统一使用 `El*`，基础设施统一使用 `Element*`。旧组件入口不兼容、不保留、不做别名。

## 本轮核对结论

核对时间：2026-05-22。

| 项目 | 状态 | 说明 |
| --- | --- | --- |
| 官方基线 | 已完成 | 以 Element Plus `2.14.0` 为当前视觉、组件矩阵和版本基线。 |
| 版本线 | 已完成 | `src/Components/Element.csproj` 使用 `2.14.0-alpha.1`，主项目为 `net10.0`。 |
| DOM class | 已完成 | 组件继续输出 `el-*` 与 `is-*` class 契约。 |
| Razor 组件名 | 已完成 | 活跃组件入口统一为 `El*`。 |
| 基础设施命名 | 已完成 | 基类、异常、事件参数、全局设置和服务扩展统一为 `Element*` / `AddElementServices`。 |
| 旧组件实现 | 已完成 | 旧 B 前缀组件实现、旧项目和旧包入口已删除，不留兼容层、继承包装或过渡别名。 |
| demo/template/community | 已完成 | 示例和模板中的活跃 Razor 标签已迁移为 `El*`。 |

## 命名规则

| 层级 | 规则 |
| --- | --- |
| DOM/CSS | 保持 Element Plus 官方 `el-*`、`is-*` class。 |
| Blazor 组件 | 使用 `El*` PascalCase，例如 `ElButton`、`ElInput`、`ElTable`、`ElFormItem`。 |
| 公共基础类型 | 使用 `Element*`，例如 `ElementComponentBase`、`ElementDialogBase`、`ElementChangeEventArgs`。 |
| 服务扩展 | 使用 `AddElementServices`。 |
| 旧组件入口 | 直接删除，不兼容、不保留、不做别名。 |
| `<el-button>` / `el_` | 不采用；Razor 不会把连字符标签解析为 C# 组件类型，`el_` 也不符合 .NET 命名习惯。 |

目标写法：

```razor
<ElButton Type="ButtonType.Primary" Loading="true">保存</ElButton>
<ElInput TValue="string" Placeholder="请输入" Clearable="true" />
<ElTable DataSource="@rows" />
```

## 阶段进度

| 阶段 | 状态 | 说明 |
| --- | --- | --- |
| P0 门面与版本基线 | 已完成 | README、路线图、对齐文档、warning 清单已建立。 |
| P1 Element Plus 主题变量 | 已完成 | `theme.css` 提供 `--el-*` 变量层，并接入示例入口。 |
| P2 组件命名切换 | 已完成 | 旧组件入口已清理，主线只使用 `El*` / `Element*`。 |
| P3 核心组件对齐 | 进行中 | `ElButton`、`ElInput`、`ElForm`、`ElDialog`、`ElTabs`、`ElMenu` 已推进；`ElSelect`、`ElTable` 继续补齐高级能力。 |
| P4 文档站与组件总览 | 待开始 | 建设组件总览、示例卡片和 API 表格。 |
| P5 组件矩阵补齐 | 待开始 | 按 Element Plus 矩阵继续补齐缺口组件。 |
| P6 社区展示站 | 进行中 | 旧社区项目作为业务素材来源，主线使用 `El*` 控件和 Element Plus 主题。 |
| P7 稳定化与发布 | 待开始 | 安全依赖、测试矩阵、NuGet 门面和发布节奏。 |

## P2 验收口径

- `src/Components` 不存在旧组件实现入口。
- `src/Markdown` 不存在旧 Markdown 组件入口。
- `demo`、`template`、`community` 的活跃 Razor 标签不使用旧组件标签。
- 不新增兼容包装、继承别名或旧包入口。
- 扫描旧组件 API 名称时，活跃代码应无命中；业务实体命名不属于组件清理范围。

