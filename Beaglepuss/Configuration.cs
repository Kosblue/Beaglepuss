using Dalamud.Configuration;
using System;

namespace Beaglepuss;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public string GetFakeName()
        => $"{FakeFirstName} {FakeLastName}";
    public string GetFormattedFakeFcTag()
        => $"«{FakeFcTag}»";
    public string GetPaddedFakeFcTag()
        => $" {GetFormattedFakeFcTag()}";
    public string GetFakeOwnerName()
        => $"《{GetFakeName()}》";

    public string FakeFirstName { get; set; } = "John";
    public string FakeLastName { get; set; } = "Doe";

    public string FakeFcName { get; set; } = "FreeCompany";
    public string FakeWorld { get; set; } = "World";
    public string FakeFcTag { get; set; } = "FC";
    public string FakeSearchComment { get; set; } = "Say hello to new me!";

    public void Save() { Services.PluginInterface.SavePluginConfig(this); }
}