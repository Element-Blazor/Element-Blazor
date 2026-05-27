namespace Element.ControlConfigs
{
    public class InputOtpAttribute : BaseAttribute
    {
        public int Length { get; set; } = 6;

        public bool IsDisabled { get; set; }

        public bool Readonly { get; set; }

        public string Inputmode { get; set; } = "numeric";

        public bool Mask { get; set; }
    }
}
