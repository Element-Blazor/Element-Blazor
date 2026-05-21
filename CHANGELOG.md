# Changelog

本项目的版本变更记录统一维护在此文件中。

格式参考 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.0.0/)，版本遵循 [SemVer](https://semver.org/lang/zh-CN/)。

## [Unreleased]

### Added

- 新增 `ROADMAP.md`，明确 Element Plus 对齐路线、进度标记、阶段任务和可复用任务提示词
- 新增 `docs/element-plus-alignment.md`，沉淀 Element Plus 2.14.0 设计语言、命名策略、现状审计和改造原则
- 新增 `docs/p0-project-baseline.md`，记录 P0 版本基线、文档门面、.NET 10 主线和进度标记约定
- 新增 `docs/p0-warning-inventory.md`，归档组件库构建 warning、安全依赖、过期依赖和最小风险修复顺序
- 新增 `community` Git 子模块，接入 `Element-Blazor/BlazorCommunity` 作为 DiscuzX 类社区展示站基础
- 新增 `CHANGELOG.md`，建立版本变更记录基线
- 新增 `RELEASE_TEMPLATE.md`，提供统一发布说明模板

### Changed

- 更新 `README.md`，补充 Element Plus 2.14 对齐目标、组件库本体构建方式、子模块状态、命名策略和阶段路线
- 更新 `README.en.md`，并与中文 README 结构保持一致
- 更新解决方案 Solution Items，增加 `community` 子模块入口
- 将 Element 相关包版本线调整为 `2.14.0-alpha.1`，以 Element Plus `2.14.0` 为当前对齐基线
- 明确 `src/Components/Element.csproj` 以 .NET 10 为主线基线，历史子模块不需要先全面原地升级才进入 P1/P2
- 将 P0：版本基线、文档门面和 warning 清单标记为 🟢 已完成，并约定文档中的完成项必须使用表情状态标记
- 明确下一阶段为破坏性命名切换：旧公开组件名直接删除，公开控件统一改为 Element Plus 风格 `El*`

## [0.0.7.2] - 2023-02-14

### Changed

- 调整部分文字描述（基于仓库提交记录）

## [0.0.7.1] - 历史版本

### Notes

- 历史版本变更未完整沉淀到仓库文档，后续将逐步补齐。

---

## 维护约定

1. 每次发布前将 `Unreleased` 内容归档到对应版本。
2. 每条变更尽量标注类型：`Added`、`Changed`、`Fixed`、`Removed`。
3. PR 合并时同步更新此文件，避免发布后补录。
