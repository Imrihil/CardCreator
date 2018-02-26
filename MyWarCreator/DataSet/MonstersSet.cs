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
            if (row.Count == MonsterData.Headers.Count)
            {
                if (!MonstersDD.Any(x => x.Name == row[0]))
                {
                    MonstersDD.Add(new MonsterData(row.ToArray()));
                    return true;
                }
                return false;
            }
            else
            {
                MonsterData monsterData = MonstersDD.FirstOrDefault(x => x.Name.Equals(row[0]));
                if (monsterData == null)
                    monsterData = MonstersDD.FirstOrDefault(x => x.Name.StartsWith(row[0] + ","));
                if (monsterData == null)
                {
                    try
                    {
                        Add(new Monster(row, dirPath));
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    Monster monster = new Monster(monsterData, dirPath);
                    monster.Update(row, dirPath);
                    Add(monster);
                }
                return true;
            }
        }
    }
}
