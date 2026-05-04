import argparse
import subprocess
import json
import re
from pathlib import Path
import xml.etree.ElementTree as ET
from typing import Literal

# ----------------------------------------------------------------------------------------------------------------------
# Constants / Types
# ----------------------------------------------------------------------------------------------------------------------

VERSION_PATTERN = re.compile(r"^\d+\.\d+\.\d+(-preview\.\d+)?$")
BumpPart = Literal["major", "minor", "patch", "preview"]


# ----------------------------------------------------------------------------------------------------------------------
# Utilities
# ----------------------------------------------------------------------------------------------------------------------

def run(cmd: list[str], cwd: Path | None = None):
    result = subprocess.run(cmd, cwd=cwd, capture_output=True, text=True)
    if result.returncode != 0:
        raise RuntimeError(f"Command failed: {' '.join(cmd)}\n{result.stderr}")
    return result.stdout.strip()


def validate_version(version: str) -> bool:
    return VERSION_PATTERN.match(version) is not None


# ----------------------------------------------------------------------------------------------------------------------
# Version logic
# ----------------------------------------------------------------------------------------------------------------------

def bump(version: str, part: BumpPart) -> str:
    core, preview = version, None

    if "-preview." in version:
        core, preview = version.split("-preview.")

    major, minor, patch = map(int, core.split("."))

    if part == "major":
        major, minor, patch = major + 1, 0, 0
        preview = "0" if preview else None
    elif part == "minor":
        minor, patch = minor + 1, 0
        preview = "0" if preview else None
    elif part == "patch":
        patch += 1
        preview = "0" if preview else None
    elif part == "preview":
        preview = "1" if preview is None else str(int(preview) + 1)
    else:
        raise ValueError(part)

    result = f"{major}.{minor}.{patch}"
    if preview is not None:
        result += f"-preview.{preview}"

    return result


# ----------------------------------------------------------------------------------------------------------------------
# XML helpers (.csproj / Directory.Build.props)
# ----------------------------------------------------------------------------------------------------------------------

def update_xml_version(file: Path, new_version: str):
    tree = ET.parse(file)
    root = tree.getroot()

    version_elem = root.find(".//Version")

    if version_elem is None:
        raise RuntimeError(f"<Version> not found in {file}")

    version_elem.text = new_version
    tree.write(file, encoding="utf-8", xml_declaration=True)

    print(f"Updated XML version in {file}")


# ----------------------------------------------------------------------------------------------------------------------
# CMake
# ----------------------------------------------------------------------------------------------------------------------

def update_cmake_version(file: Path, new_version: str):
    text = file.read_text(encoding="utf-8")

    updated, count = re.subn(
        r"(?m)^(\s*project\(\s*InfiniFrame\.Native\s+VERSION\s+)\S+",
        rf"\g<1>{new_version}",
        text,
        count=1,
    )

    if count == 0:
        raise RuntimeError(f"Failed to update CMake version in {file}")

    file.write_text(updated, encoding="utf-8")

    print(f"Updated CMake version in {file}")


# ----------------------------------------------------------------------------------------------------------------------
# package.json
# ----------------------------------------------------------------------------------------------------------------------

def update_package_json(file: Path, new_version: str):
    data = json.loads(file.read_text(encoding="utf-8"))

    if "version" in data:
        data["version"] = new_version

    # update scripts too
    for key, value in data.get("scripts", {}).items():
        if isinstance(value, str):
            data["scripts"][key] = re.sub(
                r"\d+\.\d+\.\d+(?:-preview\.\d+)?",
                new_version,
                value,
            )

    file.write_text(json.dumps(data, indent=2) + "\n", encoding="utf-8")

    print(f"Updated package.json: {file}")


# ----------------------------------------------------------------------------------------------------------------------
# Discovery helpers
# ----------------------------------------------------------------------------------------------------------------------

def find_files(root: Path, pattern: str, exclude: list[str] | None = None):
    exclude = exclude or []

    for path in root.rglob(pattern):
        if any(ex in path.parts for ex in exclude):
            continue
        yield path


# ----------------------------------------------------------------------------------------------------------------------
# Git
# ----------------------------------------------------------------------------------------------------------------------

def commit_and_push(repo_dir: Path, message: str, push: bool):
    run(["git", "add", "."], cwd=repo_dir)
    run(["git", "commit", "-m", message], cwd=repo_dir)

    if push:
        run(["git", "push"], cwd=repo_dir)


# ----------------------------------------------------------------------------------------------------------------------
# Pipeline
# ----------------------------------------------------------------------------------------------------------------------

def update_versions(
    repo_root: Path,
    part: str | None,
    custom_version: str | None,
    directory_props: Path | None,
    csproj_files: list[Path],
    update_all_csproj: bool,
    cmake_file: Path | None,
    update_package_json_files_flag: bool,
    commit: bool,
    push: bool,
):
    # ------------------------------------------------------------------------------------------------------------------
    # Determine base version
    # ------------------------------------------------------------------------------------------------------------------
    base_file = directory_props or (csproj_files[0] if csproj_files else None)

    if not base_file:
        raise RuntimeError("No base version source provided")

    tree = ET.parse(base_file)
    root = tree.getroot()
    version_elem = root.find(".//Version")

    if version_elem is None or not version_elem.text:
        raise RuntimeError(f"No version found in {base_file}")

    old_version = version_elem.text.strip()

    if custom_version:
        if not validate_version(custom_version):
            raise RuntimeError(f"Invalid version: {custom_version}")
        new_version = custom_version
    else:
        new_version = bump(old_version, part)  # type: ignore

    # ------------------------------------------------------------------------------------------------------------------
    # Apply updates
    # ------------------------------------------------------------------------------------------------------------------

    if directory_props:
        update_xml_version(directory_props, new_version)

    if update_all_csproj:
        csproj_files = list(find_files(repo_root, "*.csproj"))

    for csproj in csproj_files:
        update_xml_version(csproj, new_version)

    if cmake_file:
        update_cmake_version(cmake_file, new_version)

    if update_package_json_files_flag:
        for pkg in find_files(repo_root, "package.json", exclude=["node_modules"]):
            update_package_json(pkg, new_version)

    print(f"Bumped version: {old_version} -> {new_version}")

    if commit:
        commit_and_push(
            repo_root,
            f"Bump version to {new_version}",
            push,
        )

    return new_version


# ----------------------------------------------------------------------------------------------------------------------
# CLI
# ----------------------------------------------------------------------------------------------------------------------

def main():
    parser = argparse.ArgumentParser()

    parser.add_argument("--repo-root", default=".")
    parser.add_argument("--part", choices=["major", "minor", "patch", "preview"])
    parser.add_argument("--custom-version")

    parser.add_argument("--directory-props")
    parser.add_argument("--csproj", action="append", help="Specific csproj files")
    parser.add_argument("--all-csproj", action="store_true")

    parser.add_argument("--cmake-file")
    parser.add_argument("--update-package-json", action="store_true")

    parser.add_argument("--commit", action="store_true")
    parser.add_argument("--push", action="store_true")

    args = parser.parse_args()

    update_versions(
        repo_root=Path(args.repo_root),
        part=args.part,
        custom_version=args.custom_version,
        directory_props=Path(args.directory_props) if args.directory_props else None,
        csproj_files=[Path(p) for p in args.csproj or []],
        update_all_csproj=args.all_csproj,
        cmake_file=Path(args.cmake_file) if args.cmake_file else None,
        update_package_json_files_flag=args.update_package_json,
        commit=args.commit,
        push=args.push,
    )


if __name__ == "__main__":
    main()