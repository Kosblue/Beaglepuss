using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class CharacterPanelAddon(PluginData pluginData)
    : AddonHandlerBase(pluginData, AddonEvent.PostUpdate, "Character")
{

    protected override void OnUpdate(AtkUnitBase* addon)
    {
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

        Utils.TrySetText(nameNode, StringIdentifier.FakeName, PluginData);
    }

    private void UpdateCharacterProfilePanel(AtkUnitBase* profile)
    {
        if (!profile->IsVisible) { return; }

        AtkTextNode* profileNameNode = profile->GetNodeById(6)->GetAsAtkTextNode();

        if (profileNameNode is null) { return; }

        string nameText = profileNameNode->NodeText.ToString();
        if (!nameText.Contains(Plugin.GetOwnName(), StringComparison.OrdinalIgnoreCase)) { return; }

        Utils.TrySetText(profileNameNode, StringIdentifier.FakeName, PluginData);
    }
}