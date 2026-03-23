#!/bin/bash
set -euo pipefail
set -x

project_dir="$(cd "$(dirname "$0")/.." && pwd)"
mod_name="QuickReload"
mods_dir="$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods/$mod_name"
zip_path="$project_dir/${mod_name}.zip"
temp_root="$project_dir/.release_tmp"
temp_dir="$temp_root/$mod_name"

dotnet build -c Release QuickReload.csproj

rm -rf "$temp_root"
mkdir -p "$temp_dir"

cp "$mods_dir/$mod_name.dll" "$temp_dir/"
cp "$mods_dir/$mod_name.pck" "$temp_dir/"
cp "$mods_dir/$mod_name.json" "$temp_dir/"

rm -f "$zip_path"
(cd "$temp_root" && zip -r "$zip_path" "$mod_name")
rm -rf "$temp_root"
