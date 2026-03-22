#!/bin/bash
set -euo pipefail
set -x

dotnet build -c Release MieMod.csproj
open -n ~/Library/Application\ Support/Steam/steamapps/common/Slay\ the\ Spire\ 2/SlayTheSpire2.app --args --fastmp=join --clientId=1001
# open -n ~/Library/Application\ Support/Steam/steamapps/common/Slay\ the\ Spire\ 2/SlayTheSpire2.app --args --remote-debug tcp://127.0.0.1:6007 --fastmp=join --clientId=1001

