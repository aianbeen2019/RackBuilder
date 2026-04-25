# RackBuilder

**A rack management UI mod for Data Center**

> WIP — pre-release (v1.1.1)

---

## Features

- **Data Center floor plan** — visual grid of every rack position; grey = empty, green = occupied
- **Per-rack detail view** — click any rack to see installed equipment with accurate U positions (U01–U47)
- **Server identification** — recognises System X, RISC, GPU, and Mainframe variants; filters transceiver sockets
- **Remove items** — single-click removal with confirmation
- **Auto-Best Config** — fills an empty rack with an optimal server/switch/patch panel layout via technicians
- **Auto-Wire** — automatically cables rack servers to available switch ports
- **Auto-Fill SFP/QSFP** — fills all empty transceiver slots on rack switches with the best compatible modules

---

## Installation

1. Install [MelonLoader v0.6+](https://melonwiki.xyz/#/?id=requirements) for Data Center
2. Download `RackBuilderMod.dll` from [Releases](../../releases)
3. Drop it into your `Data Center/Mods/` folder
4. Launch the game — press **R** in-game or use the **Rack Builder** button in the laptop's Computer Shop

---

## Building from Source

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- MelonLoader installed and the game run at least once (generates the IL2Cpp assemblies)

### Required DLLs (not included — redistributing game files is not permitted)

Copy the following from your game install into the locations your `.csproj` references.

**From** `Data Center/MelonLoader/net6/`:
- `MelonLoader.dll`
- `Il2CppInterop.Runtime.dll`

**From** `Data Center/MelonLoader/Il2CppAssemblies/`:
- `Assembly-CSharp.dll`
- `Assembly-CSharp-firstpass.dll`
- `Il2Cpp__Generated.dll`
- `Il2CppSystem.dll`
- `Il2CppSystem.Core.dll`
- `Il2CppNewtonsoft.Json.dll`
- *(and any others referenced in `RackBuilderMod_v3.csproj`)*

> The `.csproj` uses absolute paths pointing to `D:\SteamLibrary\steamapps\common\Data Center\...`. Update the `<HintPath>` values to match your own install path.

### Build

```bash
dotnet build /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary
```

Output: `bin/Debug/net6.0/RackBuilderMod.dll`

---

## Planned Features

- **SFP/QSFP module type selector** — choose module type before Auto-Fill, with per-switch control
- **Optimised Auto-Wire to Customer** — wire a specific rack to a chosen customer base
- **Rack utilisation summary** — show U usage per rack on the floor plan (e.g. `32/47U used`)
- **Bulk remove** — clear an entire rack with one confirmation
- **Rack profiles / templates** — save a rack layout as a named profile; one-click apply to any empty rack

---

## AI Usage

GitHub Copilot (Claude Sonnet 4.6) used for UI scaffolding, IL2CPP interop patterns, and scan logic. All code manually reviewed, tested in-game, and iterated on by the mod author.

---

## License

MIT — do what you like, credit appreciated.
