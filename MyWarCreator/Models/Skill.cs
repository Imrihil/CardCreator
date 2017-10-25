using MyWarCreator.Extensions;
using MyWarCreator.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.Models
{
    class Skill : Card
    {
        public Image MainImage { get; set; }
        public Rectangle MainImageArea { get; set; } = new Rectangle(65, 40, 230, 230);
        public List<int> PTs { get; set; }
        public string PT { get { return string.Join("/", PTs.Select(x => x.ToString())); } }
        public Rectangle PTArea { get; set; } = new Rectangle(70, 25, 230, 32);
        public string Statistic { get; set; }
        public Rectangle StatisticArea { get; set; } = new Rectangle(300, 25, 35, 32);
        public string Attribute { get; set; }
        public string SkillName { get; set; }
        public override string Type { get { return $"{Attribute} - {SkillName}"; } }
        public Skill(IList<string> row, string dirPath) : base(row, dirPath)
        {
            RunesArea = new Rectangle(25, 25, 45, 70);

            if (row.Count < 5)
                throw new ArgumentException("W wierszu znajduje się za mało kolumn by utworzyć kartę!");
            int value;

            Rune = row[0];
            Statistic = row[1];
            Attribute = row[2];
            SkillName = row[3];
            Dmg = row[4];
            int.TryParse(row[6], out value);
            if (value != 0) PTs.Add(value);
            if (Rune == "NULL") Rune = null;
            Description = row[8] + $"\n[{row[9]}]";
            string backgroundPath = dirPath + "/background.png";
            for (int i = 10; i < 16; ++i)
            {
                int.TryParse(row[i], out value);
                if (value != 0 && value != PTs.Last()) PTs.Add(value);
            }

            if (File.Exists(backgroundPath))
                BackgroundImage = Image.FromFile(backgroundPath);
            string mainImagePath = dirPath + "/" + Name + ".png";
            if (File.Exists(mainImagePath))
            {
                MainImage = Image.FromFile(mainImagePath);
            }
        }
        public override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
            if (MainImage != null)
                DrawingHelper.MapDrawing(graphics, MainImage, MainImageArea);
            using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 24, FontStyle.Bold, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(PT, font, Brushes.Black, PTArea, FontsHelper.StringFormatCentered, 6);
        }
    }
}
