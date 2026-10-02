using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cjayride.PersistentRaids
{
    internal static class CreatureRules
    {
        internal static bool PlayersOnline()
        {
            if (ZNet.instance == null)
            {
                return false;
            }

            if (ZNet.instance.IsServer())
            {
                return ZNet.instance.GetPeers().Count > 0 || Player.m_localPlayer != null;
            }

            return Player.m_localPlayer != null;
        }

        internal static bool RulesActive()
        {
            if (ModConfig.Enabled == null || !ModConfig.Enabled.Value)
            {
                return false;
            }

            if (ModConfig.OnlyApplyWhilePlayersOnline.Value && !PlayersOnline())
            {
                return false;
            }

            return true;
        }

        internal static bool PassesFilter(Character character)
        {
            if (character == null || character.IsPlayer())
            {
                return false;
            }

            string prefab = character.gameObject.name.Replace("(Clone)", string.Empty);
            string blacklist = ModConfig.CreatureBlacklist.Value;
            if (!string.IsNullOrWhiteSpace(blacklist))
            {
                foreach (string item in blacklist.Split(','))
                {
                    if (prefab.Equals(item.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            string whitelist = ModConfig.CreatureWhitelist.Value;
            if (string.IsNullOrWhiteSpace(whitelist))
            {
                return true;
            }

            foreach (string item in whitelist.Split(','))
            {
                if (prefab.Equals(item.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsEnemy(Character character)
        {
            if (character == null || character.IsPlayer())
            {
                return false;
            }

            Character.Faction faction = character.GetFaction();
            return faction != Character.Faction.Players && faction != Character.Faction.Dverger;
        }

        internal static bool ShouldPersist(Character character, MonsterAI monster)
        {
            if (!RulesActive() || !PassesFilter(character) || !IsEnemy(character))
            {
                return false;
            }

            if (character.IsBoss())
            {
                return ModConfig.NeverDespawnBosses.Value;
            }

            // Only raid/event creatures persist. Open-world wildlife culls like vanilla
            // so it does not accumulate around bases.
            return ModConfig.NeverDespawnMonsters.Value && IsRaidCreature(monster);
        }

        internal static void ApplyPersistence(MonsterAI monster)
        {
            if (monster == null)
            {
                return;
            }

            Character character = monster.m_character;
            if (character == null || !ShouldPersist(character, monster))
            {
                return;
            }

            ZNetView nview = monster.m_nview;
            if (nview == null || !nview.IsValid())
            {
                return;
            }

            ZDO zdo = nview.GetZDO();
            if (zdo != null && !zdo.Persistent)
            {
                zdo.Persistent = true;
            }
        }

        internal static bool IsRaidCreature(MonsterAI monster)
        {
            return monster != null && monster.IsEventCreature();
        }

        internal static bool ShouldKeepHunt(BaseAI ai)
        {
            if (!RulesActive() || ai == null || ai.m_character == null || !PassesFilter(ai.m_character))
            {
                return false;
            }

            MonsterAI monster = ai as MonsterAI;
            if (!IsRaidCreature(monster))
            {
                return false;
            }

            return ModConfig.EnablePersistentChase.Value || ModConfig.EventMonstersSurviveRaidEnd.Value;
        }

        internal static void ApplyRaidChase(MonsterAI monster)
        {
            if (monster == null || !RulesActive() || !IsRaidCreature(monster) || !PassesFilter(monster.m_character))
            {
                return;
            }

            if (!ModConfig.EnablePersistentChase.Value && !ModConfig.EventMonstersSurviveRaidEnd.Value)
            {
                return;
            }

            EnforceFightAi(monster);

            if (!ModConfig.EnablePersistentChase.Value)
            {
                return;
            }

            if (monster.GetComponent<RaidChaseApplied>() == null)
            {
                monster.gameObject.AddComponent<RaidChaseApplied>();
                float mult = Mathf.Max(1f, ModConfig.ChaseRangeMultiplier.Value);
                monster.m_maxChaseDistance *= mult;
                monster.m_viewRange *= mult;
                monster.m_alertRange *= mult;
                float cap = ModConfig.MaxChaseDistance.Value;
                if (cap > 0f)
                {
                    monster.m_maxChaseDistance = Mathf.Min(monster.m_maxChaseDistance, cap);
                }
            }
        }

        // Re-assert every AI tick. Vanilla clears hunt when the event HUD ends and
        // shy prefabs (Greyling, Boar, Neck, etc.) keep trying to flee.
        internal static void EnforceFightAi(MonsterAI monster)
        {
            if (monster == null)
            {
                return;
            }

            monster.m_fleeIfNotAlerted = false;
            monster.m_fleeIfHurtWhenTargetCantBeReached = false;
            monster.m_fleeIfLowHealth = 0f;
            monster.m_enableHuntPlayer = true;
            if (!monster.m_huntPlayer)
            {
                monster.SetHuntPlayer(true);
            }

            if (!monster.IsAlerted())
            {
                monster.SetAlerted(true);
            }
        }

        internal sealed class RaidChaseApplied : MonoBehaviour
        {
        }

        internal static void DebugLog(string message)
        {
            if (ModConfig.EnableDebugLogging != null && ModConfig.EnableDebugLogging.Value)
            {
                Plugin.Log.LogInfo(message);
            }
        }
    }
}
