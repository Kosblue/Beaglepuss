using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class TargetAddons : IDisposable
{
    private readonly Configuration config;
    public TargetAddons(Configuration config)
    {
        this.config = config;
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "_TargetInfo", OnTargetInfoUpdate);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "_TargetInfoMainTarget", TargetInfoMainTargetUpdate);
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "_FocusTargetInfo", FocusTargetInfoUpdate);
    }

    public void Dispose()
    {
        Services.AddonLifecycle.UnregisterListener(OnTargetInfoUpdate);
        Services.AddonLifecycle.UnregisterListener(TargetInfoMainTargetUpdate);
        Services.AddonLifecycle.UnregisterListener(FocusTargetInfoUpdate);
    }

    private void FocusTargetInfoUpdate(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon;
        if (addon->IsVisible)
        {
            RefreshTargetInfo(addon, 10, Services.Targets.FocusTarget);
        }
    }

    private void TargetInfoMainTargetUpdate(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon;
        if (!addon->IsVisible) { return; }

        RefreshTargetInfo(addon, 10, Services.Targets.Target);
        TryUpdateTargetOfTarget(addon);
    }

    private void OnTargetInfoUpdate(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon;
        if (!addon->IsVisible) { return; }

        RefreshTargetInfo(addon, 16, Services.Targets.Target);

        TryUpdateTargetOfTarget(addon);
    }

    private void TryUpdateTargetOfTarget(AtkUnitBase* addon)
    {
        AtkResNode* targetOfTargetNode = addon->GetNodeById(3);
        if (targetOfTargetNode is not null && targetOfTargetNode->IsVisible())
        {
            RefreshTargetInfo(addon, 7, Services.Targets.Target?.TargetObject ?? Services.ClientState.LocalPlayer);
        }
    }

    private void RefreshTargetInfo(AtkUnitBase* addon, uint id, IGameObject? target)
    {
        if (target is not IPlayerCharacter targetChar)
        {
            return;
        }

        AtkTextNode* textNode = addon->GetTextNodeById(id);
        string text = textNode->NodeText.ToString();

        string oriFcName = $"«{targetChar.CompanyTag.TextValue}»";

        string ownName = Plugin.GetOwnName();
        if (text.Contains(ownName, StringComparison.OrdinalIgnoreCase))
        {
            text = text.Replace(ownName, config.GetFakeName());
            if (text.Contains(oriFcName, StringComparison.OrdinalIgnoreCase))
            {
                text = text.Replace(oriFcName, config.GetFormattedFakeFcTag());
            }
        }

        textNode->NodeText.SetString(text);
    }
}
