# QuickReload

QuickReload is a Slay the Spire 2 mod that adds fast run reload from the pause menu, including multiplayer support.

During a run, open the pause menu (`Esc`) and press the `Reload` button. This reloads the run from the last autosave.

## Install

QuickReload should be installed as one folder named `QuickReload` inside the game's `mods` directory.

Required files in `QuickReload/`:
- `QuickReload.dll`
- `QuickReload.pck`
- `QuickReload.json`

### macOS

1. Open your Steam library path:
   `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods/`
2. Create `mods/` folder if missing
3. Copy the unzipped `QuickReload/` into that folder.

### Windows

1. Open your Steam library path:
   `Steam\steamapps\common\Slay the Spire 2\mods\`
2. Create `mods\` folder if missing
3. Copy the unzipped `QuickReload\` into that folder.


## Build

```bash
dotnet build -c Release QuickReload.csproj
```

There are helper scripts in `scripts/` for build and local multiplayer debug workflows.
