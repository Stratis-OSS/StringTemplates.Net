---
name: Skill Creation
description: Use when creating a new agent skill for the StringTemplates project. TRIGGER when the user says "create a skill", "new skill", "scaffold a skill", or "add a skill". SKIP for edits to the body of an existing skill — only use this for shape, frontmatter, and scaffolding.
usage: This skill is the source of truth for how skills are authored in this repository. Skills here are designed to ship as part of a Claude plugin (`.claude-plugin`) so that users can install them. Every skill is a single `SKILL.md` file under `./skills/<skill-slug>/` at the repo root — no references, no extra assets, no nested folders.
---

# Skill Creation

This skill describes how to author a new agent skill in this repository.

## Where skills live

- All skills live under `./skills/<skill-slug>/SKILL.md` at the repository root. This is the only canonical location.
- `.agents/skills` and `.claude/skills` are symlinks that point at `./skills/` for local development. Never write skill files into those paths directly — author them under `./skills/` and the symlinks expose them to each agent surface automatically.
- The slug is the folder name. Use kebab-case (e.g. `skill-creation`, `mailkit-templates`).
- There is exactly one file per skill: `SKILL.md`. No `references/` folder, no assets, no nested subfolders. If a topic feels too big for one file, split it into two separate skills instead.

## File contract

A skill is a single `SKILL.md` with YAML frontmatter and a Markdown body.

```yaml
---
name: <Title Case Name>
description: <When to use this skill — trigger-oriented, one paragraph.>
usage: <Prose summary aimed at the reader once the skill is open.>
---
```

- `name` — human-readable title, Title Case (e.g. `Skill Creation`).
- `description` — the dispatcher reads this to decide whether to load the skill. Be specific and concrete. Include positive triggers ("Use when …", "TRIGGER when …") and a negative boundary ("SKIP for …").
- `usage` — one short paragraph telling the reader what the skill covers once they open it.

### YAML gotcha: no unquoted `:` in inline values

YAML rejects an unquoted value on the same line as its key if the value contains a `:`. Use an em-dash, comma, or quotes instead.

```text
# BAD
description: Use when scaffolding: commands, queries, DTOs.

# OK
description: "Use when scaffolding: commands, queries, DTOs."

# OK
description: Use when scaffolding — commands, queries, DTOs.
```

## Body style

Keep the body short and focused. A skill about one feature describes only that feature.

- One topic per skill. If the body grows past a few screens, the skill is doing too much — split it.
- Lead with the rule or workflow, not preamble.
- Show concrete examples in fenced code blocks when they clarify; skip them when prose is enough.
- No "Related skills" sections, no cross-links, no pointers to other files. Each skill stands alone.
- End the file with a single trailing newline. UTF-8, no BOM.

## How to create a new skill

1. Pick a kebab-case slug that names the feature (`mailkit-templates`, `configuration-binding`, etc.).
2. Create `./skills/<slug>/SKILL.md` at the repository root.
3. Write the frontmatter: `name`, `description` with explicit triggers, and a short `usage` paragraph.
4. Write the body — straight to the point, single feature, no references.
5. Verify the frontmatter parses (no stray `:` in unquoted values) and the file ends in a newline.

## How to remove or rename a skill

- To rename: rename the folder under `./skills/`. Update the `name` field in the frontmatter if the human-readable title changed.
- To remove: delete the `./skills/<slug>/` folder.

## What does NOT belong in a skill

- References to other skills or files outside `./skills/<slug>/`.
- Repository-wide conventions that apply to the source code — those belong in `AGENTS.md`.
- Build, test, or release instructions — those belong in `AGENTS.md` or `README.md`.
- Planning notes, changelogs, or task lists.
