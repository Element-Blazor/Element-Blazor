# Element.X

Element.X provides optional AI and conversation scene components for Element-Blazor.

## Install

```powershell
dotnet add package Element.X --prerelease
```

## Requirements

Install and configure the main Element-Blazor package first:

```powershell
dotnet add package Element --prerelease
```

Load the Element core assets, then load the Element.X stylesheet:

```html
<link rel="stylesheet" href="/_content/Element.X/css/x.css" />
```

Use `Element.X` components and models:

```razor
<ElXBubbleList Items="@messages" />
<ElXSender @bind-Value="@draft" OnSubmit="SendAsync" />
```

## Package Notes

- Target framework: `net10.0`
- Version line: `2.14.0-alpha.1`
- License: MIT
- Repository: https://github.com/Element-Blazor/Element-Blazor
- Changelog: https://github.com/Element-Blazor/Element-Blazor/blob/main/CHANGELOG.md
