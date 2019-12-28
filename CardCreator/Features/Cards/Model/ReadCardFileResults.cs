using System;
using System.Collections.Generic;
using System.Text;

namespace CardCreator.Features.Cards.Model
{
    public class ReadCardFileResults
    {
        public List<string> CardSchemaParams { get; set; }
        public List<List<string>> ElementSchemasParams { get; set; }
        public List<List<string>> CardsElements { get; set; }
    }
}
