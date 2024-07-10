using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class SocialPanelAddon : IDisposable
{
    private readonly Configuration config;
    public SocialPanelAddon(Configuration config)
    {
        this.config = config;
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "Social", OnSocialPanelUpdate);
    }

    public void Dispose()
    {
        Services.AddonLifecycle.UnregisterListener(OnSocialPanelUpdate);
    }

    private void OnSocialPanelUpdate(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon;
        if (!addon->IsVisible) { return; }

        IntPtr partyMemberListPtr = Services.GameGui.GetAddonByName("PartyMemberList");
        if (partyMemberListPtr == IntPtr.Zero) { return; }

        AtkUnitBase* partyMemberList = (AtkUnitBase*)partyMemberListPtr;

        AtkComponentNode* list = partyMemberList->GetNodeById(11)->GetAsAtkComponentNode();
        if (list is null) { return; }

        AtkTextNode* myNameNode = list->Component->UldManager.SearchNodeById(5)->GetAsAtkTextNode();

        if (myNameNode is null) { return; }

        string nameText = myNameNode->NodeText.ToString();
        if (!nameText.Matches(Plugin.GetOwnName())) { return; }

        myNameNode->SetText(config.GetFakeName());

        AtkTextNode* myFcNode = list->Component->UldManager.SearchNodeById(21)->GetAsAtkTextNode();

        if (myFcNode is null) { return; }
        myFcNode->SetText(config.FakeFcTag);
    }
}