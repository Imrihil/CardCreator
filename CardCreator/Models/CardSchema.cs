using System.Collections.Generic;

namespace CardCreator.Models
{
    public class CardSchema : List<ElementSchema>
    {
        public CardSchema() : base(new List<ElementSchema>()) { }
    }
}
