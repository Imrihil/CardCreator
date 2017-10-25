using MyWarCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.DataSet
{
    class EquipmentSet : CardSet
    {
        public override void AddRow(IList<string> row, string dirPath)
        {
            if (row.Skip(2).Take(1).Any(x => !string.IsNullOrEmpty(x)))
                Add(new Weapon(row, dirPath));
            else if (row.Skip(12).Take(1).Any(x => !string.IsNullOrEmpty(x)))
                Add(new Armour(row, dirPath));
        }
    }
}
