using System.Collections.Generic;
using System.Linq;

namespace Element
{
    public class CascaderSuggestion
    {
        public IList<CascaderOption> Path { get; set; } = new List<CascaderOption>();

        public CascaderOption Option => Path?.LastOrDefault();

        public IList<string> Values => Path?.Select(x => x.Value).Where(x => x != null).ToList() ?? new List<string>();

        public string Text { get; set; }
    }
}
