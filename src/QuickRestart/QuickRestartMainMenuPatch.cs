using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Connection;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace MieMod.QuickRestart;

[HarmonyPatch(typeof(NMainMenu), nameof(NMainMenu._Ready))]
static class QuickRestartMainMenuPatch
{
    static void Postfix(NMainMenu __instance)
    {
        if (!QuickRestartState.TryConsumePendingRestart(out var playerId))
        {
            Log.Info("[MIEMOD]: Tried to consume pending restart on main menu, but none was pending.");
            return;
        }

        Log.Info($"[MIEMOD]: Consumed pending restart on main menu. playerId={playerId}");

        if (CommandLineHelper.HasArg("fastmp"))
        // if (true)
        {
            QuickRestartState.SetAutoReady(true);
            __instance.OpenMultiplayerSubmenu().OnJoinFriendsPressed();
            Log.Info("[MIEMOD]: Using fastmp quick-restart join flow via Join Friends screen.");
            return;
        }

        var steamInitializer = SteamClientConnectionInitializer.FromPlayer(playerId);
        if (steamInitializer == null)
        {
            Log.Warn("[MIEMOD]: Failed to create Steam connection initializer from player ID, aborting quick restart.");
            return;
        }

        QuickRestartState.SetAutoReady(true);
        TaskHelper.RunSafely(__instance.JoinGame(steamInitializer));
    }
}
