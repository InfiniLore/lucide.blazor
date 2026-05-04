import argparse
import subprocess
import shutil
import json
from pathlib import Path
import re
import xml.etree.ElementTree as ET
from copy import deepcopy


# ----------------------------------------------------------------------------------------------------------------------
# Utilities
# ----------------------------------------------------------------------------------------------------------------------

def run(cmd: list[str], cwd: Path | None = None):
    result = subprocess.run(cmd, cwd=cwd, capture_output=True, text=True)
    if result.returncode != 0:
        raise RuntimeError(f"Command failed: {' '.join(cmd)}\n{result.stderr}")
    return result.stdout.strip()


# ----------------------------------------------------------------------------------------------------------------------
# Git operations
# ----------------------------------------------------------------------------------------------------------------------

def clone_or_update_repo(repo_url: str, target_dir: Path, branch: str):
    if target_dir.exists():
        print(f"Updating repo in {target_dir}")
        run(["git", "fetch"], cwd=target_dir)
        run(["git", "checkout", branch], cwd=target_dir)
        run(["git", "pull"], cwd=target_dir)
    else:
        print(f"Cloning repo {repo_url}")
        run(["git", "clone", "--branch", branch, repo_url, str(target_dir)])


# ----------------------------------------------------------------------------------------------------------------------
# Icon processing
# ----------------------------------------------------------------------------------------------------------------------

def copy_icons(src: Path, dst: Path):
    if dst.exists():
        shutil.rmtree(dst)
    shutil.copytree(src, dst)
    print(f"Copied icons from {src} to {dst}")


def load_icon_files(icon_dir: Path):
    return sorted(icon_dir.glob("*.svg"), key=lambda p: p.stem)


# ----------------------------------------------------------------------------------------------------------------------
# Razor generation
# ----------------------------------------------------------------------------------------------------------------------

LUCIDE_RAZOR_LICENSE_HEADER = """@* --- *@
@* ISC License *@
@* Copyright (c) for portions of Lucide are held by Cole Bemis 2013-2022 as part of Feather (MIT). All other copyright (c) for Lucide are held by Lucid Contributors 2022. *@
@*  *@
@* Permission to use, copy, modify, and/or distribute this software for any *@
@* purpose with or without fee is hereby granted, provided that the above *@
@* copyright notice and this permission notice appear in all copies. *@
@*  *@
@* THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES *@
@* WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF *@
@* MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR *@
@* ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES *@
@* WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN *@
@* ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF *@
@* OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE. *@
@*  *@
@* --- *@
@* ReSharper disable once CheckNamespace *@"""


def to_pascal_case(name: str) -> str:
    parts = [part for part in re.split(r"[^a-zA-Z0-9]+", name) if part]
    return "".join(part[0].upper() + part[1:] for part in parts)


def extract_svg_children(svg_file: Path) -> str:
    xml_content = svg_file.read_text(encoding="utf-8")
    try:
        root = ET.fromstring(xml_content)
    except ET.ParseError as exc:
        raise RuntimeError(f"Failed to parse SVG file: {svg_file}") from exc

    def strip_namespaces(node: ET.Element):
        if node.tag.startswith("{"):
            node.tag = node.tag.split("}", 1)[1]
        namespaced_attrs = [key for key in node.attrib if key.startswith("{")]
        for key in namespaced_attrs:
            local_key = key.split("}", 1)[1]
            node.attrib[local_key] = node.attrib.pop(key)
        for sub in node:
            strip_namespaces(sub)

    child_nodes = []
    for child in root:
        child_copy = deepcopy(child)
        strip_namespaces(child_copy)
        rendered = ET.tostring(child_copy, encoding="unicode", short_empty_elements=True).strip()
        if rendered:
            child_nodes.append(f"    {rendered}")
    return "\n".join(child_nodes)


def clear_existing_razor_files(output_dir: Path):
    if not output_dir.exists():
        return
    for file in output_dir.glob("*.razor"):
        file.unlink()


def generate_razor(icon_files: list[Path], output_dir: Path, namespace: str):
    output_dir.mkdir(parents=True, exist_ok=True)
    clear_existing_razor_files(output_dir)

    for icon in icon_files:
        name = f"Li{to_pascal_case(icon.stem)}"
        inner_svg = extract_svg_children(icon)

        component = f"""{LUCIDE_RAZOR_LICENSE_HEADER}
@namespace {namespace}
@inherits LucideComponentBase
<svg class="@Class"
     xmlns="http://www.w3.org/2000/svg"
     width="@Size"
     height="@Size"
     viewBox="0 0 24 24"
     fill="@Fill"
     stroke="@Stroke"
     stroke-width="@StrokeWidth"
     stroke-linecap="@StrokeLineCap"
     stroke-linejoin="@StrokeLineJoin">
{inner_svg}
</svg>
"""

        (output_dir / f"{name}.razor").write_text(component, encoding="utf-8")

    print(f"Generated {len(icon_files)} Razor components")


# ----------------------------------------------------------------------------------------------------------------------
# Versioning
# ----------------------------------------------------------------------------------------------------------------------

def update_version(version_file: Path, new_version: str):
    if version_file.suffix.lower() != ".json":
        raise RuntimeError(f"Version file must be JSON: {version_file}")

    existing = {}
    if version_file.exists():
        content = version_file.read_text(encoding="utf-8").strip()
        if content:
            existing = json.loads(content)

    if not isinstance(existing, dict):
        raise RuntimeError(f"Expected root JSON object in version file: {version_file}")

    dependencies = existing.get("dependencies")
    updated = False

    if "version" in existing:
        existing["version"] = new_version
        updated = True

    if isinstance(dependencies, dict) and "lucide-static" in dependencies:
        dependencies["lucide-static"] = new_version
        updated = True

    # Fallback for plain {"version": "..."} style files.
    if not updated:
        existing["version"] = new_version

    version_file.write_text(json.dumps(existing, indent=2) + "\n", encoding="utf-8")
    print(f"Updated version fields in {version_file} to {new_version}")


# ----------------------------------------------------------------------------------------------------------------------
# Git commit/push
# ----------------------------------------------------------------------------------------------------------------------

def commit_and_push(repo_dir: Path, message: str, push: bool):
    run(["git", "add", "."], cwd=repo_dir)
    run(["git", "commit", "-m", message], cwd=repo_dir)

    if push:
        run(["git", "push"], cwd=repo_dir)


# ----------------------------------------------------------------------------------------------------------------------
# Main pipeline
# ----------------------------------------------------------------------------------------------------------------------

def update_lucide_static(
    lucide_repo: str,
    lucide_branch: str,
    lucide_local_path: Path,
    icon_source_subdir: str,
    icon_output_dir: Path,
    razor_output_dir: Path,
    namespace: str,
    version_file: Path | None,
    version: str | None,
    commit: bool,
    push: bool,
):
    clone_or_update_repo(lucide_repo, lucide_local_path, lucide_branch)

    icon_source = lucide_local_path / icon_source_subdir
    copy_icons(icon_source, icon_output_dir)

    icons = load_icon_files(icon_output_dir)
    generate_razor(icons, razor_output_dir, namespace)

    if version_file and version:
        update_version(version_file, version)

    if commit:
        commit_and_push(
            repo_dir=Path.cwd(),
            message=f"Update Lucide icons to {version or 'latest'}",
            push=push,
        )


# ----------------------------------------------------------------------------------------------------------------------
# CLI
# ----------------------------------------------------------------------------------------------------------------------

def main():
    parser = argparse.ArgumentParser()

    parser.add_argument("--lucide-repo", required=True)
    parser.add_argument("--lucide-branch", default="main")
    parser.add_argument("--lucide-local-path", required=True)

    parser.add_argument("--icon-source-subdir", default="icons")
    parser.add_argument("--icon-output-dir", required=True)

    parser.add_argument("--razor-output-dir", required=True)
    parser.add_argument("--namespace", required=True)

    parser.add_argument("--version-file")
    parser.add_argument("--version")

    parser.add_argument("--commit", action="store_true")
    parser.add_argument("--push", action="store_true")

    args = parser.parse_args()

    update_lucide_static(
        lucide_repo=args.lucide_repo,
        lucide_branch=args.lucide_branch,
        lucide_local_path=Path(args.lucide_local_path),
        icon_source_subdir=args.icon_source_subdir,
        icon_output_dir=Path(args.icon_output_dir),
        razor_output_dir=Path(args.razor_output_dir),
        namespace=args.namespace,
        version_file=Path(args.version_file) if args.version_file else None,
        version=args.version,
        commit=args.commit,
        push=args.push,
    )


if __name__ == "__main__":
    main()
