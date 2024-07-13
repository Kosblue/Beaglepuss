using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

/// <summary>
/// Replace your name and FC when checking the search info of yourself.
/// </summary>
/// <param name="data"></param>
public sealed unsafe class SearchInfoAddon(PluginData data)
    : AddonHandlerBase(data, AddonEvent.PostRequestedUpdate, "SocialDetailB")
{
    protected override void OnUpdate(AtkUnitBase* addon)
    {
        AtkTextNode* nameNode = addon->GetNodeById(3)->GetAsAtkTextNode();

        if (nameNode is null) { return; }

        string nameText = nameNode->NodeText.ToString();

        string ownName = Plugin.GetOwnName();

        if (nameText.Matches(ownName))
        {
            if (PluginData.StringManager.TryGetString(StringIdentifier.FakeName, out byte* fakeNamePtr))
            {
                nameNode->SetText(fakeNamePtr);
            }
            else
            {
                Services.Log.Warning("Unable to get fake name pointer. Hiding the text node instead.");
                nameNode->ToggleVisibility(false);
            }
        }

        if (nameText.Matches(ownName) || nameText.Matches(PluginData.Config.GetFakeName()))
        {
            AtkTextNode* fcNode = addon->GetNodeById(10)->GetAsAtkTextNode();
            AtkTextNode* fcTagNode = addon->GetNodeById(11)->GetAsAtkTextNode();
            AtkTextNode* searchComment = addon->GetNodeById(99)->GetAsAtkTextNode();

            if (fcNode is null || fcTagNode is null || searchComment is null) { return; }

            string originalFcText = fcNode->NodeText.ToString();
            string fakeFcName = PluginData.Config.FakeFcName;

            Utils.TrySetText(fcNode, StringIdentifier.FakeFreeCompany, PluginData);
            Utils.TrySetText(fcTagNode, StringIdentifier.PaddedFakeFcTag, PluginData);
            Utils.TrySetText(searchComment, StringIdentifier.FakeSearchComment, PluginData);

            if (originalFcText.Matches(fakeFcName)) { return; }

            ushort outWidth;
            fixed (byte* replacedTextPtr = Utils.ToNullTerminatedAsciiBytes(fakeFcName))
            {
                ushort outHeight; // unused
                fcNode->GetTextDrawSize(&outWidth, &outHeight, replacedTextPtr);
            }
            fcTagNode->SetXFloat(fcNode->GetXFloat() + outWidth);
        }
    }
}