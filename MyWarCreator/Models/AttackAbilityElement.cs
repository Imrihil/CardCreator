using MyWarCreator.Helpers;
using System.Drawing;

namespace MyWarCreator.Models
{
    public class AttackAbilityElement
    {
        public string Name { get; set; }
        public string LowerName => Name.ToLower();
        public int Value { get; set; }
        public Image Image { get; set; }
        public AttackAbilityElement(string dirPath, string name)
        {
            Name = name;
            Image = ImageHelper.LoadImage(dirPath, name);
        }
    }
}
