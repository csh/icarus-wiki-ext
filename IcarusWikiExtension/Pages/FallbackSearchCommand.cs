using System;
using System.Globalization;
using System.Text;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace IcarusWikiExtension;

public sealed partial class FallbackSearchCommand : FallbackCommandItem, IDisposable
{
    private static readonly CompositeFormat QuickSearchFmt = CompositeFormat.Parse(Resources.quick_search_fmt);

    private readonly IcarusWikiExtensionPage _searchCommand;
    
    public FallbackSearchCommand() : base(new IcarusWikiExtensionPage(), Resources.quick_search)
    {
        Icon = Icons.IcarusIcon;
        Title = string.Empty;
        
        _searchCommand = (IcarusWikiExtensionPage) Command!;
    }

    public override void UpdateQuery(string query)
    {
        var isEmpty = string.IsNullOrEmpty(query);
        Title = isEmpty ? string.Empty : string.Format(CultureInfo.CurrentCulture, QuickSearchFmt, query);
        if (!isEmpty) _searchCommand.SearchText = query;
    }

    public void Dispose()
    {
        _searchCommand?.Dispose();
    }
}