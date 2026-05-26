# P1 Token Demo

The token demo is available as a real Razor demo:

- `demo/Theme/DesignTokens.razor`
- route: `/theme` in the sample hosts
- data entry: `theme` in `demo/demos.json`

It shows these token groups:

- Colors: primary, success, warning, danger, info.
- Typography: base, medium, large and extra-large sizes.
- Border radius, border color and shadow samples.
- Component size tokens and live Button/Input/Table examples.

The Basic category also exposes token-only Element Plus entries directly:

- `/border` shows `--el-border`, radius and shadow usage.
- `/color` shows status color palettes.
- `/typography` shows font family, size, line-height and text color tokens.

The demo intentionally uses CSS variables directly through `var(--el-*)`, plus existing `El*` components, so it verifies both the design-token layer and the public component naming rule.
