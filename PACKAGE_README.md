# Element-Blazor

Element-Blazor is a Blazor component library aligned with Element Plus 2.14.

## Install

```powershell
dotnet add package Element --prerelease
```

## Usage

Register services:

```csharp
builder.Services.AddElementServices();
```

Load static assets in your host page or Blazor Web App root:

```html
<link rel="stylesheet" href="/_content/Element/css/fix.css" />
<link rel="stylesheet" href="/_content/Element/css/index.css" />
<link rel="stylesheet" href="/_content/Element/css/theme.css" />
<script src="/_content/Element/js/dom.js"></script>
```

Use Element Plus-style `El*` Razor components:

```razor
<ElButton Type="@ButtonType.Primary">Primary</ElButton>
<ElInput TValue="string" Placeholder="Search" Clearable="true" />
```

## Package Notes

- Target framework: `net10.0`
- Version line: `2.14.0-alpha.1`
- License: MIT
- Repository: https://github.com/Element-Blazor/Element-Blazor
- Changelog: https://github.com/Element-Blazor/Element-Blazor/blob/main/CHANGELOG.md
