using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Beaglepuss.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration Configuration;
    private readonly StringManager<StringIdentifier> StringManager;

    public ConfigWindow(Plugin plugin) : base("Beaglepuss###Kosblue1")
    {
        Size = new Vector2(400, 300);
        SizeCondition = ImGuiCond.Once;

        Configuration = plugin.Configuration;
        StringManager = plugin.StringManager;
    }

    public void Dispose() { }

    public override void Draw()
    {
        string fakeFirstName = Configuration.FakeFirstName;
        if (ImGui.InputText("Fake First Name###Kosblue2", ref fakeFirstName, 15))
        {
            Configuration.FakeFirstName = fakeFirstName;
            StringManager.SetString(StringIdentifier.FakeName, Configuration.GetFakeName());
            StringManager.SetString(StringIdentifier.FakeFirstName, Configuration.FakeFirstName);
            Configuration.Save();
        }

        string fakeLastName = Configuration.FakeLastName;
        if (ImGui.InputText("Fake Last Name###Kosblue3", ref fakeLastName, 15))
        {
            Configuration.FakeLastName = fakeLastName;
            StringManager.SetString(StringIdentifier.FakeName, Configuration.GetFakeName());
            StringManager.SetString(StringIdentifier.FakeLastName, Configuration.FakeLastName);
            Configuration.Save();
        }

        string fakeFcName = Configuration.FakeFcName;
        if (ImGui.InputText("Fake FC Name###Kosblue4", ref fakeFcName, 20))
        {
            Configuration.FakeFcName = fakeFcName;
            StringManager.SetString(StringIdentifier.FakeFreeCompany, Configuration.FakeFcName);
            Configuration.Save();
        }

        string fakeFcTag = Configuration.FakeFcTag;
        if (ImGui.InputText("Fake FC Tag###Kosblue5", ref fakeFcTag, 5))
        {
            Configuration.FakeFcTag = fakeFcTag;
            StringManager.SetString(StringIdentifier.FakeFcTag, Configuration.FakeFcTag);
            StringManager.SetString(StringIdentifier.PaddedFakeFcTag, Configuration.GetPaddedFakeFcTag());
            Configuration.Save();
        }

        string fakeSearchComment = Configuration.FakeSearchComment;
        if (ImGui.InputText("Fake Search Comment###Kosblue6", ref fakeSearchComment, 60))
        {
            Configuration.FakeSearchComment = fakeSearchComment;
            StringManager.SetString(StringIdentifier.FakeSearchComment, fakeSearchComment);
            Configuration.Save();
        }

        bool replaceBattleTalk = Configuration.ReplaceBattleTalk;
        if (ImGui.Checkbox("Replace Battle Talk###Kosblue7", ref replaceBattleTalk))
        {
            Configuration.ReplaceBattleTalk = replaceBattleTalk;
            Configuration.Save();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("""
                       Allow replacing battle talk (dialogue box at the top center of the screen) text with fake names.
                       The reason you would want to disable this is if your character's first name is a common word that gets replaced often mistakenly.
                       """);
            ImGui.EndTooltip();
        }

        bool hideJobsFromSearchInfo = Configuration.ShowAllJobsAsMaxLevel;
        if (ImGui.Checkbox("Hide Jobs from Search Info###Kosblue8", ref hideJobsFromSearchInfo))
        {
            Configuration.ShowAllJobsAsMaxLevel = hideJobsFromSearchInfo;
            Configuration.Save();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("Makes all jobs in search info and character menu appear as level 100.");
            ImGui.EndTooltip();
        }
    }
}
