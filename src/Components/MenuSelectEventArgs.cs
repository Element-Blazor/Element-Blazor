using System.Collections.Generic;

namespace Element
{
    public class MenuSelectEventArgs
    {
        public string Index { get; set; }

        public IReadOnlyList<string> IndexPath { get; set; }

        public string Route { get; set; }

        public ElMenuItem Item { get; set; }
    }
}
