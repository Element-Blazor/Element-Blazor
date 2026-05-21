namespace Element
{
    internal interface ISelectDropDownContext
    {
        bool Loading { get; }

        string LoadingText { get; }

        string EmptyText { get; }

        bool ShouldShowEmpty { get; }

        bool ShouldShowNoMatch { get; }
    }
}
