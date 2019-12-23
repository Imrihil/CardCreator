using System.Collections.Generic;

namespace MyWarCreator.Models
{
    public class CardSchema : List<ElementSchema>
    {
        public CardSchema() : base(new List<ElementSchema>()) { }
    }
}
