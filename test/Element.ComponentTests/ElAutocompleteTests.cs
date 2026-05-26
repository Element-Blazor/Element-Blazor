using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Element.ComponentTests
{
    public class ElAutocompleteTests : BunitContext
    {
        public ElAutocompleteTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public async Task DebouncesRemoteSuggestions()
        {
            var calls = new List<string>();
            var cut = Render<ElAutocomplete>(parameters => parameters
                .Add(x => x.Debounce, 75)
                .Add(x => x.TriggerOnFocus, false)
                .Add(x => x.FetchSuggestionsAsync, query =>
                {
                    calls.Add(query);
                    return Task.FromResult<IEnumerable<AutocompleteOption>>(new[]
                    {
                        new AutocompleteOption { Label = query, Value = query }
                    });
                }));

            cut.Find("input").Input("a");
            cut.Find("input").Input("ab");

            await Task.Delay(25);
            Assert.Empty(calls);

            cut.WaitForAssertion(() =>
            {
                Assert.Equal(new[] { "ab" }, calls);
                Assert.Equal("ab", cut.Instance.CurrentSuggestions.Single().Value);
            });
        }

        [Fact]
        public async Task ClearCancelsPendingRemoteSuggestions()
        {
            var pending = new TaskCompletionSource<IEnumerable<AutocompleteOption>>();
            var cut = Render<ElAutocomplete>(parameters => parameters
                .Add(x => x.Debounce, 0)
                .Add(x => x.Value, "a")
                .Add(x => x.Clearable, true)
                .Add(x => x.TriggerOnFocus, false)
                .Add(x => x.FetchSuggestionsAsync, _ => pending.Task));

            cut.Find("input").Input("a");
            await Task.Delay(25);
            Assert.True(cut.Instance.IsInternalLoading);

            await cut.InvokeAsync(() => cut.Instance.ClearAsync());
            Assert.False(cut.Instance.IsInternalLoading);

            pending.SetResult(new[]
            {
                new AutocompleteOption { Label = "stale", Value = "stale" }
            });

            cut.WaitForAssertion(() => Assert.Empty(cut.Instance.CurrentSuggestions));
        }

        [Fact]
        public async Task IgnoresSlowerStaleRemoteResults()
        {
            var pendingA = new TaskCompletionSource<IEnumerable<AutocompleteOption>>();
            var pendingAb = new TaskCompletionSource<IEnumerable<AutocompleteOption>>();
            var cut = Render<ElAutocomplete>(parameters => parameters
                .Add(x => x.Debounce, 0)
                .Add(x => x.TriggerOnFocus, false)
                .Add(x => x.FetchSuggestionsAsync, query => query == "a" ? pendingA.Task : pendingAb.Task));

            cut.Find("input").Input("a");
            cut.Find("input").Input("ab");

            pendingAb.SetResult(new[]
            {
                new AutocompleteOption { Label = "current", Value = "current" }
            });
            cut.WaitForAssertion(() => Assert.Equal("current", cut.Instance.CurrentSuggestions.Single().Value));

            pendingA.SetResult(new[]
            {
                new AutocompleteOption { Label = "stale", Value = "stale" }
            });
            Assert.Equal("current", cut.Instance.CurrentSuggestions.Single().Value);
        }

        [Fact]
        public async Task RendersSuggestionItemTemplateAlias()
        {
            RenderFragment<AutocompleteOption> template = item => builder =>
            {
                builder.OpenElement(0, "strong");
                builder.AddAttribute(1, "class", "custom-suggestion");
                builder.AddContent(2, $"{item.Label}:{item.Data}");
                builder.CloseElement();
            };

            var cut = Render<ElAutocomplete>(parameters => parameters
                .Add(x => x.Debounce, 0)
                .Add(x => x.TriggerOnFocus, false)
                .Add(x => x.Suggestions, new[]
                {
                    new AutocompleteOption { Label = "Element Plus", Value = "element-plus", Data = "docs" }
                })
                .Add(x => x.SuggestionItemTemplate, template));

            cut.Find("input").Input("e");
            cut.WaitForAssertion(() => Assert.Single(cut.Instance.CurrentSuggestions));

            var dropdown = Services.GetRequiredService<PopupService>().SelectDropDownOptions.Single();
            var fragment = Render(dropdown.OptionContent);

            Assert.Contains("custom-suggestion", fragment.Markup);
            Assert.Contains("Element Plus:docs", fragment.Markup);
        }
    }
}
