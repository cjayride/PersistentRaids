using HarmonyLib;
using UnityEngine;

namespace Cjayride.PersistentRaids
{
    [HarmonyPatch(typeof(BaseAI), nameof(BaseAI.MoveAwayAndDespawn), typeof(float), typeof(bool))]
    internal static class PatchMoveAwayAndDespawn
    {
        private static bool Prefix(BaseAI __instance)
        {
            if (__instance.m_character != null && CreatureRules.ShouldPersist(__instance.m_character, __instance as MonsterAI))
            {
                CreatureRules.DebugLog("Blocked MoveAwayAndDespawn " + __instance.m_character.name);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.HuntPlayer))]
    internal static class PatchMonsterHuntPlayer
    {
        private static bool Prefix(MonsterAI __instance, ref bool __result)
        {
            if (!CreatureRules.ShouldKeepHunt(__instance))
            {
                return true;
            }

            if (__instance.DespawnInDay() && EnvMan.IsDay() && !ModConfig.NeverDespawnNightMonsters.Value)
            {
                return true;
            }

            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(BaseAI), nameof(BaseAI.SetAlerted))]
    internal static class PatchSetAlerted
    {
        private static bool Prefix(BaseAI __instance, bool alert)
        {
            if (!alert && CreatureRules.ShouldKeepHunt(__instance))
            {
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(BaseAI), nameof(BaseAI.SetHuntPlayer))]
    internal static class PatchSetHuntPlayer
    {
        private static bool Prefix(BaseAI __instance, bool hunt)
        {
            // Vanilla UpdateAI clears hunt when HaveActiveEvent is false. Keep raid
            // creatures hunting for every raid while our fight rules apply.
            if (!hunt && CreatureRules.ShouldKeepHunt(__instance))
            {
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(BaseAI), nameof(BaseAI.Flee), typeof(float), typeof(Vector3))]
    internal static class PatchFlee
    {
        private static bool Prefix(BaseAI __instance, ref bool __result)
        {
            if (CreatureRules.ShouldKeepHunt(__instance))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.UpdateAI))]
    internal static class PatchMonsterUpdateAI
    {
        private static void Postfix(MonsterAI __instance)
        {
            if (!CreatureRules.ShouldKeepHunt(__instance))
            {
                return;
            }

            CreatureRules.EnforceFightAi(__instance);
        }
    }

    [HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.Awake))]
    internal static class PatchMonsterAwake
    {
        private static void Postfix(MonsterAI __instance)
        {
            // At Awake the event-creature flag is usually not set yet, so raid mobs get
            // their persistence from PatchSetEventCreature. Bosses persist from here.
            CreatureRules.ApplyRaidChase(__instance);
            CreatureRules.ApplyPersistence(__instance);

            // Cap is not applied from Awake — destroying ZDOs while the zone loads crashes the client.
        }
    }

    [HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.SetEventCreature))]
    internal static class PatchSetEventCreature
    {
        private static void Postfix(MonsterAI __instance)
        {
            CreatureRules.ApplyRaidChase(__instance);
            CreatureRules.ApplyPersistence(__instance);
        }
    }

    [HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.SetDespawnInDay))]
    internal static class PatchSetDespawnInDay
    {
        private static bool Prefix(MonsterAI __instance, ref bool despawn)
        {
            if (despawn && CreatureRules.RulesActive() && ModConfig.NeverDespawnNightMonsters.Value && CreatureRules.IsRaidCreature(__instance) && CreatureRules.PassesFilter(__instance.m_character))
            {
                despawn = false;
            }

            return true;
        }
    }
}
