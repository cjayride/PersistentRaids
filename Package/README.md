# PersistentRaids

**Raid/event** monsters keep hunting and get longer sight. Open-world creatures (boars, greydwarves, etc.) stay vanilla.

Far-despawn can still be blocked for enemies. Raid **mobs** keep hunting after the HUD ends. Custom Raids owns when the next raid starts. Raid creature cap defaults to 60 server-wide (`CapScope`).

Do not enable Expand World Events Multiple events. Leave `AllowMultipleRaids = false`.

Install on **server and all clients**.

Commands: `persistraid clear | event | stop | cap | boss <prefab>`

## Changelog

### 1.0.8

- **RaidTimeout:** If nobody is in the event circle for 25 real minutes, or the event has been running 90 minutes, the random event ends so Custom Raids can roll again (mobs may stay).

### 1.0.7

- Cap counts only raid creatures and works on a dedicated server. Tames, wildlife, and bosses are never removed.
- `CapScope`: `ServerWide` (default 60), `PerPlayerArea` (default 40 within 200 m), or `Both`. Furthest from players go first.

### 1.0.6

- ALL raid/event creatures forced to fight every AI tick (block flee, keep hunt). Every raid, every shy prefab.

### 1.0.5

- Timers: leave per-raid Custom Raids `Duration` / `PauseIfNoPlayerInArea` alone by default. Optional `OverrideRaidDurations` is off.
- Raid AI: force hunt + alert, clear shy flee flags so Greylings fight instead of milling.

### 1.0.4

- Persistence is raid-only now. Never-despawn / `ZDO.Persistent` apply only to raid/event creatures (and bosses).
- Open-world wildlife culls like vanilla, so it no longer piles up around bases (fixes base-area FPS).

### 1.0.3

- Fix: 1.0.1 lied that a player was always inside the raid circle and could report a stashed event as “current”. Custom Raids then skipped every new raid.
- `AllowMultipleRaids` now defaults **false**. `persistraid stop` clears a stuck current event.

### 1.0.2

- Long-range sight, forced hunt, and never-lose-aggro apply **only** to RandomEvent / raid creatures.
- Open-world mobs keep vanilla view range and aggro.
- Night despawn skip is raid-only.
- World bosses no longer inherit raid hunt from `NeverDespawnBosses`.

### 1.0.1

- Initial release.

## Config Values


[Despawn]

Block far-despawn and mark ZDOs persistent for RAID/event creatures only. Open-world wildlife culls like vanilla so it does not pile up around bases.
Setting type: Boolean
Default value: true
NeverDespawnMonsters = true

Bosses persist / skip far-despawn. Hunt range is still raid-only.
Setting type: Boolean
Default value: true
NeverDespawnBosses = true

Event creatures keep hunting after the raid timer ends.
Setting type: Boolean
Default value: true
EventMonstersSurviveRaidEnd = true

Ignore despawn-in-day on night-spawned event creatures.
Setting type: Boolean
Default value: true
NeverDespawnNightMonsters = true

[Filters]

Comma prefab names. Empty = all enemies.
Setting type: String
Default value: 
CreatureWhitelist = 

Comma prefab names to ignore.
Setting type: String
Default value: 
CreatureBlacklist = 

[General]

Master switch.
Setting type: Boolean
Default value: true
Enabled = true

[Logging]

Log blocked despawns and extra raids.
Setting type: Boolean
Default value: false
EnableDebugLogging = false

Log when the monster cap is hit.
Setting type: Boolean
Default value: true
LogCapEvents = true

[MultipleRaids]

Leave false. Custom Raids will not start a new raid while any event is still current.
Setting type: Boolean
Default value: false
AllowMultipleRaids = false

OFF by default. When on, keeps the raid timer (and spawn checks) running with nobody in the circle. Leave off so each raid's PauseIfNoPlayerInArea and Duration work as configured.
Setting type: Boolean
Default value: false
ContinueSpawningWithoutPlayers = false

Unused (kept so old configs still load).
Setting type: Single
Default value: 600
SpawnWithoutPlayersDuration = 600

OFF by default. When on, replaces every random-event Duration with OverrideDurationSeconds. Leave off to use each Custom Raids Duration.
Setting type: Boolean
Default value: false
OverrideRaidDurations = false

Only used when OverrideRaidDurations is on. Seconds.
Setting type: Single
Default value: 60
OverrideDurationSeconds = 60

[RaidChase]

Only RandomEvent/raid creatures keep hunt/aggro. Open-world mobs stay vanilla.
Setting type: Boolean
Default value: true
EnablePersistentChase = true

Multiplies view/alert/chase range for raid creatures only. 1 = vanilla.
Setting type: Single
Default value: 3
ChaseRangeMultiplier = 3

Absolute chase cap in meters. 0 = multiplier only.
Setting type: Single
Default value: 200
MaxChaseDistance = 200

[Safety]

Hard cap on living non-player characters.
Setting type: Boolean
Default value: true
EnableMonsterCap = true

Global living monster cap.
Setting type: Int32
Default value: 220
MaxActiveMonsters = 220

0 = stop persisting new ones. 1 = despawn oldest. 2 = despawn furthest from any player.
Setting type: Int32
Default value: 1
CapBehavior = 1

When no players are online, never-despawn rules are off so the server can unload.
Setting type: Boolean
Default value: true
OnlyApplyWhilePlayersOnline = true

Minutes between cap sweeps. 0 = only on spawn.
Setting type: Single
Default value: 15
AutoCleanupInterval = 15

## Contact

𝕏: x.com/cjayride

Discord: discord.gg/cjayride (find me at the top of the user list) "cjayride"

Twitch: twitch.tv/cjayride

## AI Generated

This code was AI Generated.