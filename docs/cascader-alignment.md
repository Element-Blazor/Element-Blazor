# ElCascader Alignment

`ElCascader` supports single path selection, multiple path selection, lazy loading, filtering, node templates, clear, disabled, size, and form validation integration.

## Multiple Selection

Use `Multiple` with `Values`/`ValuesChanged` for several selected paths. `Value` is still updated to the latest toggled path for compatibility with existing single-value form binding.

```razor
<ElCascader Options="@options"
            Multiple="true"
            Values="@selectedPaths"
            ValuesChanged="paths => selectedPaths = paths"
            Clearable="true" />
```

## Lazy Loading

Set `Lazy` and provide `LazyLoad`. The loader runs once per unloaded non-leaf node and writes returned children back to that `CascaderOption`.

```razor
<ElCascader Options="@roots"
            Lazy="true"
            LazyLoad="LoadChildrenAsync" />

@code {
    private Task<IEnumerable<CascaderOption>> LoadChildrenAsync(CascaderOption node)
    {
        var children = new[]
        {
            new CascaderOption { Label = $"{node.Label} / Child", Value = $"{node.Value}-child", Leaf = true }
        };

        return Task.FromResult<IEnumerable<CascaderOption>>(children);
    }
}
```

## Filter And Slots

`Filterable` lets the input show matching leaf paths. Use `NodeTemplate` for panel nodes and `SuggestionItemTemplate` for filtered suggestions.

```razor
<ElCascader Options="@options"
            Filterable="true"
            FilterMethod="(option, query) => (option.Label ?? option.Value).Contains(query, StringComparison.OrdinalIgnoreCase)">
    <NodeTemplate Context="node">
        <span>@node.Label</span>
        <small>@node.Value</small>
    </NodeTemplate>
    <SuggestionItemTemplate Context="suggestion">
        <span>@suggestion.Text</span>
    </SuggestionItemTemplate>
</ElCascader>
```
