using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

/// <summary>
/// Replaces character names in the character list menu when logging in.
/// </summary>
public sealed class CharacterListMenuAddon(PluginData data)
    : AddonHandlerBase(data, AddonEvent.PostRequestedUpdate, "_CharaSelectListMenu")
{
    protected override unsafe void OnUpdate(AtkUnitBase* addon)
    {
        AtkComponentNode* listNode = addon->GetNodeById(13)->GetAsAtkComponentNode();
        if (listNode is null)
        {
            Utils.LogNull(nameof(listNode));
            return;
        }

        AtkUldManager parentUldManager = listNode->Component->UldManager;
        AtkResNode** list = parentUldManager.NodeList;

        if (list is null)
        {
            Utils.LogNull(nameof(list));
            return;
        }

        // start at 2 -> skip collision node and scrollbar
        for (var i = 2; i < parentUldManager.NodeListCount; i++)
        {
            AtkComponentNode* node = list[i]->GetAsAtkComponentNode();
            if (node is null)
            {
                Utils.LogNull(nameof(node));
                continue;
            }

            AtkUldManager uldManager = node->Component->UldManager;

            AtkTextNode* nameNode = uldManager.SearchNodeById(6)->GetAsAtkTextNode();
            if (nameNode is null)
            {
                Utils.LogNull(nameof(nameNode));
                continue;
            }

            nameNode->SetText(PluginData.Config.GetFakeName());
        }
    }
}