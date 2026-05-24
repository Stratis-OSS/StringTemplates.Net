---
name: Solution
description: Use when working on solution-wide concerns in the StringTemplates repository — editing `Directory.Build.props` or `Directory.Packages.props`, changing the target framework or C# language version, pinning a central NuGet version, scaffolding a new package or plugin project, or wiring projects into `StringTemplates.slnx`. TRIGGER on edits to root-level build files, the slnx, or `build-*.sh`. SKIP for code-only edits inside a single project that do not touch solution-wide settings.
usage: This skill is the source of truth for the StringTemplates solution layout, shared build settings, central NuGet package management, package metadata conventions, and the local pack scripts. It does not cover the runtime behaviour of the core library or its plugins — that belongs in the per-feature skills.
---

# Solution

The StringTemplates repository is a multi-project .NET solution that ships several NuGet packages: a core library and a family of plugin packages. Everything below is solution-wide — changes here ripple across every project at once.

## Layout

```
StringTemplates.slnx        Solution file (XML-based slnx format)
Directory.Build.props       Shared MSBuild settings for every project
Directory.Packages.props    Central NuGet package versions
build-core.sh               Cleans, builds, and packs the core library
build-plugin.sh             Cleans, builds, and packs a plugin (takes the plugin name as an argument)
Makefile                    Runs build-core.sh and build-plugin.sh for every plugin under src/plugins/

src/
  StringTemplates/                  The core library (ships as `StringTemplates`)
  plugins/
    StringTemplates.Plugins.<Name>/ One folder per official plugin

tests/
  StringTemplates.UnitTests/
  StringTemplates.IntegrationTests/
  StringTemplates.IntegrationTests.Common/
  plugins/
    <Name>/StringTemplates.Plugins.<Name>.UnitTests/

examples/
  StringTemplates.Demo/             Minimal ASP.NET host that wires up the library

skills/                             Agent skills (this skill lives here)
.agents/skills, .claude/skills      Symlinks to ./skills/ for local agent surfaces
```

## Shared build settings (`Directory.Build.props`)

`Directory.Build.props` at the repo root defines the MSBuild properties that every project inherits — target framework, C# language version, nullable annotations, implicit usings, and code-style enforcement. Concrete values live in the file; this skill only governs the *rule* that they live there centrally.

- Edit shared settings in `Directory.Build.props`. Do not duplicate them in individual `.csproj` files.
- Do not override an inherited property in a single `.csproj` unless that project genuinely needs a different value, and write a short XML comment explaining why.
- Code-style enforcement is on, so `.editorconfig` violations fail the build. Treat the editorconfig as part of the build contract.

## Central NuGet package management (`Directory.Packages.props`)

Central Package Management is enabled (`<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`). Pin every NuGet version in `Directory.Packages.props` once, then reference it from project files without a `Version` attribute.

```xml
<!-- Directory.Packages.props -->
<ItemGroup>
  <PackageVersion Include="<PackageName>" Version="<PinnedVersion>" />
</ItemGroup>
```

```xml
<!-- some .csproj -->
<ItemGroup>
  <PackageReference Include="<PackageName>" />
  <!-- never write Version="..." here -->
</ItemGroup>
```

Rules:
- `<PackageReference>` in any `.csproj` must never carry a `Version` attribute. The version belongs in `Directory.Packages.props`.
- Group entries by vendor/family with a comment header (e.g. `<!-- Microsoft -->`, `<!-- xUnit -->`). Keep entries inside a group alphabetical.
- To add a package: add a `<PackageVersion>` entry to the matching group, then add a `<PackageReference>` (no version) to the project that needs it.
- To bump a package: edit the `<PackageVersion Version="..." />` in one place.
- To remove a package: remove every `<PackageReference>` first, then the `<PackageVersion>`.

## Package metadata convention

Every shippable project (core and plugins) declares NuGet metadata in its `.csproj` and sets `<GeneratePackageOnBuild>true</GeneratePackageOnBuild>` so a `dotnet build -c Release` produces a `.nupkg` automatically. Test and example projects do not.

The required metadata block per shippable project:

```xml
<PropertyGroup>
  <PackageId>StringTemplates.{Suffix}</PackageId>
  <Title>StringTemplates.{Suffix}</Title>
  <Version>{Version}</Version>

  <Authors>Stratis-Dermanoutsos</Authors>
  <Company>Stratis-OSS</Company>

  <Copyright>Copyright © Stratis Dermanoutsos {Year}</Copyright>
  <PackageLicenseFile>LICENSE</PackageLicenseFile>

  <PackageProjectUrl>https://github.com/Stratis-OSS/StringTemplates.Net</PackageProjectUrl>
  <RepositoryUrl>https://github.com/Stratis-OSS/StringTemplates.Net</RepositoryUrl>

  <Description>{One-sentence description.}</Description>
  <PackageTags>{space-separated tags}</PackageTags>
  <PackageReadmeFile>README.md</PackageReadmeFile>
  <PackageReleaseNotes>* {What changed in this version.}</PackageReleaseNotes>

  <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
</PropertyGroup>

<ItemGroup>
  <None Include="{relative-path-to}/LICENSE" Pack="true" PackagePath="/"/>
  <None Include="{relative-path-to}/README.md" Pack="true" PackagePath="/"/>
</ItemGroup>
```

Notes:
- `<Version>` is per-project — each package can ship independently. Bump it in every `.csproj` you intend to release.
- The relative path to `LICENSE` and `README.md` depends on the project's depth in the tree (`../../` from `src/<Project>/`, `../../../` from `src/plugins/<Project>/`).

## Project naming

- Core library: `StringTemplates` (folder and assembly).
- Plugin libraries: folder `StringTemplates.Plugins.<Name>`, but the shipped `<PackageId>` and `<Title>` drop the `Plugins.` segment (e.g. folder `StringTemplates.Plugins.<Name>` → package `StringTemplates.<Name>`). Keep doing this for new plugins.
- Tests: `<Project>.UnitTests` for unit tests, `<Project>.IntegrationTests` for integration tests, plus `<Project>.IntegrationTests.Common` for shared integration-test fixtures.
- Plugin tests mirror the source layout under `tests/plugins/<Name>/`.

## The slnx

`StringTemplates.slnx` is the solution file. When you add a project or a top-level file you want visible in the IDE solution tree, register it as a `<Project Path="..."/>` or `<File Path="..."/>` under the appropriate `<Folder>` element. Keep the folder structure inside the slnx mirroring the on-disk layout (`/src/`, `/src/plugins/`, `/tests/`, `/tests/plugins/`, `/examples/`, plus `/Solution Items/` for root-level files and the agent surfaces).

## Build, test, pack

All commands run from the repo root and target `StringTemplates.slnx`.

```bash
dotnet restore StringTemplates.slnx
dotnet build StringTemplates.slnx
dotnet test StringTemplates.slnx

# Pack a single shippable project locally
./build-core.sh                                  # core library
./build-plugin.sh <PluginName>                   # one plugin, e.g. ./build-plugin.sh Configuration

# Pack core + every plugin under src/plugins/ via the Makefile
make                                             # = make all = make core + make plugins
make core                                        # just the core library
make plugins                                     # every plugin
make <PluginName>                                # one plugin by name
make list                                        # show the discovered targets
```

The two shell scripts each `cd` into one project, `dotnet clean`, then `dotnet build -c Release` and `dotnet pack -c Release`. `build-plugin.sh` takes the plugin's short name (the suffix after `StringTemplates.Plugins.`) and resolves the path itself. The `Makefile` discovers plugins automatically from `src/plugins/StringTemplates.Plugins.*` — a new plugin folder is picked up with no further wiring.

## How to scaffold a new plugin package

1. Create `src/plugins/StringTemplates.Plugins.<Name>/StringTemplates.Plugins.<Name>.csproj`.
2. Copy the metadata block from an existing plugin and replace `<PackageId>`, `<Title>`, `<Description>`, and the `<None Include="..."/>` paths if the folder depth differs.
3. Reference the core: `<PackageReference Include="StringTemplates"/>` (no version). Add any third-party dependencies the same way and pin them in `Directory.Packages.props`.
4. Add the implementation class — an `ITemplatePlugin` (or `ITemplatePlugin<TInput>`) under the `StringTemplates.Plugins.<Name>` namespace.
5. Register the new project in `StringTemplates.slnx` under `<Folder Name="/src/plugins/">`.
6. Add a matching test project under `tests/plugins/<Name>/StringTemplates.Plugins.<Name>.UnitTests/` and register it in the slnx under `<Folder Name="/tests/plugins/<Name>/">`.
7. Verify with `dotnet build StringTemplates.slnx` and `dotnet test StringTemplates.slnx`.
