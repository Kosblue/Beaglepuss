using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class AdventurePlateAddons : IDisposable
{
    private readonly Configuration config;
    public AdventurePlateAddons(Configuration config)
    {
        this.config = config;
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "CharaCard", OnCharaCardUpdated);
    }

    public void Dispose() { Services.AddonLifecycle.UnregisterListener(OnCharaCardUpdated); }

    private void OnCharaCardUpdated(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon;
        if (!addon->IsVisible) { return; }

        if (TryReplaceName(addon))
        {
            TryReplaceFc(addon);
            TryReplaceSearchComment(addon);
        }
    }

    private bool TryReplaceName(AtkUnitBase* addon)
    {
        AtkComponentNode* nameTitleContainer = addon->GetNodeById(4)->GetAsAtkComponentNode();

        if (nameTitleContainer is null)
        {
            Services.Log.Error("Name title container not found");
            return false;
        }

        AtkResNode* nameContainer = nameTitleContainer->Component->UldManager.SearchNodeById(4);

        if (nameContainer is null)
        {
            Services.Log.Error("Name container not found");
            return false;
        }

        AtkTextNode* nameTextNode = nameContainer->ChildNode->GetAsAtkTextNode();
        if (nameTextNode is null)
        {
            Services.Log.Error("Name text node not found");
            return false;
        }

        string text = nameTextNode->NodeText.ToString();
        if (!text.Matches(Plugin.GetOwnName())) { return false; }

        Services.Log.Debug("Replacing name on Adventure Plate");
        nameTextNode->SetText(config.GetFakeName());
        return true;
    }

    private void TryReplaceFc(AtkUnitBase* addon)
    {
        AtkComponentNode* fcContainer = addon->GetNodeById(8)->GetAsAtkComponentNode();

        if (fcContainer is null) { return; }
        AtkTextNode* fcName = fcContainer->Component->UldManager.SearchNodeById(2)->GetAsAtkTextNode();

        if (fcName is null) { return; }

        fcName->SetText(config.FakeFcName);
    }

    private void TryReplaceSearchComment(AtkUnitBase* addon)
    {
        AtkComponentNode* searchCommentContainer = addon->GetNodeById(11)->GetAsAtkComponentNode();

        if (searchCommentContainer is null) { return; }

        AtkTextNode* searchComment = searchCommentContainer->Component->UldManager.SearchNodeById(3)->GetAsAtkTextNode();
        if (searchComment is null) { return; }

        searchComment->SetText(config.FakeSearchComment);
    }
}