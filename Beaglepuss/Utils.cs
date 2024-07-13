using System;
using System.IO;
using System.Runtime.CompilerServices;

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
}