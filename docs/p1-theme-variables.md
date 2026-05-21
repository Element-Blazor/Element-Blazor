# P1 Theme Variables

This layer aligns Element-Blazor with the Element Plus `2.14.0` theme-token model without changing the existing `el-*` DOM class contract.

## Loading Order

Load `theme.css` after the legacy `index.css` file:

```html
<link rel="stylesheet" href="/_content/Element/css/index.css" />
<link rel="stylesheet" href="/_content/Element/css/theme.css" />
```

For the legacy package name, use:

```html
<link rel="stylesheet" href="/_content/Blazui.Component/css/index.css" />
<link rel="stylesheet" href="/_content/Blazui.Component/css/theme.css" />
```

Applications can override tokens after `theme.css`:

```css
:root {
    --el-color-primary: #0057d9;
    --el-color-primary-light-3: #4d91e8;
    --el-color-primary-light-5: #80abec;
    --el-color-primary-light-7: #b3cdf3;
    --el-color-primary-light-8: #ccddf7;
    --el-color-primary-light-9: #e6eefb;
    --el-color-primary-dark-2: #0046ae;
    --el-border-radius-base: 6px;
    --el-component-size: 34px;
}
```

## Token Scope

The new stylesheet centralizes these Element Plus-compatible variables:

- Color: `--el-color-*`, text colors, background colors and fill colors.
- Typography: `--el-font-size-*`, `--el-font-family`, font weight and line-height tokens.
- Radius, border and shadow: `--el-border-*`, `--el-border-radius-*`, `--el-box-shadow-*`.
- Component sizing: `--el-component-size-large`, `--el-component-size`, `--el-component-size-small`, plus Element-Blazor compatibility aliases `--el-component-size-medium` and `--el-component-size-mini`.
- Component tokens for migrated surfaces: `--el-button-*`, `--el-input-*`, `--el-table-*`.

## Migration Examples

### Button

Before:

```css
.el-button {
    background: #fff;
    border: 1px solid #dcdfe6;
    color: #606266;
    padding: 12px 20px;
    border-radius: 4px;
}

.el-button--primary {
    background-color: #409eff;
    border-color: #409eff;
    color: #fff;
}
```

After:

```css
.el-button {
    --el-button-bg-color: var(--el-fill-color-blank);
    --el-button-border-color: var(--el-border-color);
    --el-button-text-color: var(--el-text-color-regular);
    --el-button-size: var(--el-component-size);
    background-color: var(--el-button-bg-color);
    border: var(--el-border);
    border-color: var(--el-button-border-color);
    color: var(--el-button-text-color);
    height: var(--el-button-size);
    border-radius: var(--el-border-radius-base);
}

.el-button--primary {
    --el-button-bg-color: var(--el-color-primary);
    --el-button-border-color: var(--el-color-primary);
    --el-button-text-color: var(--el-color-white);
}
```

### Input

Before:

```css
.el-input__inner {
    background-color: #fff;
    border: 1px solid #dcdfe6;
    color: #606266;
    height: 40px;
    border-radius: 4px;
}

.el-input__inner:focus {
    border-color: #409eff;
}
```

After:

```css
.el-input {
    --el-input-height: var(--el-component-size);
    --el-input-bg-color: var(--el-fill-color-blank);
    --el-input-border-color: var(--el-border-color);
    --el-input-focus-border-color: var(--el-color-primary);
    --el-input-text-color: var(--el-text-color-regular);
}

.el-input__inner {
    background-color: var(--el-input-bg-color);
    border: var(--el-border);
    border-color: var(--el-input-border-color);
    color: var(--el-input-text-color);
    height: var(--el-input-height);
    border-radius: var(--el-input-border-radius);
}

.el-input__inner:focus {
    border-color: var(--el-input-focus-border-color);
}
```

### Table

Before:

```css
.el-table {
    background-color: #fff;
    color: #606266;
    font-size: 14px;
}

.el-table td,
.el-table th.is-leaf {
    border-bottom: 1px solid #ebeef5;
}

.el-table--enable-row-hover .el-table__body tr:hover > td {
    background-color: #f5f7fa;
}
```

After:

```css
.el-table {
    --el-table-bg-color: var(--el-fill-color-blank);
    --el-table-text-color: var(--el-text-color-regular);
    --el-table-border-color: var(--el-border-color-lighter);
    --el-table-border: var(--el-border-width) var(--el-border-style) var(--el-table-border-color);
    --el-table-row-hover-bg-color: var(--el-fill-color-light);
    background-color: var(--el-table-bg-color);
    color: var(--el-table-text-color);
    font-size: var(--el-font-size-base);
}

.el-table td,
.el-table th.is-leaf {
    border-bottom: var(--el-table-border);
}

.el-table--enable-row-hover .el-table__body tr:hover > td {
    background-color: var(--el-table-row-hover-bg-color);
}
```

These examples keep selectors such as `el-button`, `el-input__inner` and `el-table__row` unchanged, so tests and downstream CSS that depend on the existing DOM structure continue to target the same classes.
