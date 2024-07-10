using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class PartyBannerAddon : IDisposable
{
    private readonly Configuration config;
    public PartyBannerAddon(Configuration config)
    {
        this.config = config;
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "BannerParty", OnPartyBannerUpdate);
    }

    public void Dispose()
    {
        Services.AddonLifecycle.UnregisterListener(OnPartyBannerUpdate);
    }

    private static readonly uint[] AdvPlateIndices = [4, 6, 8, 10, 12, 14, 16, 18];

    private void OnPartyBannerUpdate(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon;
        if (!addon->IsVisible) { return; }

        foreach (uint index in AdvPlateIndices)
        {
            AtkComponentNode* partyBannerNode = addon->GetNodeById(index)->GetAsAtkComponentNode();
            if (partyBannerNode is null || !partyBannerNode->IsVisible())
            {
                continue;
            }

            AtkUldManager children = partyBannerNode->Component->UldManager;

            AtkTextNode* fullName = children.SearchNodeById(5)->GetAsAtkTextNode();
            AtkTextNode* firstName = children.SearchNodeById(6)->GetAsAtkTextNode();
            AtkTextNode* lastName = children.SearchNodeById(7)->GetAsAtkTextNode();

            if (fullName is null || firstName is null || lastName is null)
            {
                continue;
            }

            string nameText = fullName->NodeText.ToString();

            string ownName = Plugin.GetOwnName();

            if (nameText.Matches(ownName))
            {
                fullName->SetText(config.GetFakeName());
            }

            if (nameText.Matches(ownName) || nameText.Matches(config.GetFakeName()))
            {
                firstName->SetText(config.FakeFirstName);
                lastName->SetText(config.FakeLastName);
            }
        }
    }
}