using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Lang
{
    public interface ILang
    {
        void InitBLangBase();

        string T(string sections);
    }
}
