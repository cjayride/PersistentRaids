using UnityEngine;

namespace Cjayride.PersistentRaids
{
    /// <summary>
    /// Ends stuck random events when nobody remains in the event circle (e.g. sailing away)
    /// or when a wall-clock cap is exceeded. Does not remove surviving raid creatures.
    /// </summary>
    internal static class RaidEventTimeout
    {
        private static float _raidStartedAt;
        private static float _lastPlayerInAreaAt;
        private static string _trackedName;

        internal static void Reset()
        {
            _trackedName = null;
        }

        internal static void OnEventSet(RandomEvent ev)
        {
            if (ev == null)
            {
                Reset();
                return;
            }

            _raidStartedAt = Time.time;
            _lastPlayerInAreaAt = Time.time;
            _trackedName = ev.m_name;
        }

        internal static void Tick(RandEventSystem system)
        {
            if (!CreatureRules.RulesActive() || system == null)
            {
                return;
            }

            RandomEvent ev = system.m_randomEvent;
            if (ev == null)
            {
                Reset();
                return;
            }

            float abandonMin = ModConfig.EndEventAfterNoPlayersInAreaMinutes.Value;
            float maxWallMin = ModConfig.MaxRaidWallClockMinutes.Value;
            if (abandonMin <= 0f && maxWallMin <= 0f)
            {
                return;
            }

            if (_trackedName != ev.m_name)
            {
                OnEventSet(ev);
            }

            float now = Time.time;

            if (maxWallMin > 0f && now - _raidStartedAt >= maxWallMin * 60f)
            {
                EndEvent(system, ev, "wall-clock max " + maxWallMin + " min");
                return;
            }

            if (IsAnyPlayerInEventArea(ev))
            {
                _lastPlayerInAreaAt = now;
                return;
            }

            if (abandonMin > 0f && now - _lastPlayerInAreaAt >= abandonMin * 60f)
            {
                EndEvent(system, ev, "no players in event area for " + abandonMin + " min");
            }
        }

        private static bool IsAnyPlayerInEventArea(RandomEvent re)
        {
            if (ZNet.instance == null || re == null)
            {
                return false;
            }

            foreach (ZDO zdo in ZNet.instance.GetAllCharacterZDOS())
            {
                Vector3 pos = zdo.GetPosition();
                if (pos.y > 3000f)
                {
                    continue;
                }

                float dx = pos.x - re.m_pos.x;
                float dz = pos.z - re.m_pos.z;
                if (dx * dx + dz * dz < re.m_eventRange * re.m_eventRange)
                {
                    return true;
                }
            }

            return false;
        }

        private static void EndEvent(RandEventSystem system, RandomEvent ev, string reason)
        {
            Plugin.Log.LogInfo("PersistentRaids: ending raid '" + ev.m_name + "' (" + reason + ")");
            CreatureRules.DebugLog("Raid timeout " + ev.m_name + ": " + reason);
            Reset();
            system.ResetRandomEvent();
        }
    }
}
