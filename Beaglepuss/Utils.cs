using System;

namespace Beaglepuss;

public static class Utils
{
    public static bool Matches(this string source, string value)
        => source.Contains(value, StringComparison.OrdinalIgnoreCase);
}