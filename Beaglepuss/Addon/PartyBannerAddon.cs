using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class PartyBannerAddon(PluginData data)
    : AddonHandlerBase(data, AddonEvent.PostUpdate, "BannerParty")
{
    private static readonly uint[] AdvPlateIndices = [4, 6, 8, 10, 12, 14, 16, 18];

    protected override void OnUpdate(AtkUnitBase* addon)
    {
        foreach (uint index in AdvPlateIndices)
        {
            AtkComponentNode* partyBannerNode = addon->GetNodeById(index)->GetAsAtkComponentNode();
            if (partyBannerNode is null || !partyBannerNode->IsVisible()) { continue; }

            AtkUldManager children = partyBannerNode->Component->UldManager;

            AtkTextNode* fullName = children.SearchNodeById(5)->GetAsAtkTextNode();
            AtkTextNode* firstName = children.SearchNodeById(6)->GetAsAtkTextNode();
            AtkTextNode* lastName = children.SearchNodeById(7)->GetAsAtkTextNode();

            if (fullName is null || firstName is null || lastName is null) { continue; }

            string nameText = fullName->NodeText.ToString();

            string ownName = Plugin.GetOwnName();

            if (nameText.Matches(ownName))
            {
                Utils.TrySetText(fullName, StringIdentifier.FakeName, PluginData);
            }

            if (nameText.Matches(ownName) || nameText.Matches(PluginData.Config.GetFakeName()))
            {
                Utils.TrySetText(firstName, StringIdentifier.FakeFirstName, PluginData);
                Utils.TrySetText(lastName, StringIdentifier.FakeLastName, PluginData);
            }

            // the cutoff seems to be based on length of the text but let's approximate it
            const int splitLength = 12;

            int fakeLength = PluginData.Config.FakeFirstName.Length +
                             PluginData.Config.FakeLastName.Length;

            if (fakeLength >= splitLength)
            {
                firstName->ToggleVisibility(true);
                lastName->ToggleVisibility(true);
                fullName->ToggleVisibility(false);
            }
            else
            {
                firstName->ToggleVisibility(false);
                lastName->ToggleVisibility(false);
                fullName->ToggleVisibility(true);
            }
        }
    }
}