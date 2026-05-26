# ElAutocomplete Alignment

`ElAutocomplete` supports Element Plus-style input suggestions with synchronous data, async remote lookup, debounce, keyboard selection, clear, disabled, size, and form validation integration.

## Remote Debounce

`Debounce` defaults to 300ms. New input cancels the pending debounce window and marks older async results as stale, so slower remote responses cannot reopen or replace the current suggestion list after a later query or clear.

```razor
<ElAutocomplete Value="@keyword"
                ValueChanged="value => keyword = value"
                Placeholder="Search APIs"
                Clearable="true"
                FetchSuggestionsAsync="SearchAsync"
                Debounce="500"
                OnSelect="SelectApiAsync" />

@code {
    private string keyword;

    private async Task<IEnumerable<AutocompleteOption>> SearchAsync(string query)
    {
        await Task.Delay(200);

        return apis
            .Where(x => x.Name.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            .Select(x => new AutocompleteOption
            {
                Label = x.Name,
                Value = x.Name,
                Data = x
            });
    }

    private Task SelectApiAsync(AutocompleteOption option)
    {
        selectedApi = option.Data as ApiInfo;
        return Task.CompletedTask;
    }
}
```

## Suggestion Item Template

Use `ItemTemplate` or the Element Plus-style alias `SuggestionItemTemplate` to customize the default suggestion item content.

```razor
<ElAutocomplete Suggestions="@links"
                @bind-Value="selectedLink"
                Placeholder="Pick a page">
    <SuggestionItemTemplate Context="item">
        <div class="autocomplete-link">
            <span>@item.Label</span>
            <small>@((item.Data as LinkInfo)?.Url)</small>
        </div>
    </SuggestionItemTemplate>
</ElAutocomplete>

@code {
    private string selectedLink;
    private IEnumerable<AutocompleteOption> links = new[]
    {
        new AutocompleteOption
        {
            Label = "Element Plus",
            Value = "element-plus",
            Data = new LinkInfo("https://element-plus.org")
        },
        new AutocompleteOption
        {
            Label = "Element-Blazor",
            Value = "element-blazor",
            Data = new LinkInfo("https://github.com/Element-Blazor/Element-Blazor")
        }
    };

    private sealed record LinkInfo(string Url);
}
```
