using System.Collections.Generic;

namespace CardCreator.Features.Cards.Model
{
    public class ReadCardFileResults
    {
        public List<string> CardSchemaParams { get; set; }
        public List<List<string>> ElementSchemasParams { get; set; }
        public List<List<string>> CardsElements { get; set; }
        public List<int> CardsRepetitions { get; internal set; }
    }
}
