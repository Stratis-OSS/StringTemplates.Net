#!/usr/bin/env python3
"""PreToolUse hook: refuse Edit/Write that adds a Version attribute to a <PackageReference> in a .csproj.

Versions are managed centrally in Directory.Packages.props.
"""
import json
import re
import sys


def main() -> int:
    try:
        data = json.load(sys.stdin)
    except json.JSONDecodeError:
        return 0

    tool = data.get("tool_name", "")
    if tool not in ("Edit", "Write"):
        return 0

    tool_input = data.get("tool_input", {}) or {}
    file_path = tool_input.get("file_path", "") or ""
    if not file_path.endswith(".csproj"):
        return 0

    if tool == "Edit":
        candidate = tool_input.get("new_string", "") or ""
    else:
        candidate = tool_input.get("content", "") or ""

    if re.search(r"<PackageReference\b[^>]*\bVersion\s*=", candidate):
        sys.stderr.write(
            "Blocked: <PackageReference> must not carry a Version attribute. "
            "Pin the version in Directory.Packages.props instead "
            "(see AGENTS.md > Conventions > Centralized package management).\n"
        )
        return 2

    return 0


if __name__ == "__main__":
    sys.exit(main())
