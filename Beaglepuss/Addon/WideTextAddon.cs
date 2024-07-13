using System;
using System.Runtime.InteropServices;
using System.Text;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

/// <summary>
/// Replaces the text of the toast notification that might contain the player's name.
/// </summary>
public sealed class WideTextAddon(PluginData data)
    : AddonHandlerBase(data, AddonEvent.PostRequestedUpdate, "_WideText")
{
    protected override unsafe void OnUpdate(AtkUnitBase* addon)
    {
        AtkTextNode* textNode = addon->GetNodeById(3)->GetAsAtkTextNode();
        if (textNode is null)
        {
            Utils.LogNull(nameof(textNode));
            return;
        }

        string textNodeText = textNode->NodeText.ToString();

        string ownName = Plugin.GetOwnName();
        if (string.IsNullOrWhiteSpace(ownName))
        {
            Utils.LogNull(nameof(ownName));
            return;
        }

        if (!textNodeText.Contains(ownName)) { return; }

        string replacedText = textNodeText.Replace(ownName, PluginData.Config.GetFakeName());

        textNode->SetText(replacedText);

        IntPtr replacedTextPtr = Marshal.StringToHGlobalAnsi(replacedText);
        ushort outWidth,
               outHeight;
        textNode->GetTextDrawSize(&outWidth, &outHeight, (byte*)replacedTextPtr);
        Marshal.FreeHGlobal(replacedTextPtr);

        AtkNineGridNode* shadowNode = addon->GetNodeById(4)->GetAsAtkNineGridNode();

        if (shadowNode is null)
        {
            Utils.LogNull(nameof(shadowNode));
            return;
        }

        ushort oldWidth = shadowNode->GetWidth();
        ushort newWidth = (ushort)(outWidth + 30); // 30 seems like a good padding? Not sure
        shadowNode->SetWidth(newWidth);

        // re-center
        float widthDiff = newWidth - oldWidth;
        shadowNode->SetXFloat(shadowNode->GetXFloat() - (widthDiff / 2f));
    }
}