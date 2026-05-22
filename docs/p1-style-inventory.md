# P1 Style Inventory And Layering

This inventory records how the legacy style files are treated after the Element Plus 2.14 token layer landed.

## Loading Order

Use this order for applications and templates:

```html
<link rel="stylesheet" href="/_content/Element/css/fix.css" />
<link rel="stylesheet" href="/_content/Element/css/index.css" />
<link rel="stylesheet" href="/_content/Element/css/theme.css" />
```

`fix.css` still loads first because it contains Element-Blazor layout compatibility fixes. `index.css` remains the legacy theme-chalk baseline. `theme.css` loads last so `--el-*` variables and migrated Element Plus 2.14 selectors override hard-coded legacy values without changing DOM classes.

## File Decisions

| File | Decision | Notes |
| --- | --- | --- |
| `src/Components/wwwroot/css/index.css` | Keep as legacy baseline for now. | It is the large theme-chalk bundle and still carries many component selectors that have not been migrated to variables. Do not add new patch styles here. |
| `src/Components/wwwroot/css/theme.css` | Primary Element Plus 2.14 token layer. | New component visual work should prefer `--el-*` variables here and keep selectors compatible with `el-*` and `is-*`. |
| `src/Components/wwwroot/css/fix.css` | Keep only compatibility/layout fixes. | It should stay focused on Element-Blazor layout gaps, old table flex fixes, upload/ribbon/groupbox glue, and scrollbar compatibility. Do not place new design tokens here. |
| Template host pages | Load `theme.css` after `index.css`. | Server, WebAssembly, PWA, and SEO samples now load the token layer. |

## Migration Rules

- New visual work goes to `theme.css` unless it is a narrow layout compatibility fix.
- Migrate hard-coded colors, borders, radius, shadows, spacing, and component sizes to `--el-*` variables.
- Keep public DOM classes stable: `el-*` and `is-*` selectors remain the contract.
- Move legacy one-off fixes out of `fix.css` only when the target component has an equivalent variable-backed rule in `theme.css`.
- Leave `index.css` as an upstream baseline until the component surface has enough variable-backed replacements to safely split or regenerate it.

## Current Tokenized Surfaces

- Global color, text, background, fill, border, radius, shadow, transition, size, overlay, and z-index tokens.
- Button, Input, Table, Select, Dialog, Tabs, Menu, Popper, Select dropdown, table loading and empty states.
- Compatibility size aliases for existing Element-Blazor values: medium and mini.

## Layering Rules

Element Plus 2.14 uses `--el-index-*` as the shared z-index vocabulary. Element-Blazor now follows that vocabulary in `theme.css`:

| Token | Default | Use |
| --- | --- | --- |
| `--el-index-normal` | `1` | Component-local stacking such as table internals. |
| `--el-index-top` | `1000` | Sticky/high local surfaces. |
| `--el-index-popper` | `2000` | Popper, select dropdown, menu popups, dialog overlays. |

The popup renderer still allocates per-instance z-index numbers in C# for ordering multiple active overlays. CSS surfaces should use `--el-index-popper`, `--el-box-shadow`, `--el-box-shadow-light`, or component-specific aliases so Dialog, Dropdown, Select, Popover and Tooltip can share the same visual stack when their component implementations are added or completed.
