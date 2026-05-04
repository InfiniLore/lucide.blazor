import argparse
import subprocess
import shutil
import json
from pathlib import Path


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
    return list(icon_dir.glob("*.svg"))


# ----------------------------------------------------------------------------------------------------------------------
# Razor generation (simplified)
# ----------------------------------------------------------------------------------------------------------------------

def generate_razor(icon_files: list[Path], output_dir: Path, namespace: str):
    output_dir.mkdir(parents=True, exist_ok=True)

    for icon in icon_files:
        name = icon.stem.replace("-", "_").title().replace("_", "")
        content = icon.read_text()

        component = f"""@namespace {namespace}

<svg>
{content}
</svg>
"""

        (output_dir / f"{name}.razor").write_text(component)

    print(f"Generated {len(icon_files)} Razor components")


# ----------------------------------------------------------------------------------------------------------------------
# Versioning
# ----------------------------------------------------------------------------------------------------------------------

def update_version(version_file: Path, new_version: str):
    data = {"version": new_version}
    version_file.write_text(json.dumps(data, indent=2))
    print(f"Updated version to {new_version}")


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