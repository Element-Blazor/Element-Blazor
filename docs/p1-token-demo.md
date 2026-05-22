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

The demo intentionally uses CSS variables directly through `var(--el-*)`, plus existing `El*` components, so it verifies both the design-token layer and the public component naming rule.
