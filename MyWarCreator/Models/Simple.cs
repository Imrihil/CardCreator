using System.Collections.Generic;
using System.Drawing;

namespace MyWarCreator.Models
{
    public class Simple : Card
    {
        public Simple(IList<string> row, string dirPath) : base(dirPath)
        {
            Type = row[0];
            Name = row[1];
            NameArea = new Rectangle(5, 5, 350, 40);
            Description = row[2];
            DescriptionArea = new Rectangle(5, 50, 350, 455);
        }
    }
}
