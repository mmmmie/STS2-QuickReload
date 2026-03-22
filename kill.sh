#!/bin/bash
set -euo pipefail
set -x

dotnet build -c Release MieMod.csproj
pkill -SIGTERM -f 'Slay the Spire 2' || true
pkill -SIGKILL -f 'Slay the Spire 2' || true

