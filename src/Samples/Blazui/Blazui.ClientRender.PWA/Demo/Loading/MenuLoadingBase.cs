

using Blazui.ClientRender.PWA.Demo.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Element;

namespace Blazui.ClientRender.PWA.Demo.Loading
{
    public class MenuLoadingBase : ComponentBase
    {
        protected IContainerComponent menu;

        protected void ShowLoading(object arg)
        {
            menu.Loading();
        }

        protected void CloseLoading()
        {
            menu.Close();
        }
    }
}
