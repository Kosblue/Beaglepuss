using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class SearchInfoAddon : IDisposable
{
    private readonly Configuration config;
    public SearchInfoAddon(Configuration config)
    {
        this.config = config;
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "SocialDetailB", OnSearchInfoUpdate);
    }

    public void Dispose()
    {
        Services.AddonLifecycle.UnregisterListener(OnSearchInfoUpdate);
    }

    private void OnSearchInfoUpdate(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon;
        if (!addon->IsVisible) { return; }

        AtkTextNode* nameNode = addon->GetNodeById(3)->GetAsAtkTextNode();

        if (nameNode is null) { return; }

        string nameText = nameNode->NodeText.ToString();

        string ownName = Plugin.GetOwnName();

        if (nameText.Matches(ownName))
        {
            nameNode->SetText(config.GetFakeName());
        }

        if (nameText.Matches(ownName) || nameText.Matches(config.GetFakeName()))
        {
            AtkTextNode* fcNode = addon->GetNodeById(10)->GetAsAtkTextNode();
            AtkTextNode* fcTagNode = addon->GetNodeById(11)->GetAsAtkTextNode();
            AtkTextNode* searchComment = addon->GetNodeById(99)->GetAsAtkTextNode();

            if (fcNode is null || fcTagNode is null || searchComment is null) { return; }

            fcNode->SetText(config.FakeFcName);
            fcTagNode->ToggleVisibility(false); // don't wanna position this lole
            searchComment->SetText(config.FakeSearchComment);
        }
    }
}