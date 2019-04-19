using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public abstract class BaseComponent<TDataModel, TComponentModel> : ComponentBase
    {
        [Parameter]
        protected List<TDataModel> For { get; set; }

        protected IList<TComponentModel> models { get; set; }

        protected bool IsCollectionDataSource()
        {
            return For != null;
        }

        protected bool ModelItemIsSimpleType { get; set; }

        protected override void OnInit()
        {
            if (IsCollectionDataSource())
            {
                var type = typeof(TDataModel);
                ModelItemIsSimpleType = type.IsValueType || type.IsPrimitive || type == typeof(string);
                if (ModelItemIsSimpleType)
                {
                    models = new List<TComponentModel>();
                    foreach (var item in For)
                    {
                        models.Add(ConvertModelItem(item));
                    }
                }
            }
        }

        protected abstract TComponentModel ConvertModelItem(TDataModel modelItem);
    }
}
