using System;
using System.IO;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace IcarusWikiExtension;

internal sealed partial class IcarusWikiSettings : JsonSettingsManager, IDisposable
{
    internal static readonly IcarusWikiSettings Instance = new();

    private readonly ChoiceSetSetting _service = new("service", [
        new ChoiceSetSetting.Choice("wiki.gg", "https://icarus.wiki.gg/api.php"),
        new ChoiceSetSetting.Choice("Fandom", "https://icarus.fandom.com/api.php")
    ])
    {
        Label = Resources.wiki_service,
        Description = Resources.wiki_service_subtitle,
        IsRequired = true
    };

    public string ApiUrl => _service.Value!;

    private IcarusWikiSettings()
    {
        ExtensionHost.LogMessage("Loading settings from file.");

        FilePath = Path.Combine(Utilities.BaseSettingsPath("IcarusWikiExtension"), "settings.json");
        
        Settings.Add(_service);

        LoadSettings();
        Settings.SettingsChanged += SettingsChanged;
    }

    private void SettingsChanged(object sender, Settings args)
    {
        SaveSettings();
    }

    public void Dispose()
    {
        Settings.SettingsChanged -= SettingsChanged;
    }
}