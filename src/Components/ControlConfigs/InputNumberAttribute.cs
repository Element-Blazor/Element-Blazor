using System;

namespace Element.ControlConfigs
{
    public class InputNumberAttribute : BaseAttribute
    {
        public double Min { get; set; } = double.NaN;

        public double Max { get; set; } = double.NaN;

        public double Step { get; set; } = 1;

        public bool StepStrictly { get; set; }

        public int Precision { get; set; } = -1;

        public bool Controls { get; set; } = true;

        public string ControlsPosition { get; set; }

        public bool IsDisabled { get; set; }

        public bool Readonly { get; set; }

        public InputSize Size { get; set; } = InputSize.Normal;

        public string Placeholder { get; set; }

        public string Autocomplete { get; set; } = "off";

        public string Inputmode { get; set; } = "decimal";

        public int Tabindex { get; set; } = 0;

        public string AriaLabel { get; set; }

        public string AriaLabelledby { get; set; }

        public string AriaDescribedby { get; set; }

        public bool ValidateEvent { get; set; } = true;
    }
}
