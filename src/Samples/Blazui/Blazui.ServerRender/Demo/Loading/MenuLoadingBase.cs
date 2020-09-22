

using Element.ServerRender.Demo.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Element;

namespace Element.ServerRender.Demo.Loading
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
