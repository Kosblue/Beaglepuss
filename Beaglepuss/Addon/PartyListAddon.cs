using System;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class PartyListAddon(PluginData pluginData)
    : AddonHandlerBase(pluginData, AddonEvent.PostUpdate, "_PartyList")
{
    private DateTime nextUpdate = DateTime.MinValue;
    private uint? lastJobId;

    protected override void OnUpdate(AtkUnitBase* addon)
    {
        uint? job = Services.ClientState.LocalPlayer?.ClassJob.Id;

        if (job == lastJobId && DateTime.Now < nextUpdate) { return; }

        nextUpdate = DateTime.Now.AddSeconds(1);
        lastJobId = job;

        IntPtr partListPtr = Services.GameGui.GetAddonByName("_PartyList");
        if (partListPtr == IntPtr.Zero) { return; }

        AddonPartyList* partyList = (AddonPartyList*)partListPtr;

        var members = partyList->PartyMembers;
        foreach (var member in members)
        {
            if (member.Name is null) { continue; } // Can happen when logging in

            string nameText = member.Name->NodeText.ToString();
            if (string.IsNullOrWhiteSpace(nameText)) { continue; }
            if (nameText.Contains(Plugin.GetOwnName(), StringComparison.OrdinalIgnoreCase))
            {
                // Needs to be replaced since the level indicator is part of the text
                nameText = nameText.Replace(Plugin.GetOwnName(), PluginData.Config.GetFakeName());
                member.Name->SetText(nameText);
            }
        }
    }
}