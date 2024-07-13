using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class SocialPanelAddon(PluginData pluginData)
    : AddonHandlerBase(pluginData, AddonEvent.PostUpdate, "Social")
{
    protected override void OnUpdate(AtkUnitBase* addon)
    {
        IntPtr partyMemberListPtr = Services.GameGui.GetAddonByName("PartyMemberList");
        if (partyMemberListPtr == IntPtr.Zero) { return; }

        AtkUnitBase* partyMemberList = (AtkUnitBase*)partyMemberListPtr;

        AtkComponentNode* list = partyMemberList->GetNodeById(11)->GetAsAtkComponentNode();
        if (list is null) { return; }

        AtkTextNode* myNameNode = list->Component->UldManager.SearchNodeById(5)->GetAsAtkTextNode();

        if (myNameNode is null) { return; }

        string nameText = myNameNode->NodeText.ToString();
        if (!nameText.Matches(Plugin.GetOwnName())) { return; }

        Utils.TrySetText(myNameNode, StringIdentifier.FakeName, PluginData);

        AtkTextNode* myFcNode = list->Component->UldManager.SearchNodeById(21)->GetAsAtkTextNode();

        if (myFcNode is null) { return; }
        Utils.TrySetText(myFcNode, StringIdentifier.FakeFcTag, PluginData);
    }
}