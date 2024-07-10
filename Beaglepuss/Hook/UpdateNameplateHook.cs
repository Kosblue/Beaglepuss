using System;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss;

public sealed unsafe class UpdateNameplateHook : IDisposable
{
    private delegate void* UpdateNameplateDelegate(RaptureAtkModule* raptureAtkModule,
                                                   RaptureAtkModule.NamePlateInfo* namePlateInfo,
                                                   NumberArrayData* numArray,
                                                   StringArrayData* stringArray,
                                                   BattleChara* battleChara,
                                                   int numArrayIndex,
                                                   int stringArrayIndex);

    [Signature(Signatures.UpdateNamePlate, DetourName = nameof(UpdateNameplateDetour))]
    private readonly Hook<UpdateNameplateDelegate> hook = null!;

    private Configuration config;
    public UpdateNameplateHook(Configuration config)
    {
        this.config = config;
        Services.Hook.InitializeFromAttributes(this);
        hook.Enable();
    }

    public void Dispose()
    {
        hook.Disable();
        hook.Dispose();
    }

    public void* UpdateNameplateDetour(RaptureAtkModule* raptureAtkModule,
                                       RaptureAtkModule.NamePlateInfo* namePlateInfo,
                                       NumberArrayData* numArray,
                                       StringArrayData* stringArray,
                                       BattleChara* battleChara,
                                       int numArrayIndex,
                                       int stringArrayIndex)
    {
        void* original = hook.Original(raptureAtkModule,
                                       namePlateInfo,
                                       numArray,
                                       stringArray,
                                       battleChara,
                                       numArrayIndex,
                                       stringArrayIndex);
        try
        {
            if (Services.ClientState.IsPvPExcludingDen) { return original; }

            GameObject* gameObject = &battleChara->Character.GameObject;

            switch (gameObject->ObjectKind)
            {
                case ObjectKind.Pc when gameObject->SubKind is 4:
                    AfterNameplateUpdate(namePlateInfo);
                    break;
                case ObjectKind.BattleNpc when gameObject->SubKind is 2:
                    AfterPetNameplateUpdate(namePlateInfo);
                    break;
            }
        }
        catch (Exception ex)
        {
            Services.Log.Error(ex, "UpdateNameplateDetour failed");
        }

        return original;
    }

    private void AfterPetNameplateUpdate(RaptureAtkModule.NamePlateInfo* namePlateInfo)
    {
        //string displayTitle = namePlateInfo->DisplayTitle.ToString();
        string title = namePlateInfo->Title.ToString();
        if (!title.Matches(Plugin.GetOwnName())) { return; }

        Services.Log.Info($"Changing pet owner name {namePlateInfo->DisplayTitle}");
        namePlateInfo->DisplayTitle.SetString(config.GetFakeOwnerName());
        namePlateInfo->IsDirty = true;
    }

    private void AfterNameplateUpdate(RaptureAtkModule.NamePlateInfo* namePlateInfo)
    {
        uint actorId = namePlateInfo->ObjectId.ObjectId;
        if (actorId is 0 or 0xE0000000) { return; } // no idea what the second magic id is
        if (Services.Objects.SearchById(actorId) is not IPlayerCharacter player) { return; }

        string name = namePlateInfo->Name.ToString();

        if (!name.Matches(Plugin.GetOwnName())) { return; }

        namePlateInfo->Name.SetString(config.GetFakeName());

        string fcName = namePlateInfo->FcName.ToString();

        string paddedFcTag = config.GetPaddedFakeFcTag();
        if (ShouldShowFcTag(player) && !fcName.Matches(paddedFcTag))
        {
            Services.Log.Info("Changing FC name");
            namePlateInfo->FcName.SetString(paddedFcTag);
        }

        namePlateInfo->IsDirty = true;
        return;

        // FC tag should be shown if the player is in their home world and not in an instance.
        // in duties the tag is hidden, and when traveling it is replaced with Traveler.
        static bool ShouldShowFcTag(IPlayerCharacter player)
            => player.CurrentWorld.Id == player.HomeWorld.Id &&
               !Services.Condition[ConditionFlag.BoundByDuty];
    }
}