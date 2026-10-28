#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
destination="${1:-$root/.fitnesse}"
requested="${2:-}"
base="https://repo1.maven.org/maven2/org/fitnesse/fitnesse"

if [ -n "$requested" ]; then
  version="$requested"
else
  version="$(curl -fsSL "$base/maven-metadata.xml" | sed -n 's:.*<release>\(.*\)</release>.*:\1:p' | head -n 1)"
fi
if [ -z "$version" ]; then
  echo "Could not determine the FitNesse version." >&2
  exit 1
fi

mkdir -p "$destination"
jar="$destination/fitnesse-$version-standalone.jar"
if [ -f "$jar" ]; then
  echo "$jar"
  exit 0
fi

archive="$base/$version/fitnesse-$version-standalone.jar"
partial="$jar.part"
rm -f "$partial"
if ! curl -fsSL -o "$partial" "$archive"; then
  rm -f "$partial"
  exit 1
fi

expected="$(curl -fsSL "$archive.sha256" | awk '{print $1}')"
if command -v sha256sum >/dev/null 2>&1; then
  actual="$(sha256sum "$partial" | awk '{print $1}')"
else
  actual="$(shasum -a 256 "$partial" | awk '{print $1}')"
fi
if [ "$expected" != "$actual" ]; then
  echo "Checksum mismatch for $archive" >&2
  rm -f "$partial"
  exit 1
fi

mv "$partial" "$jar"
echo "$jar"
