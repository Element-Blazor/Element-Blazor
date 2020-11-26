

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Element;
namespace Element.Demo.Transfer
{
    public partial class FormTransfer : BComponentBase
    {
        internal BForm form;
        internal TransferModel value;
        internal List<TransferItem> List1 = new List<TransferItem>()
        {
            new TransferItem()
            {
                 Id="1",
                 Label="选项1"
            },
            new TransferItem()
            {
                 Id="2",
                 Label="选项2"
            },
            new TransferItem()
            {
                 Id="3",
                 Label="选项3"
            }
        };

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            value = new TransferModel()
            {
                Items = new List<string>() { "2" }
            };
        }

        internal void Submit()
        {
            if (!form.IsValid())
            {
                return;
            }
            var tansferModel = form.GetValue<TransferModel>();
            Alert(string.Join(",", tansferModel.Items));
        }
    }
}
