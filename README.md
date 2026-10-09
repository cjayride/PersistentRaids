# PersistentRaids

Valheim BepInEx mod: **raid/event** monsters keep hunting and get longer sight. Open-world creatures stay vanilla. Custom Raids owns when raids start.

**Do not enable Expand World Events “Multiple events”.** Leave `AllowMultipleRaids` false.

Install on server and all clients.

## Commands (server)

`persistraid clear` — destroy non-boss enemies  
`persistraid event` — destroy event/raid creatures  
`persistraid stop` — clear a stuck current random event so the next raid can start  
`persistraid cap` — enforce MaxActiveMonsters  
`persistraid boss <prefab>` — destroy matching boss prefab  

## Safety

Default cap 220. `OnlyApplyWhilePlayersOnline` turns persistence off when the server is empty so zones can unload.

## Stuck raids (sail away)

With Custom Raids `PauseIfNoPlayerInArea`, the event timer freezes when nobody is in the circle, which blocks the next raid. **`RaidTimeout`** (defaults on): end the random event after **25** real minutes with no players in the circle, or **90** minutes after start (wall clock). Surviving mobs are unchanged unless you clear them. Set either minutes value to **0** to disable that rule.

Zone-unload persistence uses `ZDO.Persistent`. That writes creatures into the save. Keep the cap on.

## Changelog

See `CHANGELOG.md`. **1.0.5** leaves per-raid timers alone by default and hardens raid AI (Greylings fight). **1.0.4** scopes never-despawn to raid creatures. **1.0.3** stops blocking Custom Raids. **1.0.2** restricts long sight/hunt to raid creatures.
