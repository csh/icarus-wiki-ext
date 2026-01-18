using System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace IcarusWikiExtension;

public sealed partial class IcarusWikiExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;
    private readonly IcarusWikiExtensionPage? _searchPage;
    private readonly FallbackSearchCommand _fallbackSearchCommand;

    public IcarusWikiExtensionCommandsProvider()
    {
        DisplayName = Resources.icarus_ext_display_name;
        Id = "IcarusWiki";
        Icon = Icons.IcarusIcon;
        Settings = IcarusWikiSettings.Instance.Settings;
        
        _fallbackSearchCommand = new FallbackSearchCommand();
        _commands =
        [
            new ListItem(_searchPage = new IcarusWikiExtensionPage())
            {
                Icon = this.Icon,
                Title = Resources.icarus_wiki_search,
                Subtitle = Resources.icarus_wiki_search_subtitle,
                MoreCommands = [new CommandContextItem(IcarusWikiSettings.Instance.Settings.SettingsPage)]
            },
            new ListItem(new OpenUrlCommand("https://icarusintel.com/")
            {
                Icon = this.Icon,
                Name = Resources.icarus_intel,
                Result = CommandResult.Dismiss(),
            })
            {
                Subtitle = Resources.icarus_intel_subtitle,
            },
            new ListItem(new OpenUrlCommand("https://mapgenie.io/icarus")
            {
                Icon = this.Icon,
                Name = Resources.mapgenie,
                Result = CommandResult.Dismiss()
            })
            {
                Subtitle = Resources.mapgenie_subtitle,
            },
        ];
    }

    public override ICommandItem[] TopLevelCommands() => _commands;

    public override IFallbackCommandItem[]? FallbackCommands()
    {
        return
        [
            _fallbackSearchCommand
        ];
    }

    public override void Dispose()
    {
        IcarusWikiSettings.Instance.Dispose();
        _fallbackSearchCommand?.Dispose();
        _searchPage?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}