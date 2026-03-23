using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.PauseMenu;
using MegaCrit.Sts2.Core.Runs;

namespace QuickReload;

[HarmonyPatch(typeof(NPauseMenu), nameof(NPauseMenu._Ready))]
static class QuickReloadPauseMenuPatch
{
    private const string QuickReloadNodeName = "QuickReload_QuickReloadButton";
    private static readonly LocString RestartLoc = new("gameplay_ui", "PAUSE_MENU.RESTART");

    static void Postfix(NPauseMenu __instance)
    {
        if (RunManager.Instance.NetService.Type == NetGameType.Client)
        {
            Log.Info("[QUICKRELOAD]: Quick Restart: client run detected, skipping button addition.");
            return;
        }

        var buttonContainer = __instance.GetNodeOrNull<VBoxContainer>("PanelContainer/ButtonContainer");
        if (buttonContainer == null)
        {
            Log.Warn("[QUICKRELOAD]: Quick Restart: couldn't find button container.");
            return;
        }

        if (buttonContainer.GetNodeOrNull<Node>(QuickReloadNodeName) != null)
        {
            Log.Warn("[QUICKRELOAD]: Quick Restart: button already exists, skipping.");
            return;
        }

        var saveAndQuitButton = buttonContainer.GetNode<NPauseMenuButton>("SaveAndQuit");
        var restartButton = saveAndQuitButton.Duplicate((int)(
            Node.DuplicateFlags.Groups |
            Node.DuplicateFlags.Scripts |
            Node.DuplicateFlags.UseInstantiation
        )) as NPauseMenuButton;

        if (restartButton == null)
        {
            Log.Warn("[QUICKRELOAD]: Quick Restart: failed to duplicate template button.");
            return;
        }

        restartButton.Name = QuickReloadNodeName;
        restartButton.GetNode<MegaLabel>("Label").SetTextAutoSize(RestartLoc.GetFormattedText());
        MakeVisualsUnique(restartButton);

        var pauseMenu = saveAndQuitButton.GetParent();
        pauseMenu.AddChild(restartButton);
        pauseMenu.MoveChild(restartButton, saveAndQuitButton.GetIndex());

        ConnectFocusNeighbors(buttonContainer, restartButton);

        restartButton.Connect(
            NClickableControl.SignalName.Released,
            Callable.From<NButton>(_ => OnQuickReloadPressed(__instance))
        );

        Log.Info("[QUICKRELOAD]: Quick Restart button added.");
    }

    private static void ConnectFocusNeighbors(VBoxContainer buttonContainer, NPauseMenuButton restartButton)
    {
        var buttons = new List<NPauseMenuButton>();
        foreach (var button in buttonContainer.GetChildren())
        {
            if (button is NPauseMenuButton { Visible: true } pauseMenuButton)
            {
                buttons.Add(pauseMenuButton);
            }
        }

        var index = buttons.IndexOf(restartButton);
        if (index <= 0 || index >= buttons.Count - 1)
        {
            Log.Warn("[QUICKRELOAD]: Quick Restart: unexpected button ordering, skipping focus neighbor update.");
            return;
        }

        var previousButton = buttons[index - 1];
        var nextButton = buttons[index + 1];

        previousButton.FocusNeighborBottom = restartButton.GetPath();
        restartButton.FocusNeighborTop = previousButton.GetPath();
        nextButton.FocusNeighborTop = restartButton.GetPath();
        restartButton.FocusNeighborBottom = nextButton.GetPath();
    }

    private static void MakeVisualsUnique(NPauseMenuButton button)
    {
        var image = button.GetNodeOrNull<TextureRect>("ButtonImage");
        if (image?.Material != null)
        {
            image.Material = image.Material.Duplicate() as Material;
        }

        var label = button.GetNodeOrNull<CanvasItem>("Label");
        if (label?.Material != null)
        {
            label.Material = label.Material.Duplicate() as Material;
        }
    }

    private static void OnQuickReloadPressed(NPauseMenu pauseMenu)
    {
        Log.Info("[QUICKRELOAD]: Quick Restart pressed.");
        TaskHelper.RunSafely(QuickReloadRunner.RestartAsync(pauseMenu));
    }
}
