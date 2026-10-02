using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Cjayride.PersistentRaids
{
    internal static class ExtraRaids
    {
        internal static readonly List<RandomEvent> Stashed = new List<RandomEvent>();

        internal static void StashCurrent(RandEventSystem system)
        {
            if (system.m_randomEvent == null)
            {
                return;
            }

            Stashed.Add(system.m_randomEvent);
            CreatureRules.DebugLog("Stashed raid " + system.m_randomEvent.m_name + " extras=" + Stashed.Count);
            system.m_randomEvent = null;
        }

        internal static void StopAll(RandEventSystem system)
        {
            Stashed.Clear();
            if (system != null)
            {
                system.SetRandomEvent(null, Vector3.zero);
            }
        }

        internal static void ApplyDurationOverride(RandomEvent ev)
        {
            if (ev == null || !ModConfig.OverrideRaidDurations.Value)
            {
                return;
            }

            float seconds = Mathf.Max(1f, ModConfig.OverrideDurationSeconds.Value);
            CreatureRules.DebugLog("Duration override " + ev.m_name + " " + ev.m_duration + " -> " + seconds);
            ev.m_duration = seconds;
        }

        internal static void Tick(RandEventSystem system, float dt)
        {
            bool continueSpawn = ModConfig.ContinueSpawningWithoutPlayers.Value && CreatureRules.RulesActive();
            for (int i = Stashed.Count - 1; i >= 0; i--)
            {
                RandomEvent ev = Stashed[i];
                if (ev == null)
                {
                    Stashed.RemoveAt(i);
                    continue;
                }

                bool inArea = system.IsAnyPlayerInEventArea(ev);
                bool spawnOk = inArea || continueSpawn;
                if (ev.Update(true, true, spawnOk, dt))
                {
                    ev.OnStop();
                    Stashed.RemoveAt(i);
                    CreatureRules.DebugLog("Extra raid ended " + ev.m_name);
                }
            }
        }
    }

    [HarmonyPatch(typeof(RandEventSystem), nameof(RandEventSystem.StartRandomEvent))]
    internal static class PatchStartRandomEvent
    {
        private static void Prefix(RandEventSystem __instance)
        {
            if (!CreatureRules.RulesActive() || !ModConfig.AllowMultipleRaids.Value)
            {
                return;
            }

            if (__instance.m_randomEvent != null)
            {
                ExtraRaids.StashCurrent(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(RandEventSystem), nameof(RandEventSystem.SetRandomEvent))]
    internal static class PatchSetRandomEvent
    {
        private static void Postfix(RandEventSystem __instance)
        {
            if (!CreatureRules.RulesActive())
            {
                return;
            }

            ExtraRaids.ApplyDurationOverride(__instance.m_randomEvent);
        }
    }

    [HarmonyPatch(typeof(RandEventSystem), nameof(RandEventSystem.FixedUpdate))]
    internal static class PatchEventFixedUpdate
    {
        private static void Postfix(RandEventSystem __instance)
        {
            if (!ZNet.instance || !ZNet.instance.IsServer())
            {
                return;
            }

            if (!ModConfig.AllowMultipleRaids.Value || ExtraRaids.Stashed.Count == 0)
            {
                return;
            }

            ExtraRaids.Tick(__instance, Time.fixedDeltaTime);
        }
    }

    [HarmonyPatch(typeof(RandomEvent), nameof(RandomEvent.Update), typeof(bool), typeof(bool), typeof(bool), typeof(float))]
    internal static class PatchRandomEventUpdate
    {
        // Optional: keep the raid TIMER / spawn checks running when nobody is in the
        // circle. Off by default so PauseIfNoPlayerInArea and per-raid Duration behave
        // exactly as Custom Raids configured them.
        private static void Prefix(ref bool playerInArea)
        {
            if (!CreatureRules.RulesActive() || !ModConfig.ContinueSpawningWithoutPlayers.Value)
            {
                return;
            }

            playerInArea = true;
        }
    }
}
