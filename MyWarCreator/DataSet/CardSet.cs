using System.Collections.Generic;
using MyWarCreator.Models;

namespace MyWarCreator.DataSet
{
    public abstract class CardSet : List<Card>
    {
        public abstract bool AddRow(IList<string> row, string dirPath);
    }
}
