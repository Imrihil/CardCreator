using MyWarCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.DataSet
{
    class SkillsSet : CardSet
    {
        public override void AddRow(IList<string> row, string dirPath)
        {
            if (row.Skip(1).Take(1).Any(x => !string.IsNullOrEmpty(x)))
                Add(new Skill(row, dirPath));
        }
    }
}
