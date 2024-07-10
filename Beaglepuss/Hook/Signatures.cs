namespace Beaglepuss;

public static class Signatures
{
    // from: https://github.com/Caraxi/Honorific/blob/master/Plugin.cs
    public const string UpdateNamePlateNpc =
        "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 4C 89 44 24 ?? 57 41 54 41 55 41 56 41 57 48 83 EC 20 48 8B 74 24 ??";

    // from: https://github.com/Caraxi/Honorific/blob/master/Plugin.cs
    public const string UpdateNamePlate = "40 56 57 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B 84 24";
}
