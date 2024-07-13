using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public class EditSearchInfoAddon(PluginData pluginData)
    : AddonHandlerBase(pluginData, AddonEvent.PostRequestedUpdate, "SocialDetailA")
{
    private readonly uint[] NodesToHide =
    [
        29, // job list
        91 // search comment
    ];

    protected override unsafe void OnUpdate(AtkUnitBase* addon)
    {
        AtkUldManager uldManager = addon->UldManager;

        AtkResNode* jobList = addon->GetNodeById(29);
        if (jobList is not null)
        {
            jobList->ToggleVisibility(false);
        }

        for (int i = 4; i < uldManager.NodeListCount; i++)
        {
            AtkResNode* node = uldManager.NodeList[i];

            if (NodesToHide.Contains(node->NodeId))
            {
                node->ToggleVisibility(false);
            }
        }
    }
}