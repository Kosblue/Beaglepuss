using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss.Addon;

/// <summary>
/// Battle talk (dialogue box at the top center of the screen)
/// can sometimes contain the player's name. Replace it.
/// </summary>
/// <example>
/// Neo Exdeath in Deltascape V4.0 (Savage):
/// Nero: Watch out, [Player]! He's shaking the very foundations of creation!
/// </example>
/// <param name="pluginData"></param>
public sealed class BattleTalkAddon(PluginData pluginData)
    : AddonHandlerBase(pluginData, AddonEvent.PostRequestedUpdate, "_BattleTalk")
{
    protected override unsafe void OnUpdate(AtkUnitBase* addon)
    {
        if (!PluginData.Config.ReplaceBattleTalk) { return; }

        AtkTextNode* textNode = addon->GetNodeById(6)->GetAsAtkTextNode();
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

        // Most of the time, only the first name is used in the battle talk.
        string[] nameSplit = ownName.Split(' ');
        if (nameSplit.Length is 2)
        {
            if (textNodeText.Matches(nameSplit[0]))
            {
                textNodeText = textNodeText.Replace(nameSplit[0], PluginData.Config.FakeFirstName);
            }
        }

        if (textNodeText.Matches(ownName))
        {
            textNodeText = textNodeText.Replace(ownName, PluginData.Config.GetFakeName());
        }

        textNode->SetText(textNodeText);
    }
}