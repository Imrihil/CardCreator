using System.Collections.Generic;
using System.Linq;
using MyWarCreator.Models;

namespace MyWarCreator.DataSet
{
    public class MonstersSet : CardSet
    {
        public override bool AddRow(IList<string> row, string dirPath)
        {
            if (row.Skip(1).Take(1).All(string.IsNullOrEmpty)) return false;

            Add(new Monster(row, dirPath));
            return true;
        }
    }
}
