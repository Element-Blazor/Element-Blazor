# Element-Blazor

[![NuGet](https://img.shields.io/nuget/dt/Element.svg)](https://www.nuget.org/packages/Element/)

Element-Blazor is a Blazor UI component library inspired by the Element design style.

## Positioning

- A Blazor component set focused on business back-office scenarios
- Prioritizes practical usability and development efficiency for common components
- Evolves with demo-driven development

The project is currently under continuous refactoring and upgrade. See [ROADMAP.md](ROADMAP.md) for details.

## Current Status

- Main component project: `src/Components/Element.csproj`
- Current target framework: `.NET 7` (planned upgrade to LTS)
- Samples and demos: `src/Samples`, `demo`
- Test project: `test/Element.Test`

## Quick Start

### Run local samples

1. Open the repository with Visual Studio 2022 or `dotnet` CLI
2. Restore dependencies and build the solution
3. Start a sample project (recommended: `Element.ServerRender`)

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

## Versioning and Plan

- Version history: see [CHANGELOG.md](CHANGELOG.md)
- Project roadmap: see [ROADMAP.md](ROADMAP.md)

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
