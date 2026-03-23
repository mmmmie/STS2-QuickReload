#!/bin/bash
set -euo pipefail
set -x


cd "$(dirname "$0")/.."

dotnet build -c Release QuickReload.csproj
pkill -SIGTERM -f 'Slay the Spire 2' || true
pkill -SIGKILL -f 'Slay the Spire 2' || true
