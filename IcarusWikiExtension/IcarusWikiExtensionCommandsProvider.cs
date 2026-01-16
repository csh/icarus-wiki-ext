using System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace IcarusWikiExtension;

public sealed partial class IcarusWikiExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;
    private readonly IcarusWikiExtensionPage? _searchPage;

    public IcarusWikiExtensionCommandsProvider()
    {
        DisplayName = Resources.icarus_ext_display_name;
        Id = "IcarusWiki";
        Icon = Icons.IcarusIcon;
        
        _commands =
        [
            new ListItem(_searchPage = new IcarusWikiExtensionPage())
            {
                Icon = this.Icon,
                Title = Resources.icarus_wiki_search,
                Subtitle = Resources.icarus_wiki_search_subtitle
            },
            new ListItem(new OpenUrlCommand("https://icarusintel.com/")
            {
                Icon = this.Icon,
                Name = Resources.icarus_intel,
                Result = CommandResult.Dismiss()
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

    public override void Dispose()
    {
        _searchPage?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}