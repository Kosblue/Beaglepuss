using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class ExamineAddon(PluginData pluginData)
    : AddonHandlerBase(pluginData, AddonEvent.PostRefresh, "CharacterInspect")
{
    protected override void OnUpdate(AtkUnitBase* addon)
    {
        AtkTextNode* nameNode = addon->GetNodeById(6)->GetAsAtkTextNode();
        if (nameNode is null) { return; }

        string examineText = nameNode->NodeText.ToString();

        string myName = Plugin.GetOwnName();

        if (examineText.Matches(myName))
        {
            Utils.TrySetText(nameNode, StringIdentifier.FakeName, PluginData);
        }

        if (examineText.Matches(myName) || examineText.Matches(PluginData.Config.GetFakeName()))
        {
            AtkTextNode* fcNameNode = addon->GetNodeById(28)->GetAsAtkTextNode();
            AtkTextNode* noFcNode = addon->GetNodeById(26)->GetAsAtkTextNode();
            if (fcNameNode is not null && noFcNode is not null)
            {
                if (!noFcNode->IsVisible() && fcNameNode->IsVisible())
                {
                    Utils.TrySetText(fcNameNode, StringIdentifier.FakeFreeCompany, PluginData);
                }
            }

            AtkTextNode* searchCommentNode = addon->GetNodeById(4)->GetAsAtkTextNode();
            if (searchCommentNode is not null)
            {
                Utils.TrySetText(searchCommentNode, StringIdentifier.FakeSearchComment, PluginData);
                searchCommentNode->ToggleVisibility(false);
            }
        }
    }
}