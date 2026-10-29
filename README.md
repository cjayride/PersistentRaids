# ⚔️ PersistentRaids

Make Valheim raids more persistent, aggressive, and dangerous.

## 1.0.9

- Verified against Valheim 1.0.21 (Steam build 26249040). BepInEx dependency: `denikson-BepInExPack_Valheim-5.4.2360`.

**PersistentRaids** is a BepInEx mod that keeps raid and event creatures hunting players with extended sight and persistent behavior. Normal open-world creatures retain vanilla behavior, so wildlife won't permanently accumulate around your bases.

Custom Raids remains responsible for deciding when raids begin. PersistentRaids handles the creatures after they spawn.

## ✨ Features

- **Persistent Raid Creatures** — Raid and event monsters won't despawn simply because players move away.
- **Extended Detection Range** — Raid creatures can spot and pursue players from farther away.
- **Aggressive AI** — Raid creatures are kept in hunt and alert states instead of fleeing or wandering aimlessly.
- **Configurable Monster Limits** — Control the number of active raid creatures server-wide, around players, or both.
- **Stuck Raid Protection** — Automatically ends random events that remain active after players leave the event area for too long.
- **Vanilla Wildlife Behavior** — Normal world spawns aren't affected by raid persistence or extended detection.
- **Server-Side Cleanup Commands** — Manage raid creatures and recover from stuck events.

## ⚠️ Important Compatibility Settings

**Do not enable Expand World Events' `Multiple events` feature.**

Keep the following setting disabled:

```ini
AllowMultipleRaids = false
```

Custom Raids controls when raids start. PersistentRaids preserves raid creatures and helps manage their behavior; it does not need to take over raid scheduling.

## 📦 Installation

1. Install **BepInEx** for Valheim.
2. Install PersistentRaids on the **dedicated server and every client**.
3. Start the game or server to generate the configuration file.
4. Review the configuration before adjusting monster limits or raid timeout settings.

Configuration file:

`BepInEx/config/cjayride.PersistentRaids.cfg`

## ⚙️ Monster Limits & Performance

Persistent creatures are written to the world save using `ZDO.Persistent`. This prevents normal zone unloading from removing them, but it also means creatures can accumulate if left unmanaged.

**Keep the monster cap enabled** to help control performance and prevent excessive creature buildup.

| Setting | Default | Description |
|---|---:|---|
| `MaxActiveMonsters` | `60` | Maximum active raid creatures for the default server-wide cap scope. |
| `CapScope` | `ServerWide` | Choose `ServerWide`, `PerPlayerArea`, or `Both`. |
| `MaxPerPlayerArea` | `40` | Maximum raid creatures per player area. |
| `PlayerAreaRadius` | `200` | Radius around players, in meters, for the per-player-area cap. |
| Cap check interval | `2 minutes` | How often the monster cap is enforced. |
| `OnlyApplyWhilePlayersOnline` | — | Disables persistence while the server is empty, allowing zones to unload. |

When a cap is exceeded, the mod prioritizes removing excess raid creatures furthest from players. Tames, ordinary wildlife, normal spawns, and bosses are excluded from the raid-creature cap.

## ⏱️ Stuck Raid Protection

Custom Raids can pause an event when nobody is inside its event area. If everyone sails away, the event timer may freeze and prevent another raid from starting.

PersistentRaids includes **RaidTimeout**, enabled by default, to recover from this situation.

| Setting | Default | Description |
|---|---:|---|
| `EndEventAfterNoPlayersInAreaMinutes` | `25` | End the random event after this many real minutes without players in its area. |
| `MaxRaidWallClockMinutes` | `90` | End the random event after this many real minutes since it started. |

Set either timeout value to `0` to disable that rule.

**Ending an event does not automatically kill its surviving monsters.** They remain unless you remove them separately.

## 🛠️ Server Commands

Run these commands from the server console.

| Command | Description |
|---|---|
| `persistraid clear` | Destroy non-boss enemies. |
| `persistraid event` | Destroy event and raid creatures. |
| `persistraid stop` | Clear a stuck current random event so another raid can start. |
| `persistraid cap` | Immediately enforce `MaxActiveMonsters`. |
| `persistraid boss <prefab>` | Destroy bosses matching the specified prefab name. |

## 🧠 Raid Behavior & Compatibility

- Raid creatures receive extended sight and persistent hunting behavior.
- Raid AI is reinforced to prevent fleeing, losing aggression, or clearing its hunt state.
- Persistence applies to raid/event creatures, not ordinary open-world wildlife.
- Boss persistence can be configured separately with `NeverDespawnBosses`.
- Per-raid Custom Raids durations remain in control by default. Optional duration overrides are available if explicitly enabled.
