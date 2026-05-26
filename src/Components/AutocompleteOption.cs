using System;

namespace Element
{
    public class AutocompleteOption
    {
        public string Value { get; set; }

        public string Label { get; set; }

        public bool Disabled { get; set; }

        public object Data { get; set; }

        public override string ToString()
        {
            return Label ?? Value ?? string.Empty;
        }
    }
}
