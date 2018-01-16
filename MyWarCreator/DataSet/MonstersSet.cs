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
        public override bool AddRow(IList<string> row, string dirPath)
        {
            try
            {
                Add(new Monster(new MonsterData(row.ToArray()), dirPath));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
