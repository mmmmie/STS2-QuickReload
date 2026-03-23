using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace QuickReload;

[ModInitializer("Init")]
public class Entry
{
    public static void Init()
    {
        var harmony = new Harmony("QuickReload");
        harmony.PatchAll();
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        Log.Info("[QUICKRELOAD]: QuickReload mod initialized!");
    }
}
