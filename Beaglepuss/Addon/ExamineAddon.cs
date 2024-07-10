using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

public sealed unsafe class ExamineAddon : IDisposable
{
    private readonly Configuration config;
    public ExamineAddon(Configuration config)
    {
        this.config = config;
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "CharacterInspect", OnExamineUpdate);
    }

    public void Dispose() { Services.AddonLifecycle.UnregisterListener(OnExamineUpdate); }

    private void OnExamineUpdate(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon;
        if (!addon->IsVisible) { return; }

        AtkTextNode* nameNode = addon->GetNodeById(6)->GetAsAtkTextNode();
        if (nameNode is null) { return; }

        string examineText = nameNode->NodeText.ToString();

        string myName = Plugin.GetOwnName();

        if (examineText.Matches(myName))
        {
            nameNode->SetText(config.GetFakeName());
        }

        if (examineText.Matches(myName) || examineText.Matches(config.GetFakeName()))
        {
            AtkTextNode* fcNameNode = addon->GetNodeById(28)->GetAsAtkTextNode();
            AtkTextNode* noFcNode = addon->GetNodeById(26)->GetAsAtkTextNode();
            if (fcNameNode is not null && noFcNode is not null)
            {
                if (!noFcNode->IsVisible() &&
                    fcNameNode->IsVisible())
                {
                    fcNameNode->SetText(config.FakeFcName);
                }
            }

            AtkTextNode* searchCommentNode = addon->GetNodeById(4)->GetAsAtkTextNode();
            if (searchCommentNode is not null && searchCommentNode->IsVisible())
            {
                searchCommentNode->SetText(config.FakeSearchComment);
            }
        }
    }
}