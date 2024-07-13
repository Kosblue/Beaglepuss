using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public abstract unsafe class AddonHandlerBase : IDisposable
{
    //public delegate void AddonEventHandler(AtkUnitBase* addon);
    protected readonly PluginData PluginData;

    protected AddonHandlerBase(PluginData pluginData, AddonEvent targetEvent, string addonName)
    {
        PluginData = pluginData;
        Services.AddonLifecycle.RegisterListener(targetEvent, addonName, OnUpdateBase);
    }

    public void Dispose()
    {
        Services.AddonLifecycle.UnregisterListener(OnUpdateBase);
    }

    private void OnUpdateBase(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon;
        if (!addon->IsVisible) { return; }
        OnUpdate(addon);
    }

    protected abstract void OnUpdate(AtkUnitBase* addon);
}