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
        public override bool AddRow(IList<string> row, string dirPath)
        {
            if (row.Skip(3).Take(1).Any(x => !string.IsNullOrEmpty(x)))
            {
                Skill skill = new Skill(row, dirPath);
                if (!string.IsNullOrEmpty(skill.Name))
                {
                    Add(skill);
                    return true;
                }
            }
            return false;
        }
    }
}
