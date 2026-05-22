# Element-Blazor

[![NuGet](https://img.shields.io/nuget/dt/Element.svg)](https://www.nuget.org/packages/Element/)

Element-Blazor is a Blazor UI component library aligned with [Element Plus](https://element-plus.org/). The current mainline targets Element Plus `2.14` conventions and uses public Razor component names such as `<ElButton>`, `<ElInput>`, and `<ElTable>`.

## Current Status

- Main package: `Element`
- Main project: `src/Components/Element.csproj`
- Markdown package: `Element.Markdown`
- Target framework: `net10.0`
- Version line: `2.14.0-alpha.1`
- License: MIT

## Install

```powershell
dotnet add package Element --prerelease
```

Register Element services:

```csharp
builder.Services.AddElementServices();
```

Load static assets:

```html
<link rel="stylesheet" href="/_content/Element/css/fix.css" />
<link rel="stylesheet" href="/_content/Element/css/index.css" />
<link rel="stylesheet" href="/_content/Element/css/theme.css" />
<script src="/_content/Element/js/dom.js"></script>
```

Use `El*` components:

```razor
<ElButton Type="@ButtonType.Primary">Primary</ElButton>
<ElInput TValue="string" Placeholder="Search" Clearable="true" />
```

## Build

Build the component library:

```powershell
dotnet restore src/Components/Element.csproj
dotnet build src/Components/Element.csproj -c Release
```

Build the full solution:

```powershell
dotnet restore Element-Blazor.sln
dotnet build Element-Blazor.sln -c Release
```

The `demo`, `template`, and `community` directories are Git submodules or showcase workspaces. Initialize them before validating the full solution:

```powershell
git submodule sync --recursive
git submodule update --init --recursive
```

## Release

Release preparation is documented in `docs/release-checklist.md`.

Before publishing, verify:

- NuGet metadata, license, icon, readme, symbols, and Source Link
- `dotnet list src/Components/Element.csproj package --vulnerable --include-transitive`
- `dotnet pack src/Components/Element.csproj -c Release`
- `dotnet build Element-Blazor.sln -c Release`

## Documentation

- Changelog: `CHANGELOG.md`
- Roadmap: `ROADMAP.md`
- Release checklist: `docs/release-checklist.md`
- Element Plus alignment: `docs/element-plus-alignment.md`

## Repository

- Source: https://github.com/Element-Blazor/Element-Blazor
- NuGet: https://www.nuget.org/packages/Element/
