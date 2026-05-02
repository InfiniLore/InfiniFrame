#!/usr/bin/env bash
set -euo pipefail

repo_root="$(git rev-parse --show-toplevel)"
native_root="${repo_root}/src/InfiniFrame.Native"
quality_root="${native_root}/build/native-quality"

collect_format_sources() {
    git -C "${repo_root}" ls-files --cached --others --exclude-standard -- \
        'src/InfiniFrame.Native/*.cpp' \
        'src/InfiniFrame.Native/*.h' \
        'src/InfiniFrame.Native/*.mm' \
        'src/InfiniFrame.Native/**/*.cpp' \
        'src/InfiniFrame.Native/**/*.h' \
        'src/InfiniFrame.Native/**/*.mm' |
        grep -v '^src/InfiniFrame.Native/Dependencies/' |
        sed "s#^#${repo_root}/#"
}

run_format_check() {
    mapfile -t sources < <(collect_format_sources)

    if [[ ${#sources[@]} -eq 0 ]]; then
        echo "No native sources found for clang-format."
        return 0
    fi

    clang-format --dry-run --Werror "${sources[@]}"
}

configure_tidy_build() {
    cmake -S "${native_root}" \
        -B "${quality_root}/tidy" \
        -DCMAKE_BUILD_TYPE=Debug \
        -DCMAKE_EXPORT_COMPILE_COMMANDS=ON \
        -DINFINIFRAME_BUILD_TEST_EXPORTS=ON
}

collect_tidy_sources() {
    local compile_commands="${quality_root}/tidy/compile_commands.json"

    python3 - "${compile_commands}" "${repo_root}" <<'PY'
import json
import pathlib
import sys

compile_commands = pathlib.Path(sys.argv[1])
repo_root = pathlib.Path(sys.argv[2]).resolve()
native_root = repo_root / "src" / "InfiniFrame.Native"
dependencies_root = native_root / "Dependencies"

entries = json.loads(compile_commands.read_text(encoding="utf-8"))
seen = set()

for entry in entries:
    source = pathlib.Path(entry["file"]).resolve()

    if dependencies_root in source.parents:
        continue

    if native_root not in (source, *source.parents):
        continue

    if source.suffix not in {".cpp", ".mm"}:
        continue

    if source in seen:
        continue

    seen.add(source)
    print(source)
PY
}

run_clang_tidy() {
    configure_tidy_build
    mapfile -t sources < <(collect_tidy_sources)

    if [[ ${#sources[@]} -eq 0 ]]; then
        echo "No native compile database sources found for clang-tidy."
        return 0
    fi

    clang-tidy \
        -p "${quality_root}/tidy" \
        --checks='-*,clang-analyzer-*' \
        --warnings-as-errors='clang-analyzer-*' \
        "${sources[@]}"
}

run_sanitizer_build() {
    cmake -S "${native_root}" \
        -B "${quality_root}/sanitizer" \
        -DCMAKE_BUILD_TYPE=Debug \
        -DINFINIFRAME_BUILD_TEST_EXPORTS=ON

    cmake --build "${quality_root}/sanitizer" --parallel
}

case "${1:-all}" in
    format)
        run_format_check
        ;;
    clang-tidy)
        run_clang_tidy
        ;;
    sanitizer-build)
        run_sanitizer_build
        ;;
    all)
        run_format_check
        run_clang_tidy
        run_sanitizer_build
        ;;
    *)
        echo "Usage: $0 [format|clang-tidy|sanitizer-build|all]" >&2
        exit 2
        ;;
esac
