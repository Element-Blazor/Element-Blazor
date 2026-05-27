using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Xunit;

namespace Element.ComponentTests
{
    public class ElInputTagTests : BunitContext
    {
        public ElInputTagTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void TriggerKeyAddsTagAndAvoidsDuplicates()
        {
            var cut = Render<InputTagHost>();

            var input = cut.Find("input");
            input.Input("alpha");
            input.KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.Equal(new[] { "alpha" }, cut.Instance.Value);

            input = cut.Find("input");
            input.Input("alpha");
            input.KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.Equal(new[] { "alpha" }, cut.Instance.Value);
        }

        [Fact]
        public void BackspaceAndArrowKeysSelectAndRemoveTags()
        {
            IList<string> value = new List<string> { "one", "two", "three" };
            var cut = Render<ElInputTag>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next));

            var input = cut.Find("input");

            input.KeyDown(new KeyboardEventArgs { Key = "Backspace" });
            Assert.Contains("is-focus", cut.FindAll(".el-tag")[2].ClassList);

            input.KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });
            Assert.Contains("is-focus", cut.FindAll(".el-tag")[1].ClassList);

            input.KeyDown(new KeyboardEventArgs { Key = "Delete" });
            Assert.Equal(new[] { "one", "three" }, value);
        }

        [Fact]
        public void CtrlBackspaceRemovesAllTagsWhenInputIsEmpty()
        {
            IList<string> value = new List<string> { "one", "two", "three" };
            var removed = new List<string>();
            var cut = Render<ElInputTag>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnRemove, tag => removed.Add(tag)));

            cut.Find("input").KeyDown(new KeyboardEventArgs
            {
                Key = "Backspace",
                CtrlKey = true
            });

            Assert.Empty(value);
            Assert.Equal(new[] { "one", "two", "three" }, removed);
        }

        [Fact]
        public void HomeEndAndEscapeManageTagSelection()
        {
            IList<string> value = new List<string> { "one", "two", "three" };
            var cut = Render<ElInputTag>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next));

            var input = cut.Find("input");

            input.KeyDown(new KeyboardEventArgs { Key = "Home" });
            Assert.Contains("is-focus", cut.FindAll(".el-tag")[0].ClassList);

            input.KeyDown(new KeyboardEventArgs { Key = "End" });
            Assert.Contains("is-focus", cut.FindAll(".el-tag")[2].ClassList);

            input.KeyDown(new KeyboardEventArgs { Key = "Escape" });
            foreach (var tag in cut.FindAll(".el-tag"))
            {
                Assert.DoesNotContain("is-focus", tag.ClassList);
            }
        }

        [Fact]
        public void MetaDeleteRemovesSelectedTagAndFollowingTags()
        {
            IList<string> value = new List<string> { "one", "two", "three" };
            var cut = Render<ElInputTag>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next));

            var input = cut.Find("input");
            input.KeyDown(new KeyboardEventArgs { Key = "Backspace" });
            input.KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });
            input.KeyDown(new KeyboardEventArgs
            {
                Key = "Delete",
                MetaKey = true
            });

            Assert.Equal(new[] { "one" }, value);
        }

        [Fact]
        public void DraggingReordersTagsAndRaisesOnDrag()
        {
            IList<string> value = new List<string> { "one", "two", "three" };
            (int OldIndex, int NewIndex, string Tag)? dragged = null;
            var cut = Render<ElInputTag>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.Draggable, true)
                .Add(x => x.OnDrag, next => dragged = next));

            cut.FindAll(".el-tag")[0].DragStart(new DragEventArgs());
            cut.FindAll(".el-tag")[2].DragOver(new DragEventArgs());
            cut.FindAll(".el-tag")[2].Drop(new DragEventArgs());

            Assert.Equal(new[] { "two", "three", "one" }, value);
            Assert.Equal((0, 2, "one"), dragged);
        }

        [Fact]
        public void DraggingDisabledDoesNotExposeDraggableTags()
        {
            var cut = Render<ElInputTag>(parameters => parameters
                .Add(x => x.Value, new List<string> { "one", "two" })
                .Add(x => x.Draggable, false));

            foreach (var tag in cut.FindAll(".el-tag"))
            {
                Assert.False(tag.HasAttribute("draggable"));
                Assert.DoesNotContain("is-draggable", tag.ClassList);
            }
        }

        private class InputTagHost : ComponentBase
        {
            public IList<string> Value { get; set; } = new List<string>();

            protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
            {
                builder.OpenComponent<ElInputTag>(0);
                builder.AddAttribute(1, nameof(ElInputTag.Value), Value);
                builder.AddAttribute(2, nameof(ElInputTag.ValueChanged), EventCallback.Factory.Create<IList<string>>(this, OnValueChanged));
                builder.AddAttribute(3, nameof(ElInputTag.AllowDuplicates), false);
                builder.CloseComponent();
            }

            private void OnValueChanged(IList<string> value)
            {
                Value = value;
                StateHasChanged();
            }
        }
    }
}
