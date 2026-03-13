#!/usr/bin/env python3
from bump_version import bump, validate_version

def run() -> int:
    failures = 0

    def check(actual, expected, label):
        nonlocal failures
        if actual != expected:
            failures += 1
            print(f"FAIL: {label}\n  expected: {expected}\n  actual:   {actual}")
        else:
            print(f"PASS: {label}")

    # validate_version tests
    check(validate_version("1.2.3"), True, "validate stable")
    check(validate_version("1.2.3-preview.1"), True, "validate preview")
    check(validate_version("1.2"), False, "reject short version")
    check(validate_version("1.2.3-preview"), False, "reject missing preview number")
    check(validate_version("v1.2.3"), False, "reject prefixed version")

    # bump tests for stable versions
    check(bump("1.2.3", "patch"), "1.2.4", "stable patch bump")
    check(bump("1.2.3", "minor"), "1.3.0", "stable minor bump")
    check(bump("1.2.3", "major"), "2.0.0", "stable major bump")
    check(bump("1.2.3", "preview"), "1.2.3-preview.1", "stable preview bump")

    # bump tests for preview versions
    check(bump("1.2.3-preview.5", "patch"), "1.2.4-preview.0", "preview patch bump")
    check(bump("1.2.3-preview.5", "minor"), "1.3.0-preview.0", "preview minor bump")
    check(bump("1.2.3-preview.5", "major"), "2.0.0-preview.0", "preview major bump")
    check(bump("1.2.3-preview.5", "preview"), "1.2.3-preview.6", "preview increment bump")

    # unknown part should raise
    try:
        bump("1.2.3", "banana")
        failures += 1
        print("FAIL: unknown bump part should raise ValueError")
    except ValueError:
        print("PASS: unknown bump part raises ValueError")

    if failures:
        print(f"\n{failures} test(s) failed.")
        return 1

    print("\nAll tests passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(run())

