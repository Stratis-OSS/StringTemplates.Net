# AGENTS Guidelines for this repository

This repository hosts **StringTemplates.Net** — an open-source family of .NET NuGet packages for replacing placeholders in template strings. It is composed of a core library, a set of plugin packages that add new placeholder sources, and the demo / test projects that exercise them.

The skills under `./skills/` are the canonical source for repeated, structure-sensitive tasks in this repo. Open the matching skill before making non-trivial changes; this file is the one-page orientation that points at them.

## Repository layout

```
StringTemplates.slnx        Solution file (slnx format)
Directory.Build.props       Shared MSBuild settings for every project
Directory.Packages.props    Central NuGet package versions
build-core.sh               Cleans, builds, and packs the core library
build-plugin.sh             Cleans, builds, and packs a plugin (edit the path inside)

src/
  StringTemplates/                  Core library (ships as `StringTemplates`)
  plugins/
    StringTemplates.Plugins.<Name>/ One folder per official plugin

tests/
  StringTemplates.UnitTests/                Unit tests for the core library
  StringTemplates.IntegrationTests/         Integration tests against the demo host
  StringTemplates.IntegrationTests.Common/  Shared integration-test fixtures
  plugins/<Name>/StringTemplates.Plugins.<Name>.UnitTests/

examples/
  StringTemplates.Demo/             Minimal ASP.NET host that wires up the library

skills/                             Agent skills (canonical location)
.agents/skills, .claude/skills      Symlinks to ./skills/ for per-agent surfaces
docs-reference/                     Git submodule pointing at the GitHub wiki
```

## Build, test, run

All commands run from the repo root and target `StringTemplates.slnx`.

- Restore: `dotnet restore StringTemplates.slnx`
- Build: `dotnet build StringTemplates.slnx`
- Run all tests: `dotnet test StringTemplates.slnx`
- Run a single test project: `dotnet test tests/<path>/<Project>.csproj`
- Run the demo: `dotnet run --project examples/StringTemplates.Demo/StringTemplates.Demo.csproj`
- Pack the core library locally: `./build-core.sh`
- Pack a plugin locally: `./build-plugin.sh` (edit the `cd` path inside to target a different plugin)

Shippable projects (core and plugins) set `<GeneratePackageOnBuild>true</GeneratePackageOnBuild>`, so a Release build produces `.nupkg` artifacts automatically.

## Agent skills

Project-local skills live under `./skills/<slug>/SKILL.md`. Each skill is a **single file** — no `references/` folder, no nested assets. Skills are designed to ship as part of a Claude plugin (`.claude-plugin`) so end users can install them.

- `skill-creation` — how to author a new skill: where to put it, frontmatter contract, body style, and the rule that every skill is one self-contained `SKILL.md`.
- `solution` — solution-wide concerns: `Directory.Build.props`, `Directory.Packages.props`, central package management, package metadata conventions, the slnx, and the local pack scripts.

Add new skills as `./skills/<slug>/SKILL.md`. The `.agents/skills` and `.claude/skills` symlinks expose them to each agent surface automatically — no per-agent registration step is required in this repo.

## Per-agent configuration

Each supported agent reads its own configuration files; all of them defer to the skills under `./skills/` as the source of truth, so a skill edit reaches every agent without duplicate work.

- **Claude Code** — `.claude/settings.json` (permissions, hooks, statusline), hook scripts in `.claude/hooks/`, statusline in `.claude/statusline.py`. Skills reach Claude via the `.claude/skills` symlink.
- **GitHub Copilot** — `.github/copilot-instructions.md` (single file; a short pointer to this `AGENTS.md` plus the hard rules).
- **AGENTS.md-only agents** (Codex, Aider, Cursor without rules, etc.) — read this file directly; no per-tool directory needed.

## Hard rules

These are enforced by either the build, a pre-commit / pre-tool hook, or human review. Break them and the build fails or the change is reverted.

- **No `Version` on `<PackageReference>`.** Central Package Management is on. Every NuGet version is pinned once in `Directory.Packages.props`. A `<PackageReference Version="...">` anywhere triggers `NU1008` at restore time and is blocked at edit time by `.claude/hooks/block-package-version.py`.
- **Code style is build-enforced.** `EnforceCodeStyleInBuild` is on, so `.editorconfig` violations fail the build. Treat the editorconfig as part of the build contract.
- **No local-only files in git.** `.DS_Store`, `*.user`, `*.local.json`, and `settings.local.json` must never be committed. `.claude/hooks/block-local-files-commit.py` refuses any `git add` / `git commit` that touches them.
- **Files end in a single trailing newline.** UTF-8, no BOM unless the file already had one.

## Conventions

- **Centralised project settings.** `Directory.Build.props` defines target framework, language version, nullable annotations, implicit usings, and code-style enforcement for every project. Do not duplicate or override these in an individual `.csproj` without a clear reason.
- **Centralised package versions.** Pin every NuGet version in `Directory.Packages.props`, grouped by vendor/family with a comment header and alphabetical within the group. Project files reference packages without a `Version` attribute.
- **Project naming.**
  - Core library: `StringTemplates`.
  - Plugin folder: `StringTemplates.Plugins.<Name>`. The shipped `<PackageId>` and `<Title>` drop the `Plugins.` segment (folder `StringTemplates.Plugins.<Name>` → package `StringTemplates.<Name>`).
  - Tests: `<Project>.UnitTests` for unit tests, `<Project>.IntegrationTests` for integration tests, plus `<Project>.IntegrationTests.Common` for shared integration-test fixtures. Plugin tests mirror the source layout under `tests/plugins/<Name>/`.
- **One plugin, one concern.** Each official plugin ships as a separate package and adds one placeholder source (e.g. an `IConfiguration` reader, a MailKit integration). No kitchen-sink plugins.
- **Package metadata is consistent across shippable projects.** Every core/plugin `.csproj` declares `<PackageId>`, `<Title>`, `<Version>`, `<Authors>Stratis-Dermanoutsos</Authors>`, `<Company>Stratis-OSS</Company>`, copyright, license file, project + repository URLs, a one-sentence `<Description>`, tags, release notes, and `<GeneratePackageOnBuild>true</GeneratePackageOnBuild>`. Each shippable project also `<None Include="…/LICENSE" Pack="true" />` and the README from the repo root.

## Testing

- **Framework.** xUnit (`xunit` + `xunit.runner.visualstudio`), with `Shouldly` for assertions and `coverlet.collector` for coverage.
- **Unit tests** live next to the project they cover (`tests/StringTemplates.UnitTests/`, `tests/plugins/<Name>/StringTemplates.Plugins.<Name>.UnitTests/`). They exercise one class at a time with mocks where needed.
- **Integration tests** (`tests/StringTemplates.IntegrationTests/`) run against the demo host via `WebApplicationFactory`. Shared fixtures (`ApiFactory`, common test data) live in `tests/StringTemplates.IntegrationTests.Common/` so multiple integration projects can reuse them.
- New plugins should ship with a matching `tests/plugins/<Name>/StringTemplates.Plugins.<Name>.UnitTests/` project, registered in the slnx alongside the source.

## Wiki & external docs

The user-facing documentation lives in the GitHub wiki at <https://github.com/Stratis-OSS/StringTemplates.Net/wiki>. The wiki repository is wired into this repo as a git submodule at `./docs-reference/`, so the markdown sources are available locally. Pull the latest with `git submodule update --remote docs-reference`.