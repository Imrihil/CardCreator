using System.Collections.Generic;

namespace CardCreator.Settings
{
    public class TextSettings
    {
        public IDictionary<string, string> Icons { get; set; } = new Dictionary<string, string>();
        public int ShortestAloneWords { get; set; } = 0;
    }
}
