using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

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
        byte[] bytes = Encoding.ASCII.GetBytes(str);
        byte[] nullTerminatedBytes = new byte[bytes.Length + 1];
        bytes.CopyTo(nullTerminatedBytes, 0);
        nullTerminatedBytes[^1] = 0;
        return bytes;
    }
}