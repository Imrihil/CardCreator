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
                Add(new Weapon(row, dirPath));
                return true;
            }
            else if (row.Skip(3).Take(1).Any(x => !string.IsNullOrEmpty(x)))
            {
                Add(new Armour(row, dirPath));
                return true;
            }
            else if (row[0].ToLower().Equals("łup"))
            {
                Add(new Loot(row, dirPath));
                int id = 1;
                while (File.Exists(dirPath + "/" + row[1] + " " + row[3] + id.ToString() + ".png")
                    || File.Exists(dirPath + "/" + row[1] + " " + row[3] + id.ToString() + ".jpg"))
                {
                    Add(new Loot(row, dirPath, id.ToString()));
                    ++id;
                }
                return true;
            }
            else
            {
                Add(new Equipment(row, dirPath));
                return true;
            }
        }
    }
}
