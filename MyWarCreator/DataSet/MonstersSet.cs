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
            if (row.Count == MonsterData.Headers.Count)
            {
                if (!this.Any(x => x.Name == row[0]))
                {
                    Add(new Monster(new MonsterData(row.ToArray()), dirPath));
                    return true;
                }
                return false;
            }
            else
            {
                Monster monster = (Monster)this.FirstOrDefault(x => x.Name.Equals(row[0]));
                if (monster == null)
                    monster = (Monster)this.FirstOrDefault(x => x.Name.StartsWith(row[0] + ","));
                if (monster == null)
                    try {
                        Add(new Monster(row, dirPath));
                    } catch
                    {
                        return false;
                    }
                else
                    monster.Update(row, dirPath);
                return true;
            }
        }
    }
}
