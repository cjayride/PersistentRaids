# Changelog

## 1.0.8

- **RaidTimeout:** End the random event when nobody is in the event area for 25 minutes (configurable), or after 90 minutes wall-clock since start. Unblocks Custom Raids after sailing away. Mobs may remain per `EventMonstersSurviveRaidEnd`.

## 1.0.7

- Raid creature cap counts only raid creatures, from the server's saved records, so it works on a dedicated server. Tames, wildlife, normal spawns, and bosses are never counted or removed.
- New `CapScope`: `ServerWide` (default, `MaxActiveMonsters` 60), `PerPlayerArea` (`MaxPerPlayerArea` 40 within `PlayerAreaRadius` 200 m), or `Both`. Extras furthest from players are removed first.
- Cap check default interval is 2 minutes. `persistraid clear` no longer deletes tames.

## 1.0.6

- ALL raid/event creatures are forced to fight: block flee, block hunt-clear, re-assert hunt+alert every AI tick. Applies to every raid (Greyling, Boar, Neck, Greydwarf, etc.), not only on spawn.

## 1.0.5

- Raid timers: `ContinueSpawningWithoutPlayers` now defaults **off**, so each Custom Raids `Duration` and `PauseIfNoPlayerInArea` apply as written. PersistentRaids no longer forces the event clock by default.
- New optional `OverrideRaidDurations` + `OverrideDurationSeconds` (both unused unless you turn the override **on**). Leave off to keep per-raid durations.
- Raid creature AI: event mobs force hunt + alerted, and clear shy flee flags (`fleeIfNotAlerted`, unreachable flee, low-health flee). Fixes Greylings milling around instead of fighting.

## 1.0.4

- Persistence is now raid-only. Never-despawn and `ZDO.Persistent` apply only to raid/event creatures (and bosses via `NeverDespawnBosses`).
- Open-world wildlife (boars, necks, greylings, etc.) culls like vanilla again, so it no longer accumulates around bases. Fixes base-area FPS drops caused by permanent wildlife.
- Raid mobs get persistence when the event flag is set (`SetEventCreature`), not blindly on `Awake`.

## 1.0.3

- Fix: 1.0.1 treated every event-area check as “a player is here” and could expose a stashed raid as the current event. Custom Raids refuses to start a new raid while `GetCurrentRandomEvent()` is set, so raids stopped appearing.
- `AllowMultipleRaids` defaults to false. Custom Raids owns raid start; this mod persists raid creatures.
- `persistraid stop` clears a stuck current/stashed random event.

## 1.0.2

- Long-range sight, forced hunt, and never-lose-aggro apply **only** to RandomEvent / raid creatures (`SetEventCreature`).
- Open-world mobs (boars, greydwarves, and other non-raid spawns) keep vanilla view range and aggro.
- Night despawn skip is raid-only.
- World bosses no longer inherit raid hunt from `NeverDespawnBosses` (far-despawn persist is unchanged).

## 1.0.1

- Initial release: raid chase, far-despawn block, extra events, spawn-without-players, monster cap 220.
