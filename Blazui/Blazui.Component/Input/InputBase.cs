using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component.CheckBox;
using Blazui.Component.EventArgs;

namespace Blazui.Component.Input
{
    public class InputBase : ComponentBase
    {
        [Parameter] public string InputValue { get; set; } = "";

        [Parameter] public string Placeholder { get; set; } = "请输入内容";
        [Parameter] public bool IsDisabled { get; set; } = false;
        [Parameter] public bool IsClearable { get; set; } = false;

        public bool isClearable = false;

        protected void ClearOnClick()
        {
            InputValue = string.Empty;
        }


        protected void OnFocusEventArgs()
        {
            int inputNum = 0;
            isClearable = inputNum > 0 && IsClearable;
        }

        protected void OnBlurEventArgs()
        {
            isClearable = false;
        }

        protected void OnChangeEventArgs(UIChangeEventArgs input)
        {
            int inputNum = 0;
            isClearable = inputNum > 0 && IsClearable;
        }
    }
}
