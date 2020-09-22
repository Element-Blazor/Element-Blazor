

using Blazui.ClientRender.PWA.Demo.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Element;

namespace Blazui.ClientRender.PWA.Demo.Loading
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
