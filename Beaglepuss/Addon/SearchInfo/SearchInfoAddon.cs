using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Microsoft.VisualBasic;

namespace Beaglepuss.Addon;

/// <summary>
/// Replace your name and FC when checking the search info of yourself.
/// </summary>
/// <param name="data"></param>
public sealed unsafe class SearchInfoAddon(PluginData data)
    : AddonHandlerBase(data, AddonEvent.PostRequestedUpdate, "SocialDetailB")
{
    private readonly uint[] roleCategories =
    [
        38, // Tank
        47, // Healer
        56, // Melee DPS
        66, // Physical Ranged DPS
        75, // Magical Ranged DPS
        84, // Crafter
        93  // Gatherer
    ];

    protected override void OnUpdate(AtkUnitBase* addon)
    {
        ReplaceSearchInfoNames(PluginData,
                               addon,
                               roleCategories,
                               nameNodeId: 3,
                               fcNodeId: 10,
                               fcTagNodeId: 11,
                               searchCommentNodeId: 99
        );
    }

    public static void ReplaceSearchInfoNames(PluginData pluginData,
                                              AtkUnitBase* addon,
                                              uint[] roleCategories,
                                              uint nameNodeId,
                                              uint fcNodeId,
                                              uint fcTagNodeId,
                                              uint searchCommentNodeId)
    {
        AtkTextNode* nameNode = addon->GetNodeById(nameNodeId)->GetAsAtkTextNode();

        if (nameNode is null) { return; }

        string nameText = nameNode->NodeText.ToString();

        string ownName = Plugin.GetOwnName();

        if (nameText.Matches(ownName))
        {
            if (pluginData.StringManager.TryGetString(StringIdentifier.FakeName, out byte* fakeNamePtr))
            {
                nameNode->SetText(fakeNamePtr);
            }
            else
            {
                Services.Log.Warning("Unable to get fake name pointer. Hiding the text node instead.");
                nameNode->ToggleVisibility(false);
            }
        }

        if (nameText.Matches(ownName) || nameText.Matches(pluginData.Config.GetFakeName()))
        {
            AtkTextNode* fcNode = addon->GetNodeById(fcNodeId)->GetAsAtkTextNode();
            AtkTextNode* fcTagNode = addon->GetNodeById(fcTagNodeId)->GetAsAtkTextNode();

            if (fcNode is null || fcTagNode is null) { return; }

            string originalFcText = fcNode->NodeText.ToString();
            string fakeFcName = pluginData.Config.FakeFcName;

            Utils.TrySetText(fcNode, StringIdentifier.FakeFreeCompany, pluginData);
            Utils.TrySetText(fcTagNode, StringIdentifier.PaddedFakeFcTag, pluginData);
            if (searchCommentNodeId is not uint.MaxValue)
            {
                AtkTextNode* searchComment = addon->GetNodeById(searchCommentNodeId)->GetAsAtkTextNode();
                Utils.TrySetText(searchComment, StringIdentifier.FakeSearchComment, pluginData);
            }

            if (!originalFcText.Matches(fakeFcName))
            {
                ushort outWidth;
                fixed (byte* replacedTextPtr = Utils.ToNullTerminatedAsciiBytes(fakeFcName))
                {
                    ushort outHeight; // unused
                    fcNode->GetTextDrawSize(&outWidth, &outHeight, replacedTextPtr);
                }

                fcTagNode->SetXFloat(fcNode->GetXFloat() + outWidth);
            }

            if (pluginData.Config.AnonymiseParameters)
            {
                TrySetAllJobsTo100(addon, roleCategories);
            }
        }
    }

    public static void TrySetAllJobsTo100(AtkUnitBase* addon, uint[] roleCategories)
    {
        foreach (uint id in roleCategories)
        {
            AtkResNode* jobIconsNode = addon->GetNodeById(id);

            AtkResNode* currentNode = jobIconsNode->ChildNode;
            while (currentNode is not null)
            {
                if (currentNode->IsVisible())
                {
                    AtkComponentNode* componentNode = currentNode->GetAsAtkComponentNode();

                    if (componentNode is null)
                    {
                        Utils.LogNull(nameof(componentNode));
                        break;
                    }

                    AtkTextNode* jobLevel = componentNode->Component->UldManager.SearchNodeById(3)->GetAsAtkTextNode();

                    if (jobLevel is null)
                    {
                        Utils.LogNull(nameof(jobLevel));
                        break;
                    }


                    jobLevel->SetText("100");
                }

                currentNode = currentNode->PrevSiblingNode;
            }
        }
    }
}