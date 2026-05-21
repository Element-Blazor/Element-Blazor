# Element-Blazor

[![NuGet](https://img.shields.io/nuget/dt/Element.svg)](https://www.nuget.org/packages/Element/)

Element-Blazor is a Blazor UI component library aiming to align with the Element Plus design language, component structure, and interaction patterns.

## Positioning

- A Blazor component set focused on business back-office scenarios
- Prioritizes practical usability and development efficiency for common components
- Evolves with demo-driven development
- Uses Element Plus 2.14.0 as the current visual and component-matrix alignment baseline

The project is currently under continuous refactoring and upgrade. See [ROADMAP.md](ROADMAP.md) for details.

## Current Status

- Main component project: `src/Components/Element.csproj`
- Current target framework: `.NET 10`
- Samples and demos: `template/Samples`, `demo`
- Community showcase: `community`, a DiscuzX-like Blazor community site used to demonstrate Element-Blazor in a real product surface
- Test project: `test/Element.Test`
- `demo`, `template`, and `community` are Git submodules. If they are not initialized, build the component library project directly; the full solution will fail because sample projects are missing.

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
dotnet add package Element
```

## Docs and Examples

- Online demo (GitHub): https://element-blazor.github.io/
- Online demo (Gitee): https://element-blazor.gitee.io/
- Demo source: `demo`
- Community source: `community`

## Versioning and Plan

- Version history: see [CHANGELOG.md](CHANGELOG.md)
- Project roadmap: see [ROADMAP.md](ROADMAP.md)
- Element Plus alignment notes: [docs/element-plus-alignment.md](docs/element-plus-alignment.md)

## Current Track

- 🟡 P0: project facade, build baseline, submodules, and warning inventory
- 🔵 P1: Element Plus 2.x design tokens and core component alignment
- 🔵 P2: documentation site and component overview
- 🔵 P3: DiscuzX-like community facade and real product showcase
- 🔵 P4: component matrix gap filling
- 🔵 P5: testing, release, and long-term maintenance

## Contributing

Issues and pull requests are welcome:

- GitHub Issues: https://github.com/Element-Blazor/Element-Blazor/issues
- Contribution guide: `CONTRIBUTING.md`

Before submitting a PR, please:

1. Keep the change scope minimal and avoid unrelated refactoring
2. Add examples and necessary tests for new features
3. Update relevant documentation and change notes

## Acknowledgements

- Element UI design concepts and interaction patterns
- All contributors and community users
