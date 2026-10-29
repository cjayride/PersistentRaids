# PersistentRaids — Valheim 1.0.21 release

## Goal
- Verify Release build against Valheim 1.0.21 (Steam build 26249040).
- BepInEx dependency `denikson-BepInExPack_Valheim-5.4.2360`.
- Changelog, Thunderstore `/dist` zip, git tag `v1.0.9`.

## Status
- Completed

## Changes
- Version 1.0.9: manifest, Plugin.cs, csproj, CHANGELOG (root + Package), README compatibility note.
- README refresh (Package + root) included in commit.

## Build
- `dotnet build Cjayride.PersistentRaids.csproj -c Release` — succeeded (0 warnings).
- Valheim install: `D:\SteamLibrary\steamapps\common\Valheim`, buildid **26249040**.
- DLL: `D:\AI-Agent-Projects\cjaycraft\Cjayride.PersistentRaids\bin\Release\net481\Cjayride.PersistentRaids.dll`
- Zip: `D:\AI-Agent-Projects\cjaycraft\Cjayride.PersistentRaids\dist\cjayride-PersistentRaids-1.0.9.zip`
