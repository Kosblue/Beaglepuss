using System;
using Beaglepuss.Addon;
using Beaglepuss.Windows;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace Beaglepuss;

public enum StringIdentifier
{
    FakeFirstName,
    FakeLastName,
    FakeName,
    FakeFreeCompany,
    FakeFcTag,
    PaddedFakeFcTag,
    FakeSearchComment
}

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/disguise";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("Beaglepuss");
    private ConfigWindow ConfigWindow { get; init; }

    private UpdateNameplateHook UpdateNameplateHook { get; init; }
    private TargetAddons TargetAddons { get; init; }

    public AddonHandlerBase[] AddonHandlers { get; init; }

    public readonly StringManager<StringIdentifier> StringManager = new();

    public static string GetOwnName()
        => Services.ClientState.LocalPlayer?.Name.ToString() ?? "Unknown User";

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Services>();

        Configuration = Services.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);

        StringManager.SetString(StringIdentifier.FakeName, Configuration.GetFakeName());
        StringManager.SetString(StringIdentifier.FakeFirstName, Configuration.FakeFirstName);
        StringManager.SetString(StringIdentifier.FakeLastName, Configuration.FakeLastName);
        StringManager.SetString(StringIdentifier.FakeFreeCompany, Configuration.FakeFcName);
        StringManager.SetString(StringIdentifier.FakeFcTag, Configuration.FakeFcTag);
        StringManager.SetString(StringIdentifier.PaddedFakeFcTag, Configuration.GetPaddedFakeFcTag());
        StringManager.SetString(StringIdentifier.FakeSearchComment, Configuration.FakeSearchComment);

        WindowSystem.AddWindow(ConfigWindow);

        Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Open the plugin configuration window."
        });

        Services.PluginInterface.UiBuilder.Draw += DrawUI;
        Services.PluginInterface.UiBuilder.OpenMainUi += ToggleConfigUI;
        Services.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        UpdateNameplateHook = new UpdateNameplateHook(Configuration);
        TargetAddons = new TargetAddons(Configuration);

        PluginData data = new(Configuration, StringManager);

        AddonHandlers =
        [
            new CharacterListMenuAddon(data),
            new WideTextAddon(data),
            new BattleTalkAddon(data),
            new SearchInfoAddon(data),
            new EditSearchInfoAddon(data),
            new ExamineAddon(data),
            new SocialPanelAddon(data),
            new PartyBannerAddon(data),
            new PartyListAddon(data),
            new AdventurePlateAddons(data),
            new CharacterPanelAddon(data),
        ];
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();

        UpdateNameplateHook.Dispose();
        TargetAddons.Dispose();
        StringManager.Dispose();
        foreach (AddonHandlerBase handler in AddonHandlers)
        {
            handler.Dispose();
        }

        Services.CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, toggle the display status of our main ui
        ToggleConfigUI();
    }

    private void DrawUI()
        => WindowSystem.Draw();

    public void ToggleConfigUI()
        => ConfigWindow.Toggle();
}
