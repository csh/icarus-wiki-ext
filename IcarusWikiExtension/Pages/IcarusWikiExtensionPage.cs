using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace IcarusWikiExtension;

internal sealed partial class IcarusWikiExtensionPage : DynamicListPage, IDisposable
{
    private readonly Lazy<WikiClient> _lazyClient = new(() => new WikiClient
    {
        ClientUserAgent = "WikiSearch-CmdPal/1.0"
    });

    private static readonly CommandItem NoResults = new()
    {
        Icon = Icons.IcarusIcon,
        Title = Resources.no_results,
        Subtitle = Resources.no_results_subtitle,
    };

    private static readonly CommandItem NoQuery = new()
    {
        Icon = Icons.IcarusIcon,
        Title = Resources.icarus_wiki_search,
        Subtitle = Resources.icarus_wiki_search_subtitle,
    };

    private WikiClient Client => _lazyClient.Value;

    private readonly SemaphoreSlim _searchSemaphore = new(1, 1);
    private readonly Lock _exceptionLock = new();
    private readonly Lock _itemsLock = new();
    private readonly WikiSite _site;

    private volatile List<IListItem> _items = [];
    private CancellationTokenSource? _currentSearchCts;
    private bool _disposed;

    private Exception? _lastException;

    public IcarusWikiExtensionPage()
    {
        _site = new WikiSite(Client, new SiteOptions("https://icarus.fandom.com/api.php"));

        ItemsChanged += OnItemsChanged;
        RaiseItemsChanged(0);
    }

    private void OnItemsChanged(object sender, IItemsChangedEventArgs args)
    {
        switch (args.TotalItems)
        {
            case -1:
                string errorMessage;
                lock (_exceptionLock)
                {
                    errorMessage = string.IsNullOrEmpty(_lastException?.Message)
                        ? Resources.error_unknown
                        : _lastException.Message;
                    _lastException = null;
                }
                
                EmptyContent = new ListItem(new CommandItem()
                {
                    Icon = Icons.IcarusIcon,
                    Title = Resources.error,
                    Subtitle = errorMessage,
                });
                break;
            case 0:
                EmptyContent = string.IsNullOrEmpty(SearchText) ? new ListItem(NoQuery) : new ListItem(NoResults);
                break;
        }
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        if (_disposed) return;
        EmptyContent = null;
        _ = UpdateSearchResultsAsync(newSearch);
    }

    private async Task UpdateSearchResultsAsync(string query)
    {
        if (_disposed) return;

        EmptyContent = null;

        // Cancel and dispose previous search
        var previousCts = Interlocked.Exchange(ref _currentSearchCts, new CancellationTokenSource());
        try
        {
            // ReSharper disable once MethodHasAsyncOverload
            previousCts?.Cancel();
        }
        finally
        {
            previousCts?.Dispose();
        }

        var cancellationToken = _currentSearchCts?.Token ?? CancellationToken.None;

        if (string.IsNullOrEmpty(query))
        {
            await ClearSearchResultsAsync(cancellationToken);
            return;
        }

        IsLoading = true;

        try
        {
            await PerformSearchAsync(query, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested - do nothing
        }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            Debug.WriteLine($"Search error: {ex.Message}");

            if (!cancellationToken.IsCancellationRequested && !_disposed)
            {
                IsLoading = false;
                lock (_exceptionLock)
                {
                    _lastException = ex;
                }
                RaiseItemsChanged();
            }
        }
    }

    private async Task ClearSearchResultsAsync(CancellationToken cancellationToken)
    {
        await _searchSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (cancellationToken.IsCancellationRequested || _disposed) return;

            EmptyContent = null;
            lock (_itemsLock)
            {
                _items = [];
            }

            IsLoading = false;
            RaiseItemsChanged(0);
        }
        finally
        {
            if (!_disposed)
            {
                _searchSemaphore.Release();
            }
        }
    }

    private async Task PerformSearchAsync(string query, CancellationToken cancellationToken)
    {
        await _searchSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (cancellationToken.IsCancellationRequested || _disposed) return;

            // Ensure site is initialized
            if (!_site.Initialization.IsCompleted)
            {
                await _site.Initialization.WaitAsync(cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested || _disposed) return;

            var results = await _site.OpenSearchAsync(query, 20, OpenSearchOptions.None, cancellationToken);

            if (cancellationToken.IsCancellationRequested || _disposed) return;

            var listItems = results.Select(IListItem (result) => new ListItem(new OpenUrlCommand(result.Url!)
            {
                Result = CommandResult.Hide()
            })
            {
                Title = result.Title
            });

            listItems = ListHelpers.FilterList(listItems, query);

            if (cancellationToken.IsCancellationRequested || _disposed) return;

            var newItems = listItems.ToList();

            lock (_itemsLock)
            {
                _items = newItems;
            }

            if (!_disposed)
            {
                IsLoading = false;
                RaiseItemsChanged(newItems.Count);
            }
        }
        finally
        {
            if (!_disposed)
            {
                _searchSemaphore.Release();
            }
        }
    }

    public override IListItem[] GetItems()
    {
        if (_disposed) return [];

        lock (_itemsLock)
        {
            return _items.ToArray();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        SetSearchNoUpdate(string.Empty);
        ItemsChanged -= OnItemsChanged;
        lock (_exceptionLock)
        {
            _lastException = null;
        }

        // Cancel current operation
        var cts = Interlocked.Exchange(ref _currentSearchCts, null);
        try
        {
            cts?.Cancel();
        }
        finally
        {
            cts?.Dispose();
        }

        _searchSemaphore.Dispose();
        
        if (_lazyClient.IsValueCreated)
        {
            _lazyClient.Value.Dispose();
        }
    }
}