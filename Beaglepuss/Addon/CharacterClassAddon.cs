using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class CharacterClassAddon(PluginData pluginData)
    : AddonHandlerBase(pluginData, AddonEvent.PostRequestedUpdate, "CharacterClass")
{
    private readonly uint[] roleCategories =
    [
        3, // Tank
        15, // Healer
        27, // Melee DPS
        43, // Physical Ranged DPS
        53, // Magical Ranged DPS
        67, // Crafter
        79  // Gatherer
    ];

    protected override void OnUpdate(AtkUnitBase* addon)
    {
        if (!PluginData.Config.AnonymiseParameters) { return; }

        foreach (uint id in roleCategories)
        {
            AtkResNode* roleNode = addon->GetNodeById(id);

            AtkResNode* currentNode = roleNode->ChildNode;

            while (currentNode is not null)
            {
                AtkComponentNode* jobNode = currentNode->ChildNode->GetAsAtkComponentNode();

                if (jobNode is not null)
                {
                    SetupJobNode(jobNode->Component, id);
                }
                else
                {
                    // crafter classes have a different structure
                    AtkComponentNode* jobNode2 = currentNode->GetAsAtkComponentNode();

                    if (jobNode2 is not null)
                    {
                        SetupJobNode(jobNode2->Component, id);
                    }
                }

                currentNode = currentNode->PrevSiblingNode;
            }
        }

        static void SetupJobNode(AtkComponentBase* component, uint id)
        {
            AtkUldManager uldManager = component->UldManager;

            uint nameId = 3;
            uint xpId = 7;
            uint currentXpId = 6;
            uint levelId = 2;
            uint jobIconId = 4;

            if (id is 67) // crafter
            {
                xpId = 9;
                currentXpId = 8;
                levelId = 5;
                jobIconId = 7;

                AtkResNode* desynthLevelNode = uldManager.SearchNodeById(10);
                desynthLevelNode->ScaleX = 1f;
            }

            AtkTextNode* nameNode = uldManager.SearchNodeById(nameId)->GetAsAtkTextNode();

            if (nameNode is null)
            {
                Utils.LogNull(nameof(nameNode));
                return;
            }

            nameNode->TextColor.R = 0xCC;
            nameNode->TextColor.G = 0xCC;
            nameNode->TextColor.B = 0xCC;
            nameNode->TextColor.A = 0xFF;

            AtkResNode* totalXpNode = uldManager.SearchNodeById(xpId);

            if (totalXpNode is null)
            {
                Utils.LogNull(nameof(totalXpNode));
                return;
            }

            totalXpNode->Color.A = 0xFF;

            AtkResNode* currentXpNode = uldManager.SearchNodeById(currentXpId);

            if (currentXpNode is null)
            {
                Utils.LogNull(nameof(currentXpNode));
                return;
            }

            currentXpNode->ToggleVisibility(false);

            AtkTextNode* levelNode = uldManager.SearchNodeById(levelId)->GetAsAtkTextNode();

            if (levelNode is null)
            {
                Utils.LogNull(nameof(levelNode));
                return;
            }

            levelNode->SetText("100");
            levelNode->TextColor.R = 0xF0;
            levelNode->TextColor.G = 0x8E;
            levelNode->TextColor.B = 0x37;
            levelNode->TextColor.A = 0xFF;

            AtkResNode* jobIconNode = uldManager.SearchNodeById(jobIconId);

            if (jobIconNode is null)
            {
                Utils.LogNull(nameof(jobIconNode));
                return;
            }

            jobIconNode->Color.A = 0xFF;
        }
    }
}