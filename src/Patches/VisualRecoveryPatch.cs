using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace QuickReload;

[HarmonyPatch(typeof(NRun), nameof(NRun._Ready))]
static class VisualRecoveryPatch
{
    static void Postfix(NRun __instance)
    {
        if (!QuickReloadState.TryConsumePendingVisualRecovery())
        {
            return;
        }

        TaskHelper.RunSafely(RecoverVisualStateAsync(__instance));
    }

    private static async Task RecoverVisualStateAsync(NRun run)
    {
        Log.Info("[QUICKRELOAD]: Starting visual recovery process after reload.");
        try
        {
            var tree = run.GetTree();
            var game = NGame.Instance;
            if (tree == null || game?.Transition == null)
            {
                return;
            }

            const int maxFramesToWait = 180; // ~3 seconds at 60fps.
            for (var i = 0; i < maxFramesToWait; i++)
            {
                if (!game.Transition.InTransition)
                {
                    NModalContainer.Instance?.Clear();
                    Log.Info("[QUICKRELOAD]: Transition recovered naturally; forced visual recovery not needed.");
                    return;
                }

                await run.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
            }

            await game.Transition.FadeIn(0.2f);
            NModalContainer.Instance?.Clear();
            Log.Info("[QUICKRELOAD]: Applied forced visual recovery fade-in after transition timeout.");
        }
        catch (Exception ex)
        {
            Log.Warn($"[QUICKRELOAD]: Visual recovery failed: {ex}");
        }
    }
}
