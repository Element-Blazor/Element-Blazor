
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Element;

namespace Element.ClientRender.Demo.Transfer
{
    public class BasicTransferBase : ComponentBase
    {
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
        internal List<TransferItem> List2 = new List<TransferItem>()
        {
            new TransferItem()
            {
                 Id="4",
                 Label="选项4"
            },
            new TransferItem()
            {
                 Id="5",
                 Label="选项5"
            }
        };
    }
}
