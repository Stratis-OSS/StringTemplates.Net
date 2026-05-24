# StringTemplates.Net — Copilot instructions

The canonical guide is [`AGENTS.md`](../AGENTS.md) — repository layout, build/test/run commands, conventions, and the project-local skills under `./skills/`. Read it first.

## Hard rules (enforced by the build, a hook, or review)

- `<PackageReference>` must NEVER carry a `Version` attribute. All NuGet versions are pinned centrally in `Directory.Packages.props` (`NU1008` if violated). A pre-tool hook also refuses any edit that would add one.
- Code style is build-enforced (`EnforceCodeStyleInBuild` is on). `.editorconfig` violations fail the build.
- Local-only files (`.DS_Store`, `*.user`, `*.local.json`, `settings.local.json`) must never be committed. A pre-tool hook refuses `git add` / `git commit` that touches them.
- Tests use **xUnit**. Project naming: `<Project>.UnitTests` for unit tests, `<Project>.IntegrationTests` for integration tests.
- Files must end with a single trailing newline.

## Project naming

- Core library: `StringTemplates`.
- Plugin folder: `StringTemplates.Plugins.<Name>`. The shipped `<PackageId>` and `<Title>` drop the `Plugins.` segment (folder `StringTemplates.Plugins.<Name>` → package `StringTemplates.<Name>`).
- Plugin tests mirror the source layout under `tests/plugins/<Name>/`.

## One plugin, one concern

Each official plugin ships as a separate package and adds one placeholder source. No kitchen-sink plugins.

## When in doubt

Open the skill that matches the task — skills live under `./skills/<slug>/SKILL.md` as a single self-contained file:

- `./skills/solution/SKILL.md` — solution-wide concerns (`Directory.Build.props`, `Directory.Packages.props`, central package management, package metadata, the slnx, local pack scripts).
- `./skills/skill-creation/SKILL.md` — how to author a new skill in this repo.
