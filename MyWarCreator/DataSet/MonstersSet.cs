using MyWarCreator.Helpers;
using MyWarCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.DataSet
{
    class MonstersSet : CardSet
    {
        private List<MonsterData> MonstersDD { get; } = new List<MonsterData>();
        public override bool AddRow(IList<string> row, string dirPath)
        {
            if (row.Skip(1).Take(1).Any(x => !string.IsNullOrEmpty(x)))
            {
                Add(new Monster(row, dirPath));
                return true;
            }
            return false;
        }
    }
}
