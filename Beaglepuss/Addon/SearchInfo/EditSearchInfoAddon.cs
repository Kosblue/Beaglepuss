using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class EditSearchInfoAddon(PluginData pluginData)
    : AddonHandlerBase(pluginData, AddonEvent.PostRequestedUpdate, "SocialDetailA")
{
    private readonly uint[] roleCategories =
    [
        30, // Tank
        39, // Healer
        48, // Melee DPS
        58, // Physical Ranged DPS
        67, // Magical Ranged DPS
        76, // Crafter
        85  // Gatherer
    ];

    protected override void OnUpdate(AtkUnitBase* addon)
    {
        SearchInfoAddon.ReplaceSearchInfoNames(PluginData,
                                               addon,
                                               roleCategories,
                                               nameNodeId: 3,
                                               fcNodeId: 6,
                                               fcTagNodeId: 7,
                                               searchCommentNodeId: uint.MaxValue);

        AtkResNode* searchCommentBox = addon->GetNodeById(91);
        if (searchCommentBox is not null)
        {
            searchCommentBox->ToggleVisibility(false);
        }
    }
}