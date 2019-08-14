using MyWarCreator.Helpers;
using System.Drawing;

namespace MyWarCreator.Models
{
    public class AttackAbilityElement
    {
        public string Name { get; }
        public int Value { get; set; }
        public Image Image { get; }
        public AttackAbilityElement(string dirPath, string name)
        {
            Name = name;
            Image = ImageHelper.LoadImage(dirPath, name);
        }
    }
}
