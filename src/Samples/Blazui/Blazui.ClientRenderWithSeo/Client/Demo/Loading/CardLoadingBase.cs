

using Blazui.ClientRenderWithSeo.Client.Demo.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component;

namespace Blazui.ClientRenderWithSeo.Client.Demo.Loading
{
    public class CardLoadingBase : ComponentBase
    {
        protected IContainerComponent card;

        protected void ShowLoading(object arg)
        {
            card.Loading();
        }

        protected void CloseLoading()
        {
            card.Close();
        }
    }
}
