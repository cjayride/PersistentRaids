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

Zone-unload persistence uses `ZDO.Persistent`. That writes creatures into the save. Keep the cap on.

## Changelog

See `CHANGELOG.md`. **1.0.5** leaves per-raid timers alone by default and hardens raid AI (Greylings fight). **1.0.4** scopes never-despawn to raid creatures. **1.0.3** stops blocking Custom Raids. **1.0.2** restricts long sight/hunt to raid creatures.
