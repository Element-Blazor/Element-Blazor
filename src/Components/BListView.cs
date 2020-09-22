using Element.Model;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Element
{
    public class BListView<ItemType> : BTable
    {
        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (DataType == null)
            {
                DataType = typeof(ListViewModel<>).MakeGenericType(typeof(ItemType));
            }
            Key = nameof(ListViewModel<string>.Item);
            return base.SetParametersAsync(parameters);
        }
    }
}
