using MyWarCreator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.DataSet
{
    class EquipmentSet : CardSet
    {
        public override bool AddRow(IList<string> row, string dirPath)
        {
            if (row.Skip(2).Take(1).Any(x => !string.IsNullOrEmpty(x)))
            {
                Add(new Equipment(row, dirPath));
                return true;
            }
            return false;
        }
    }
}
