#!/usr/bin/env python3
"""PreToolUse hook: refuse `git add` / `git commit` that touches local-only files.

Catches obvious mistakes like committing .DS_Store, *.user, *.local.json — files that
should never leave a developer's machine.
"""
import json
import re
import sys

BANNED_PATTERNS = [
    r"\.DS_Store\b",
    r"\.user\b",
    r"\.local\.json\b",
    r"settings\.local\.json\b",
]


def main() -> int:
    try:
        data = json.load(sys.stdin)
    except json.JSONDecodeError:
        return 0

    if data.get("tool_name", "") != "Bash":
        return 0

    cmd = (data.get("tool_input", {}) or {}).get("command", "") or ""
    if not re.search(r"\bgit\s+(add|commit)\b", cmd):
        return 0

    # Allow `git commit -m "..."` with no path args — the staged set is the user's concern.
    # Only block when a banned pattern appears literally in the command.
    for pat in BANNED_PATTERNS:
        if re.search(pat, cmd):
            sys.stderr.write(
                f"Blocked: refusing to git add/commit a local-only path (matched /{pat}/). "
                "Add it to .gitignore or stage explicitly without this hook.\n"
            )
            return 2

    return 0


if __name__ == "__main__":
    sys.exit(main())
