using Beaglepuss.Addon;
using Beaglepuss.Windows;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace Beaglepuss;

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/disguise";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("Beaglepuss");
    private ConfigWindow ConfigWindow { get; init; }

    private UpdateNameplateHook UpdateNameplateHook { get; init; }
    private TargetAddons TargetAddons { get; init; }
    private AdventurePlateAddons AdventurePlateAddons { get; init; }
    private PartyListAddon PartyListAddon { get; init; }
    private CharacterPanelAddon CharacterPanelAddon { get; init; }
    private SocialPanelAddon SocialPanelAddon { get; init; }
    private ExamineAddon ExamineAddon { get; init; }
    private SearchInfoAddon SearchInfoAddon { get; init; }
    private PartyBannerAddon PartyBannerAddon { get; init; }

    public static string GetOwnName()
        => Services.ClientState.LocalPlayer?.Name.ToString() ?? "Unknown User";

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Services>();

        Configuration = Services.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);

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
        AdventurePlateAddons = new AdventurePlateAddons(Configuration);
        PartyListAddon = new PartyListAddon(Configuration);
        CharacterPanelAddon = new CharacterPanelAddon(Configuration);
        SocialPanelAddon = new SocialPanelAddon(Configuration);
        ExamineAddon = new ExamineAddon(Configuration);
        SearchInfoAddon = new SearchInfoAddon(Configuration);
        PartyBannerAddon = new PartyBannerAddon(Configuration);
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();

        UpdateNameplateHook.Dispose();
        TargetAddons.Dispose();
        AdventurePlateAddons.Dispose();
        PartyListAddon.Dispose();
        CharacterPanelAddon.Dispose();
        SocialPanelAddon.Dispose();
        ExamineAddon.Dispose();
        SearchInfoAddon.Dispose();
        PartyBannerAddon.Dispose();

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
