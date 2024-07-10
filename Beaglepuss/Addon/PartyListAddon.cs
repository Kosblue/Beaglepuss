using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class PartyListAddon : IDisposable
{
    private DateTime nextUpdate = DateTime.MinValue;
    private uint? lastJobId;

    private readonly Configuration config;
    public PartyListAddon(Configuration config)
    {
        this.config = config;
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "_PartyList", OnPartyListUpdate);
    }

    public void Dispose()
    {
        Services.AddonLifecycle.UnregisterListener(OnPartyListUpdate);
    }

    private void OnPartyListUpdate(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon;
        if (!addon->IsVisible) { return; }

        uint? job = Services.ClientState.LocalPlayer?.ClassJob.Id;

        if (job == lastJobId && DateTime.Now < nextUpdate) { return; }

        nextUpdate = DateTime.Now.AddSeconds(1);
        lastJobId = job;

        IntPtr partListPtr = Services.GameGui.GetAddonByName("_PartyList");
        if (partListPtr == IntPtr.Zero) { return; }

        AddonPartyList* partyList = (AddonPartyList*)partListPtr;

        var members = partyList->PartyMembers;
        foreach (AddonPartyList.PartyListMemberStruct member in members)
        {
            string nameText = member.Name->NodeText.ToString();
            if (string.IsNullOrWhiteSpace(nameText)) { continue; }
            if (nameText.Contains(Plugin.GetOwnName(), StringComparison.OrdinalIgnoreCase))
            {
                // Needs to be replaced since the level indicator is part of the text
                nameText = nameText.Replace(Plugin.GetOwnName(), config.GetFakeName());
                member.Name->SetText(nameText);
            }
        }
    }
}