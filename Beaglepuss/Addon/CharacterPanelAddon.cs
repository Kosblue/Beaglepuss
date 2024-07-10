using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class CharacterPanelAddon : IDisposable
{
    private readonly Configuration config;
    public CharacterPanelAddon(Configuration config)
    {
        this.config = config;
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "Character", OnCharacterPanelUpdate);
    }

    public void Dispose()
    {
        Services.AddonLifecycle.UnregisterListener(OnCharacterPanelUpdate);
    }

    private void OnCharacterPanelUpdate(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon;
        if (!addon->IsVisible) { return; }

        IntPtr profilePtr = Services.GameGui.GetAddonByName("CharacterProfile");
        if (profilePtr != IntPtr.Zero)
        {
            AtkUnitBase* profile = (AtkUnitBase*)profilePtr;
            UpdateCharacterProfilePanel(profile);
        }

        AtkTextNode* nameNode = addon->GetNodeById(2)->GetAsAtkTextNode();
        if (nameNode is null) { return; }

        string nameText = nameNode->NodeText.ToString();
        if (!nameText.Contains(Plugin.GetOwnName(), StringComparison.OrdinalIgnoreCase)) { return; }

        nameNode->SetText(config.GetFakeName());
    }

    private void UpdateCharacterProfilePanel(AtkUnitBase* profile)
    {
        if (!profile->IsVisible) { return; }

        AtkTextNode* profileNameNode = profile->GetNodeById(6)->GetAsAtkTextNode();

        if (profileNameNode is null) { return; }

        string nameText = profileNameNode->NodeText.ToString();
        if (!nameText.Contains(Plugin.GetOwnName(), StringComparison.OrdinalIgnoreCase)) { return; }

        profileNameNode->SetText(config.GetFakeName());
    }
}