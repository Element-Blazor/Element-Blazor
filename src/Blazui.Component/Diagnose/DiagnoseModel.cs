using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Diagnose
{
    public class DiagnoseModel
    {
        public string MethodName { get; set; }
        public string StackTrace { get; set; }
        public IReadOnlyDictionary<string,object> Parameters { get; set; }

    }
}
