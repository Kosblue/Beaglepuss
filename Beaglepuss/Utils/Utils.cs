using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Beaglepuss;

public static class Utils
{
    public static bool Matches(this string source, string value)
        => source.Contains(value, StringComparison.OrdinalIgnoreCase);

    public static void LogNull(string paramName, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        string fileName = Path.GetFileNameWithoutExtension(file);
        Services.Log.Error($"NullPtr \"{paramName}\" at {fileName}:{line}");
    }

    public static byte[] ToNullTerminatedAsciiBytes(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        byte[] nullTerminatedBytes = new byte[bytes.Length + 1];
        bytes.CopyTo(nullTerminatedBytes, 0);
        nullTerminatedBytes[^1] = 0;
        return bytes;
    }

    public static unsafe void TrySetText(AtkTextNode* node, StringIdentifier identifier, PluginData data)
    {
        if (data.StringManager.TryGetString(identifier, out byte* ptr))
        {
            node->SetText(ptr);
        }
        else
        {
            Services.Log.Warning($"Unable to get pointer {identifier}. Hiding the text node instead.");
            node->ToggleVisibility(false);
        }
    }
}