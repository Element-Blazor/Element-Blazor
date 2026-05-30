# Changelog

All notable changes to Element-Blazor are recorded here.

This project follows Semantic Versioning and uses a Keep a Changelog style structure.

## [Unreleased]

### Added

- Added release stabilization checklist in `docs/release-checklist.md`.
- Added NuGet package readme files for `Element`, `Element.Markdown`, and `Element.X`.
- Added Source Link and `.snupkg` symbol package settings for packable projects.
- Added independent `Element.X` package for optional AI and conversation components.
- Added `ElementXStreamService`, `ElementXRequestService`, and `ElementXRecordService` with offline stream, cancellation, browser recording fallback, and service tests.

### Changed

- Updated the Element-Blazor 2.14 package metadata for NuGet publishing.
- Updated package license metadata to use the MIT license expression.
- Updated package documentation file output paths to the `net10.0` target framework.
- Replaced the unreadable changelog contents with a release-ready changelog baseline.
- Moved X components and `x.css` out of the core `Element` package.

### Fixed

- Fixed the malformed `Element.Markdown.csproj` package description XML.
- Removed stale .NET Framework-era package references from `Element.csproj`.
- Updated ASP.NET Core and Microsoft.Extensions package references used by `Element.csproj` to the .NET 10 package line.

## [2.14.0-alpha.1] - Unreleased

### Added

- Added `ROADMAP.md` for the Element Plus alignment plan.
- Added Element Plus 2.14 theme variable layer at `/_content/Element/css/theme.css`.
- Added documentation for the Element Plus alignment and component entry points.

### Changed

- Moved the main component library to the `.NET 10` baseline.
- Aligned public Razor component names with Element Plus-style `El*` names.
- Removed legacy public component names and compatibility aliases from the active mainline.

## [0.0.7.2] - 2023-02-14

### Changed

- Historical text and repository metadata updates.

## [0.0.7.1] - Historical

### Notes

- Earlier historical changes were not fully curated into this repository changelog.
