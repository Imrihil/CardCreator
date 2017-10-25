using MyWarCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.DataSet
{
    abstract class CardSet : List<Card>
    {
        public abstract void AddRow(IList<string> row, string dirPath);
    }
}
