using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class AdventurePlateAddons(PluginData pluginData)
    : AddonHandlerBase(pluginData, AddonEvent.PostRequestedUpdate, "CharaCard")
{

    protected override void OnUpdate(AtkUnitBase* addon)
    {
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
        Utils.TrySetText(nameTextNode, StringIdentifier.FakeName, PluginData);
        return true;
    }

    private void TryReplaceFc(AtkUnitBase* addon)
    {
        AtkComponentNode* fcContainer = addon->GetNodeById(8)->GetAsAtkComponentNode();

        if (fcContainer is null) { return; }
        AtkTextNode* fcName = fcContainer->Component->UldManager.SearchNodeById(2)->GetAsAtkTextNode();

        if (fcName is null) { return; }

        Utils.TrySetText(fcName, StringIdentifier.FakeFreeCompany, PluginData);
    }

    private void TryReplaceSearchComment(AtkUnitBase* addon)
    {
        AtkComponentNode* searchCommentContainer = addon->GetNodeById(11)->GetAsAtkComponentNode();

        if (searchCommentContainer is null) { return; }

        AtkTextNode* searchComment = searchCommentContainer->Component->UldManager.SearchNodeById(3)->GetAsAtkTextNode();
        if (searchComment is null) { return; }

        Utils.TrySetText(searchComment, StringIdentifier.FakeSearchComment, PluginData);
    }
}