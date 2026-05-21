# Element-Blazor

[![NuGet](https://img.shields.io/nuget/dt/Element.svg)](https://www.nuget.org/packages/Element/)

Element-Blazor is a Blazor UI component library. The project now targets [element-plus/element-plus](https://github.com/element-plus/element-plus) as its primary design and component baseline.

## Positioning

- A Blazor component set focused on business back-office scenarios
- Prioritizes practical usability and development efficiency for common components
- Evolves through demos, documentation, and real product surfaces
- Uses Element Plus `2.14.0` as the current visual, component-matrix, and version baseline
- Element UI `2.15.x` is kept only as a historical reference

The project is currently under continuous refactoring and upgrade. See [ROADMAP.md](ROADMAP.md) for details.

## Current Status

- Main component project: `src/Components/Element.csproj`
- Current target framework: `.NET 10`
- Current alignment version line: `2.14.0-alpha.1`
- Samples and demos: `template/Samples`, `demo`
- Community showcase: `community`, a DiscuzX-like Blazor community site used to demonstrate Element-Blazor in a real product surface
- Test project: `test/Element.Test`
- `demo`, `template`, and `community` are Git submodules. If they are not initialized, build the component library project directly; the full solution will fail because sample projects are missing.

## Naming Strategy

- DOM/CSS classes must keep the official Element Plus contract: `el-button`, `el-input`, `el-table`, `is-disabled`, `el-button--primary`.
- Public Razor components use Element Plus-style PascalCase `El*` names, such as `<ElButton>`, `<ElInput>`, and `<ElTable>`.
- Hyphenated `<el-button>` is treated as a plain HTML/custom element by Razor and is not suitable as a Blazor component type. `el_` is also not idiomatic for .NET, so it is not used.
- Legacy public component names such as `Button`, `Input`, and `Table` are removed directly. There is no compatibility layer and no transition alias.
- `B*` names may remain only as temporary internal implementation names. The roadmap target is to internalize or rename them into `El*` implementations.

## Quick Start

### Initialize submodules

This repository now uses Git submodules for `demo`, `template`, and `community`.

- `demo`: component demos and online docs data source
- `template`: project templates and sample hosts
- `community`: DiscuzX-like community site with frontend, admin, API, and WASM projects

For a fresh clone, use:

```powershell
git clone --recurse-submodules https://github.com/Element-Blazor/Element-Blazor.git
```

If you already cloned the repository, run:

```powershell
git submodule sync --recursive
git submodule update --init --recursive
```

### Build the component library

If you only need to validate the component library, run:

```powershell
dotnet restore src/Components/Element.csproj
dotnet build src/Components/Element.csproj
```

### Run local samples and the full solution

1. Open the repository with Visual Studio 2022 or `dotnet` CLI
2. Make sure the `demo`, `template`, and `community` submodules are initialized
3. Restore dependencies and build the solution
4. Start a sample project (recommended: `Element.ServerRender`)

```powershell
dotnet restore
dotnet build Element-Blazor.sln
```

### Install from NuGet

```powershell
dotnet add package Element --prerelease
```

## Docs and Examples

- Element Plus component overview: https://element-plus.org/en-US/component/overview
- Online demo (GitHub): https://element-blazor.github.io/
- Online demo (Gitee): https://element-blazor.gitee.io/
- Demo source: `demo`
- Community source: `community`

## Versioning and Plan

- Version history: see [CHANGELOG.md](CHANGELOG.md)
- Project roadmap: see [ROADMAP.md](ROADMAP.md)
- Element Plus alignment notes: [docs/element-plus-alignment.md](docs/element-plus-alignment.md)

## Current Track

- 🟡 P0: project facade, build baseline, submodules, version baseline, and warning inventory
- 🔵 P1: Element Plus 2.14 design tokens, CSS variables, and core component alignment
- 🔵 P2: remove legacy public component names and switch to Element Plus `El*` control names
- 🔵 P3: documentation site and component overview
- 🔵 P4: DiscuzX-like community facade and real product showcase
- 🔵 P5: Element Plus component matrix gap filling
- 🔵 P6: testing, release, and long-term maintenance

## Contributing

Issues and pull requests are welcome:

- GitHub Issues: https://github.com/Element-Blazor/Element-Blazor/issues
- Contribution guide: `CONTRIBUTING.md`

Before submitting a PR, please:

1. Keep the change scope minimal and avoid unrelated refactoring
2. Add examples and necessary tests for new features
3. Update relevant documentation and change notes

## Acknowledgements

- Element Plus design concepts, component specs, and interaction patterns
- Element UI historical ecosystem as reference material
- All contributors and community users
