#!/usr/bin/env python3
"""Full statusline: model | branch* | rel-cwd | $cost | +added/-removed | context warning."""
import json
import os
import subprocess
import sys


def git(cwd: str, *args: str) -> str:
    try:
        return subprocess.check_output(
            ["git", "-C", cwd, *args], stderr=subprocess.DEVNULL, text=True
        ).strip()
    except (subprocess.CalledProcessError, FileNotFoundError):
        return ""


def main() -> int:
    try:
        data = json.load(sys.stdin)
    except json.JSONDecodeError:
        data = {}

    model_info = data.get("model", {}) or {}
    model = model_info.get("display_name") or model_info.get("id") or "claude"

    workspace = data.get("workspace", {}) or {}
    cwd = workspace.get("current_dir") or data.get("cwd") or os.getcwd()
    project_dir = workspace.get("project_dir") or cwd

    branch = git(cwd, "rev-parse", "--abbrev-ref", "HEAD")
    if branch:
        dirty = bool(git(cwd, "status", "--porcelain"))
        branch = f"⎇ {branch}{'*' if dirty else ''}"

    rel = os.path.relpath(cwd, project_dir) if project_dir else cwd
    short_cwd = "." if rel in ("", ".") else rel

    cost = data.get("cost") or {}
    usd = cost.get("total_cost_usd")
    added = cost.get("total_lines_added") or 0
    removed = cost.get("total_lines_removed") or 0

    parts = [model]
    if branch:
        parts.append(branch)
    parts.append(short_cwd)
    if isinstance(usd, (int, float)):
        parts.append(f"${usd:.2f}")
    if added or removed:
        parts.append(f"+{added}/-{removed}")
    if data.get("exceeds_200k_tokens"):
        parts.append("⚠ 200k+")

    print(" │ ".join(parts))
    return 0


if __name__ == "__main__":
    sys.exit(main())
