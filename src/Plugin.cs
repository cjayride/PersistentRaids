using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace Cjayride.PersistentRaids
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "cjayride.persistentraids";
        public const string PluginName = "PersistentRaids";
        public const string PluginVersion = "1.0.9";

        internal static Plugin Instance;
        internal static Harmony Harmony;
        internal static BepInEx.Logging.ManualLogSource Log => Instance.Logger;

        private void Awake()
        {
            Instance = this;
            ModConfig.Bind(Config);
            Harmony = new Harmony(PluginGUID);
            Harmony.PatchAll(Assembly.GetExecutingAssembly());

            _ = new Terminal.ConsoleCommand("persistraid", "clear | event | stop | cap | boss <prefab> — PersistentRaids cleanup", args =>
            {
                if (!ZNet.instance || !ZNet.instance.IsServer())
                {
                    args.Context.AddString("PersistentRaids: server only");
                    return;
                }

                if (args.Length < 2)
                {
                    args.Context.AddString("persistraid clear | event | stop | cap | boss <prefab>");
                    return;
                }

                string verb = args[1].ToLowerInvariant();
                int n;
                switch (verb)
                {
                    case "clear":
                        n = CapService.Clear(false, false, null);
                        args.Context.AddString("Cleared " + n + " non-boss monsters");
                        break;
                    case "event":
                        n = CapService.Clear(true, false, null);
                        args.Context.AddString("Cleared " + n + " event monsters");
                        break;
                    case "stop":
                        ExtraRaids.StopAll(RandEventSystem.instance);
                        args.Context.AddString("Cleared current/stashed random events");
                        break;
                    case "cap":
                        CapService.EnforceCap();
                        args.Context.AddString("Cap enforced");
                        break;
                    case "boss":
                        if (args.Length < 3)
                        {
                            args.Context.AddString("persistraid boss <prefab>");
                            break;
                        }

                        n = CapService.Clear(false, true, args[2]);
                        args.Context.AddString("Cleared " + n + " matching bosses");
                        break;
                    default:
                        args.Context.AddString("persistraid clear | event | stop | cap | boss <prefab>");
                        break;
                }
            });

            if (ModConfig.AutoCleanupInterval.Value > 0f)
            {
                InvokeRepeating(nameof(TickCap), 60f, ModConfig.AutoCleanupInterval.Value * 60f);
            }

            Logger.LogInfo(PluginName + " " + PluginVersion + " loaded. Custom Raids owns raid start. This mod only persists raid creatures.");
        }

        private void TickCap()
        {
            CapService.EnforceCap();
        }
    }
}
