using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cjayride.PersistentRaids
{
    internal static class CapService
    {
        // Prefab hash -> true when the prefab is a non-boss MonsterAI creature that passes the filters.
        private static readonly Dictionary<int, bool> CountablePrefabs = new Dictionary<int, bool>();
        private static string _filterKey;

        internal static void EnforceCap()
        {
            if (!ModConfig.EnableMonsterCap.Value || ZNet.instance == null || !ZNet.instance.IsServer() || ZDOMan.instance == null)
            {
                return;
            }

            List<Vector3> players = PlayerPositions();
            List<ZDO> raid = RaidZdos();
            ModConfig.CapScopeMode scope = ModConfig.CapScope.Value;

            if (scope != ModConfig.CapScopeMode.ServerWide)
            {
                float radius = Mathf.Max(1f, ModConfig.PlayerAreaRadius.Value);
                float radiusSqr = radius * radius;
                int areaCap = Mathf.Max(0, ModConfig.MaxPerPlayerArea.Value);
                for (int p = 0; p < players.Count; p++)
                {
                    Vector3 player = players[p];
                    List<ZDO> near = new List<ZDO>();
                    for (int i = 0; i < raid.Count; i++)
                    {
                        if ((raid[i].GetPosition() - player).sqrMagnitude <= radiusSqr)
                        {
                            near.Add(raid[i]);
                        }
                    }

                    if (near.Count <= areaCap)
                    {
                        continue;
                    }

                    near.Sort((a, b) => (b.GetPosition() - player).sqrMagnitude.CompareTo((a.GetPosition() - player).sqrMagnitude));
                    RemoveExtras(near, near.Count - areaCap, raid, "within " + radius + " m of a player (cap " + areaCap + ")");
                }
            }

            if (scope != ModConfig.CapScopeMode.PerPlayerArea)
            {
                int cap = Mathf.Max(0, ModConfig.MaxActiveMonsters.Value);
                if (raid.Count > cap)
                {
                    raid.Sort((a, b) => NearestPlayer(b.GetPosition(), players).CompareTo(NearestPlayer(a.GetPosition(), players)));
                    RemoveExtras(new List<ZDO>(raid), raid.Count - cap, raid, "on the server (cap " + cap + ")");
                }
            }
        }

        private static void RemoveExtras(List<ZDO> ordered, int extra, List<ZDO> raid, string where)
        {
            if (ModConfig.LogCapEvents.Value)
            {
                Plugin.Log.LogInfo("Raid creature cap hit: " + ordered.Count + " raid creatures " + where + ".");
            }

            if (ModConfig.CapBehavior.Value == 0)
            {
                return;
            }

            long self = ZDOMan.GetSessionID();
            for (int i = 0; i < extra && i < ordered.Count; i++)
            {
                ZDO zdo = ordered[i];
                zdo.SetOwner(self);
                ZDOMan.instance.DestroyZDO(zdo);
                raid.Remove(zdo);
            }

            if (ModConfig.LogCapEvents.Value)
            {
                Plugin.Log.LogInfo("Removed " + extra + " raid creatures, furthest first.");
            }
        }

        private static List<ZDO> RaidZdos()
        {
            RefreshFilterCache();
            List<ZDO> raid = new List<ZDO>();
            foreach (ZDO zdo in ZDOMan.instance.m_objectsByID.Values)
            {
                if (zdo == null || !IsCountable(zdo.GetPrefab()))
                {
                    continue;
                }

                if (!zdo.GetBool(ZDOVars.s_eventCreature) || zdo.GetBool(ZDOVars.s_tamed))
                {
                    continue;
                }

                raid.Add(zdo);
            }

            return raid;
        }

        private static bool IsCountable(int prefabHash)
        {
            if (CountablePrefabs.TryGetValue(prefabHash, out bool countable))
            {
                return countable;
            }

            countable = false;
            GameObject prefab = ZNetScene.instance != null ? ZNetScene.instance.GetPrefab(prefabHash) : null;
            if (prefab != null && prefab.GetComponent<MonsterAI>() != null)
            {
                Character character = prefab.GetComponent<Character>();
                countable = character != null && !character.m_boss && CreatureRules.PassesFilter(character);
            }

            CountablePrefabs[prefabHash] = countable;
            return countable;
        }

        private static void RefreshFilterCache()
        {
            string key = ModConfig.CreatureWhitelist.Value + "|" + ModConfig.CreatureBlacklist.Value;
            if (key != _filterKey)
            {
                _filterKey = key;
                CountablePrefabs.Clear();
            }
        }

        private static List<Vector3> PlayerPositions()
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (ZNetPeer peer in ZNet.instance.GetPeers())
            {
                if (peer != null && peer.IsReady())
                {
                    positions.Add(peer.m_refPos);
                }
            }

            if (Player.m_localPlayer != null)
            {
                positions.Add(Player.m_localPlayer.transform.position);
            }

            return positions;
        }

        private static float NearestPlayer(Vector3 position, List<Vector3> players)
        {
            float best = float.MaxValue;
            for (int i = 0; i < players.Count; i++)
            {
                float d = (players[i] - position).sqrMagnitude;
                if (d < best)
                {
                    best = d;
                }
            }

            return best;
        }

        internal static int Clear(bool eventOnly, bool includeBosses, string prefabFilter)
        {
            int n = 0;
            List<Character> all = new List<Character>(Character.GetAllCharacters());
            for (int i = 0; i < all.Count; i++)
            {
                Character c = all[i];
                if (!CreatureRules.IsEnemy(c) || c.IsTamed())
                {
                    continue;
                }

                if (!includeBosses && c.IsBoss())
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(prefabFilter)
                    && !c.gameObject.name.Replace("(Clone)", string.Empty).Equals(prefabFilter, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (eventOnly)
                {
                    MonsterAI ai = c.GetComponent<MonsterAI>();
                    if (ai == null || !ai.IsEventCreature())
                    {
                        continue;
                    }
                }

                ZNetView nv = c.GetComponent<ZNetView>();
                if (nv != null && nv.IsValid())
                {
                    nv.Destroy();
                    n++;
                }
            }

            return n;
        }
    }
}
