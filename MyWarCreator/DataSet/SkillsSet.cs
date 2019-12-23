using System.Collections.Generic;
using System.Linq;
using MyWarCreator.Models;

namespace MyWarCreator.DataSet
{
    public class SkillsSet : CardSet
    {
        public override bool AddRow(IList<string> row, string dirPath)
        {
            if (row.Skip(3).Take(1).All(string.IsNullOrEmpty)) return false;

            var skill = new Skill(row, dirPath);

            if (string.IsNullOrEmpty(skill.Name)) return false;

            Add(skill);
            return true;
        }
    }
}
