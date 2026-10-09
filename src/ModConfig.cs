using BepInEx.Configuration;

namespace Cjayride.PersistentRaids
{
    internal static class ModConfig
    {
        private static ConfigDescription Admin(string desc)
        {
            return new ConfigDescription(desc, null, new ConfigurationManagerAttributes { IsAdminOnly = true });
        }

        internal static ConfigEntry<bool> Enabled;

        internal static ConfigEntry<bool> EnablePersistentChase;
        internal static ConfigEntry<float> ChaseRangeMultiplier;
        internal static ConfigEntry<float> MaxChaseDistance;

        internal static ConfigEntry<bool> NeverDespawnMonsters;
        internal static ConfigEntry<bool> NeverDespawnBosses;
        internal static ConfigEntry<bool> EventMonstersSurviveRaidEnd;
        internal static ConfigEntry<bool> NeverDespawnNightMonsters;

        internal static ConfigEntry<float> EndEventAfterNoPlayersInAreaMinutes;
        internal static ConfigEntry<float> MaxRaidWallClockMinutes;

        internal static ConfigEntry<bool> AllowMultipleRaids;
        internal static ConfigEntry<bool> ContinueSpawningWithoutPlayers;
        internal static ConfigEntry<float> SpawnWithoutPlayersDuration;
        internal static ConfigEntry<bool> OverrideRaidDurations;
        internal static ConfigEntry<float> OverrideDurationSeconds;

        internal enum CapScopeMode
        {
            ServerWide,
            PerPlayerArea,
            Both
        }

        internal static ConfigEntry<bool> EnableMonsterCap;
        internal static ConfigEntry<CapScopeMode> CapScope;
        internal static ConfigEntry<int> MaxActiveMonsters;
        internal static ConfigEntry<int> MaxPerPlayerArea;
        internal static ConfigEntry<float> PlayerAreaRadius;
        internal static ConfigEntry<int> CapBehavior;
        internal static ConfigEntry<bool> OnlyApplyWhilePlayersOnline;
        internal static ConfigEntry<float> AutoCleanupInterval;

        internal static ConfigEntry<string> CreatureWhitelist;
        internal static ConfigEntry<string> CreatureBlacklist;

        internal static ConfigEntry<bool> EnableDebugLogging;
        internal static ConfigEntry<bool> LogCapEvents;

        internal static void Bind(ConfigFile config)
        {
            Enabled = config.Bind("General", "Enabled", true, Admin("Master switch."));

            EnablePersistentChase = config.Bind("RaidChase", "EnablePersistentChase", true, Admin("ALL RandomEvent/raid creatures hunt and fight (no shy flee). Open-world mobs stay vanilla."));
            ChaseRangeMultiplier = config.Bind("RaidChase", "ChaseRangeMultiplier", 3f, Admin("Multiplies view/alert/chase range for raid creatures only. 1 = vanilla."));
            MaxChaseDistance = config.Bind("RaidChase", "MaxChaseDistance", 200f, Admin("Absolute chase cap in meters. 0 = multiplier only."));

            NeverDespawnMonsters = config.Bind("Despawn", "NeverDespawnMonsters", true, Admin("Block far-despawn and mark ZDOs persistent for RAID/event creatures only. Open-world wildlife culls like vanilla so it does not pile up around bases."));
            NeverDespawnBosses = config.Bind("Despawn", "NeverDespawnBosses", true, Admin("Bosses persist / skip far-despawn. Hunt range is still raid-only."));
            EventMonstersSurviveRaidEnd = config.Bind("Despawn", "EventMonstersSurviveRaidEnd", true, Admin("Event creatures keep hunting after the raid timer ends."));
            NeverDespawnNightMonsters = config.Bind("Despawn", "NeverDespawnNightMonsters", true, Admin("Ignore despawn-in-day on night-spawned event creatures."));

            EndEventAfterNoPlayersInAreaMinutes = config.Bind(
                "RaidTimeout",
                "EndEventAfterNoPlayersInAreaMinutes",
                25f,
                Admin("When no player is in the raid event circle this many real minutes, end the random event so Custom Raids can roll again. Raid creatures may remain (EventMonstersSurviveRaidEnd). 0 = vanilla pause forever."));

            MaxRaidWallClockMinutes = config.Bind(
                "RaidTimeout",
                "MaxRaidWallClockMinutes",
                90f,
                Admin("Force-end the random event this many real minutes after it started, even if PauseIfNoPlayerInArea froze the timer. Safety net for stuck events. 0 = off."));

            AllowMultipleRaids = config.Bind("MultipleRaids", "AllowMultipleRaids", false, Admin("OFF by default. Custom Raids will not start a new raid while any event is still current. Leave false."));
            ContinueSpawningWithoutPlayers = config.Bind("MultipleRaids", "ContinueSpawningWithoutPlayers", false, Admin("OFF by default. When on, keeps the raid timer (and spawn checks) running with nobody in the circle. Leave off so each raid's PauseIfNoPlayerInArea and Duration work as configured."));
            SpawnWithoutPlayersDuration = config.Bind("MultipleRaids", "SpawnWithoutPlayersDuration", 600f, Admin("Unused (kept so old configs still load)."));
            OverrideRaidDurations = config.Bind("MultipleRaids", "OverrideRaidDurations", false, Admin("OFF by default. When on, replaces every random-event Duration with OverrideDurationSeconds. Leave off to use each Custom Raids Duration."));
            OverrideDurationSeconds = config.Bind("MultipleRaids", "OverrideDurationSeconds", 60f, Admin("Only used when OverrideRaidDurations is on. Seconds."));

            EnableMonsterCap = config.Bind("Safety", "EnableMonsterCap", true, Admin("Cap how many raid creatures can exist on the server at once. Only raid creatures count. Tames, wildlife, normal spawns, and bosses are never counted or removed."));
            CapScope = config.Bind("Safety", "CapScope", CapScopeMode.ServerWide, Admin("ServerWide = MaxActiveMonsters limits raid creatures on the whole server. PerPlayerArea = MaxPerPlayerArea limits raid creatures within PlayerAreaRadius of each player, which is what affects lag in a fight. Both = apply both limits."));
            MaxActiveMonsters = config.Bind("Safety", "MaxActiveMonsters", 60, Admin("Most raid creatures allowed on the whole server, including ones from earlier raids that are still alive. Used by ServerWide and Both."));
            MaxPerPlayerArea = config.Bind("Safety", "MaxPerPlayerArea", 40, Admin("Most raid creatures allowed within PlayerAreaRadius of any one player. Extras furthest from that player are removed first. Used by PerPlayerArea and Both."));
            PlayerAreaRadius = config.Bind("Safety", "PlayerAreaRadius", 200f, Admin("Radius in meters around each player for MaxPerPlayerArea. About 200 covers the area a player has loaded."));
            CapBehavior = config.Bind("Safety", "CapBehavior", 2, Admin("0 = only log when the cap is passed. 1 or 2 = remove the raid creatures furthest from any player until back at the cap."));
            OnlyApplyWhilePlayersOnline = config.Bind("Safety", "OnlyApplyWhilePlayersOnline", true, Admin("When no players are online, never-despawn rules are off so the server can unload."));
            AutoCleanupInterval = config.Bind("Safety", "AutoCleanupInterval", 2f, Admin("Minutes between cap checks on the server. 0 turns the automatic check off (persistraid cap still works)."));

            CreatureWhitelist = config.Bind("Filters", "CreatureWhitelist", "", Admin("Comma prefab names. Empty = all enemies."));
            CreatureBlacklist = config.Bind("Filters", "CreatureBlacklist", "", Admin("Comma prefab names to ignore."));

            EnableDebugLogging = config.Bind("Logging", "EnableDebugLogging", false, "Log blocked despawns and extra raids.");
            LogCapEvents = config.Bind("Logging", "LogCapEvents", true, "Log when the monster cap is hit.");
        }
    }
}
