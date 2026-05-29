# X Component Guardrails

Before implementing Element Plus X or AI demo work in this repository, read `docs/x-component-roadmap.md` and keep these rules active:

- Public X components use `ElX*`; support models use `X*`; services use `ElementX*`.
- Implement in Blazor using the existing Element-Blazor component library and CSS tokens.
- Do not add Vue, Element Plus, Element Plus X npm packages, third-party UI component libraries, or external frontend runtimes.
- Use upstream Element Plus X and ruoyi-element-ai only as feature, API, and visual references; do not copy their Vue implementation into this repo.
- The demo AI template must run offline by default with local mock data/services; real backend integration can only be optional.
- Every finished X component needs focused demo coverage and bUnit coverage proportional to interaction risk.
