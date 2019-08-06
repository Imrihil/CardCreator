using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using MyWarCreator.Crawler;
using MyWarCreator.DataSet;
using MyWarCreator.Extensions;
using MyWarCreator.Helpers;
using OfficeOpenXml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace MyWarCreator
{
    // https://www.codeproject.com/Articles/299436/WPF-Localization-for-Dummies
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeFonts();
            ProgressBar.Visibility = Visibility.Collapsed;
            ProgressBarText.Visibility = Visibility.Collapsed;
            TextBoxResultMessage.Visibility = Visibility.Visible;
            TextBoxResultMessage.IsReadOnly = true;
        }

        private void InitializeFonts()
        {
            UpdateTextBlockResultMessage("");
            FontsHelper.AddFont(Properties.Resources.Akvaléir_Normal_v2007);
            AppendTextBlockResultMessage($"Pobrano czcionkę {FontsHelper.Pfc.Families.LastOrDefault()?.Name}.");
            FontsHelper.AddFont(Properties.Resources.colonna_mt);
            AppendTextBlockResultMessage($"Pobrano czcionkę {FontsHelper.Pfc.Families.LastOrDefault()?.Name}.");
            FontsHelper.AddFont(Properties.Resources.runic);
            AppendTextBlockResultMessage($"Pobrano czcionkę {FontsHelper.Pfc.Families.LastOrDefault()?.Name}.");
            FontsHelper.AddFont(Properties.Resources.runic_altno);
            AppendTextBlockResultMessage($"Pobrano czcionkę {FontsHelper.Pfc.Families.LastOrDefault()?.Name}.");
            FontsHelper.AddFont(Properties.Resources.trebuc);
            FontsHelper.AddFont(Properties.Resources.trebucbd);
            FontsHelper.AddFont(Properties.Resources.trebucbi);
            FontsHelper.AddFont(Properties.Resources.trebucit);
            AppendTextBlockResultMessage($"Pobrano czcionkę {FontsHelper.Pfc.Families.LastOrDefault()?.Name}.");
        }

        private void ButtonGenerateWeapons_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            const string dirPath = @"../../AppData/equipment";
#else
            const string dirPath = @"./equipment";
#endif
            try
            {
                var filePath = dirPath + "/equipment.xlsx";
                if (Directory.Exists(dirPath))
                {
                    var equipmentSet = new EquipmentSet();
                    UpdateProgressBar(0);
                    UpdateTextBlockResultMessage("");
                    AppendTextBlockResultMessage(LoadCards(dirPath, filePath, equipmentSet, 0, 50));
                    AppendTextBlockResultMessage(GenerateCards(equipmentSet, 50, 100));
                }
                else
                {
                    AppendTextBlockResultMessage("Nie znaleziono katalogu o nazwie equipment!");
                }
            }
            catch (Exception ex)
            {
                AppendTextBlockResultMessage("Podczas generowania ekwipunku wystąpił błąd: " + ex.Message);
            }
        }

        private void ButtonGenerateSkills_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            const string dirPath = @"../../AppData/skills";
#else
            const string dirPath = @"./skills";
#endif
            try
            {
                var filePath = dirPath + "/skills.xlsx";
                if (Directory.Exists(dirPath))
                {
                    var skillsSet = new SkillsSet();
                    UpdateProgressBar(0);
                    UpdateTextBlockResultMessage("");
                    AppendTextBlockResultMessage(LoadCards(dirPath, filePath, skillsSet, 0, 50));
                    AppendTextBlockResultMessage(GenerateCards(skillsSet, 50, 100));
                }
                else
                {
                    AppendTextBlockResultMessage("Nie znaleziono katalogu o nazwie skills!");
                }
            }
            catch (Exception ex)
            {
                AppendTextBlockResultMessage("Podczas generowania umiejętności wystąpił błąd: " + ex.Message);
            }
        }

        private void ButtonDownloadMonsters_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            const string dirPath = @"../../AppData/monsters";
#else
            const string dirPath = @"./monsters";
#endif
            if (File.Exists(Path.Combine(dirPath, "monsters_dd.xlsx")))
            {
                var messageBoxResult = MessageBox.Show("Czy na pewno chcesz ponownie pobrać statystyki przeciwników ze strony http://www.d20srd.org? \n\nSpowoduje to usunięcie obecnego pliku monsters_dd.xlsx i stworzenie nowego.", "Potwierdzenie pobierania", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    File.Delete(Path.Combine(dirPath, "monsters_dd.xlsx"));
                    DownloadMonsters(dirPath);
                }
                else
                {
                    AppendTextBlockResultMessage("Anulowano pobieranie przeciwników.");
                }
            }
            else
            {
                DownloadMonsters(dirPath);
            }
        }

        private void ButtonGenerateAll_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            const string equipmentDirPath = @"../../AppData/equipment";
#else
            const string equipmentDirPath = @"./equipment";
#endif
            try
            {
                var filePath = equipmentDirPath + "/equipment.xlsx";
                if (Directory.Exists(equipmentDirPath))
                {
                    var equipmentSet = new EquipmentSet();
                    UpdateProgressBar(0);
                    UpdateTextBlockResultMessage("");
                    AppendTextBlockResultMessage(LoadCards(equipmentDirPath, filePath, equipmentSet, 0, 17));
                    AppendTextBlockResultMessage(GenerateCards(equipmentSet, 17, 33));
                }
                else
                {
                    AppendTextBlockResultMessage("Nie znaleziono katalogu o nazwie equipments!");
                }
            }
            catch (Exception ex)
            {
                AppendTextBlockResultMessage("Podczas generowania ekwipunku wystąpił błąd: " + ex.Message);
            }

#if DEBUG
            const string skillsDirPath = @"../../AppData/skills";
#else
            const string skillsDirPath = @"./skills";
#endif
            try
            {
                const string filePath = skillsDirPath + "/skills.xlsx";
                if (Directory.Exists(skillsDirPath))
                {
                    var skillsSet = new SkillsSet();
                    AppendTextBlockResultMessage(LoadCards(skillsDirPath, filePath, skillsSet, 33, 50));
                    AppendTextBlockResultMessage(GenerateCards(skillsSet, 50, 67));
                }
                else
                {
                    AppendTextBlockResultMessage("Nie znaleziono katalogu o nazwie skills!");
                }
            }
            catch (Exception ex)
            {
                AppendTextBlockResultMessage("Podczas generowania umiejętności wystąpił błąd: " + ex.Message);
            }

#if DEBUG
            const string monstersDirPath = @"../../AppData/monsters";
#else
            const string monstersDirPath = @"./monsters";
#endif
            try
            {
                const string filePathDd = monstersDirPath + "/monsters_dd.xlsx";
                const string filePath = monstersDirPath + "/monsters.xlsx";
                if (Directory.Exists(monstersDirPath))
                {
                    var monstersSet = new MonstersSet();
                    AppendTextBlockResultMessage(LoadCards(monstersDirPath, filePathDd, monstersSet, 67, 78, true));
                    AppendTextBlockResultMessage(LoadCards(monstersDirPath, filePath, monstersSet, 78, 89));
                    AppendTextBlockResultMessage(GenerateCards(monstersSet, 89, 100));
                }
                else
                {
                    AppendTextBlockResultMessage("Nie znaleziono katalogu o nazwie skills!");
                }
            }
            catch (Exception ex)
            {
                AppendTextBlockResultMessage("Podczas generowania kart potworów wystąpił błąd: " + ex.Message);
            }
        }

        private void DownloadMonsters(string dirPath)
        {
            try
            {
                var crawler = new CrawlerCore("http://www.d20srd.org", "/indexes/monsters.htm", dirPath);
                var i = 0;
                var extraI = 0;
                var n = crawler.Count;
                using (var package = new ExcelPackage(new FileInfo(Path.Combine(dirPath, "monsters_dd.xlsx"))))
                {
                    var monsterNames = new HashSet<string>();
                    var worksheet = package.Workbook.Worksheets.Add("Monsters");
                    var font = worksheet.Cells[1, 1].Style.Font;
                    font.Bold = true;
                    worksheet.Row(1).Style.Font = font;
                    for (var j = 0; j < MonsterData.Headers.Count; ++j)
                    {
                        worksheet.Cells[1, j + 1].Value = MonsterData.Headers[j];
                    }
                    while (crawler.HasNext())
                    {
                        try
                        {
                            var monsters = crawler.GetNextMonsters();
                            foreach (var monster in monsters)
                            {
                                ++extraI;
                                var row = monster.Row;
                                for (var j = 0; j < row.Count; ++j)
                                {
                                    worksheet.Cells[i + extraI + 1, j + 1].Value = row[j];
                                }
                                monsterNames.Add(monster.Name);
                                AppendTextBlockResultMessage($"{monster.Name} downloaded.");
                            }
                        }
                        catch (Exception ex)
                        {
                            AppendTextBlockResultMessage(
                                $"Przy przetwarzaniu strony {crawler.GetLastUrl()} wystąpił błąd: {ex.Message}");
                        }
                        finally
                        {
                            --extraI;
                            UpdateProgressBar((double)(++i) * 100 / n);
                        }
                    }
                    for (var j = 0; j < MonsterData.Headers.Count; ++j)
                    {
                        worksheet.Column(j + 1).AutoFit();
                        if (worksheet.Column(j + 1).Width > 100)
                            worksheet.Column(j + 1).Width = 100;
                    }
                    package.Workbook.Properties.Title = "Monsters DD";
                    package.Workbook.Properties.Author = "Mateusz Ledzianowski";
                    package.Workbook.Properties.Company = "MyWar";
                    package.Save();
                }
                AppendTextBlockResultMessage($"Pomyślnie pobrano potwory ze strony d&d ({n}).");
            }
            catch (Exception ex)
            {
                AppendTextBlockResultMessage("W czasie generowania kart potworów wystąpił błąd: " + ex.Message);
            }
        }

        private void ButtonGenerateMonsters_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            const string dirPath = @"../../AppData/monsters";
#else
            const string dirPath = @"./monsters";
#endif
            try
            {
                const string filePath = dirPath + "/monsters.xlsx";
                if (Directory.Exists(dirPath))
                {
                    var monstersSet = new MonstersSet();
                    UpdateProgressBar(0);
                    UpdateTextBlockResultMessage("");
                    AppendTextBlockResultMessage(LoadCards(dirPath, filePath, monstersSet, 0, 50));
                    AppendTextBlockResultMessage(GenerateCards(monstersSet, 50, 100));
                }
                else
                {
                    AppendTextBlockResultMessage("Nie znaleziono katalogu o nazwie skills!");
                }
            }
            catch (Exception ex)
            {
                AppendTextBlockResultMessage("Podczas generowania kart potworów wystąpił błąd: " + ex.Message);
            }
        }

        private void ButtonWeaponsPdf_Click(object sender, RoutedEventArgs e)
        {
            // Equipment
#if DEBUG
            const string dirPath = @"../../AppData/equipment";
#else
            const string dirPath = @"./equipment";
#endif
            UpdateProgressBar(0);
            UpdateTextBlockResultMessage("");
            AppendTextBlockResultMessage(PreparePdf(dirPath + "/results", 0, 100));
        }

        private void ButtonSkillsPdf_Click(object sender, RoutedEventArgs e)
        {
            // Skills
#if DEBUG
            const string dirPath = @"../../AppData/skills";
#else
            const string dirPath = @"./skills";
#endif
            UpdateProgressBar(0);
            UpdateTextBlockResultMessage("");
            AppendTextBlockResultMessage(PreparePdf(dirPath + "/results", 0, 100));
        }

        private void ButtonMonstersPdf_Click(object sender, RoutedEventArgs e)
        {
            // Skills
#if DEBUG
            const string dirPath = @"../../AppData/monsters";
#else
            const string dirPath = @"./monsters";
#endif
            UpdateProgressBar(0);
            UpdateTextBlockResultMessage("");
            AppendTextBlockResultMessage(PreparePdf(dirPath + "/results", 0, 100));
        }

        private void ButtonGeneratePdf_Click(object sender, RoutedEventArgs e)
        {
            // Equipment
#if DEBUG
            const string equipmentDirPath = @"../../AppData/equipment";
#else
            const string equipmentDirPath = @"./equipment";
#endif
            UpdateProgressBar(0);
            UpdateTextBlockResultMessage("");
            AppendTextBlockResultMessage(PreparePdf(equipmentDirPath + "/results", 0, 33));

            // Skills
#if DEBUG
            const string skillsDirPath = @"../../AppData/skills";
#else
            const string skillsDirPath = @"./skills";
#endif
            AppendTextBlockResultMessage(PreparePdf(skillsDirPath + "/results", 33, 66));

            // Monsters
#if DEBUG
            const string monstersDirPath = @"../../AppData/monsters";
#else
            const string monstersDirPath = @"./monsters";
#endif
            AppendTextBlockResultMessage(PreparePdf(monstersDirPath + "/results", 66, 100));
        }

        private void ButtonCustomPdf_Click(object sender, RoutedEventArgs e)
        {
            // Custom
#if DEBUG
            const string dirPath = @"../../AppData/custom";
#else
            const string dirPath = @"./custom";
#endif
            UpdateProgressBar(0);
            UpdateTextBlockResultMessage("");
            AppendTextBlockResultMessage(PreparePdf(dirPath, 0, 100));
        }

        private string LoadCards(string dirPath, string filePath, CardSet cardSet, int minProgressBar, int maxProgressBar, bool quiet = false)
        {
            if (!Directory.Exists(dirPath)) return $"Nie znaleziono katalogu {dirPath}!";

            if (!File.Exists(filePath)) return $"Nie znaleziono pliku {filePath}.";

            try
            {
                using (var xlPackage = new ExcelPackage(new FileInfo(filePath)))
                {
                    var myWorksheet = xlPackage.Workbook.Worksheets.First(); //select sheet here
                    var totalRows = myWorksheet.Dimension.End.Row;
                    var totalColumns = myWorksheet.Dimension.End.Column;

                    for (var rowNum = 10; rowNum <= totalRows; ++rowNum) //select starting row here
                    {
                        var row = new List<string>();
                        for (var colNum = 1; colNum <= totalColumns; ++colNum)
                        {
                            var cell = myWorksheet.Cells[rowNum, colNum];
                            row.Add(cell.Value == null ? string.Empty : cell.Value.ToString());
                        }
                        if (cardSet.AddRow(row, dirPath))
                        {
                            if (!quiet)
                                AppendTextBlockResultMessage($"Wczytano kartę {cardSet.LastOrDefault()?.Name}.");
                        }
                        else
                        {
                            AppendTextBlockResultMessage($"Błąd przy wczytywaniu karty {rowNum}.");
                        }
                        UpdateProgressBar(minProgressBar + (double)(rowNum - 1) * (maxProgressBar - minProgressBar) / (totalRows - 1));
                    }
                }
                return $"Pomyślnie wczytano plik {filePath}.";
            }
            catch (IOException ex)
            {
                return ex.Message;
            }
        }

        private string GenerateCards(CardSet cardSet, int minProgressBar, int maxProgressBar, bool createDuplicates = true)
        {
            try
            {
                var n = cardSet.Count;
                var cardsNames = new Dictionary<string, int>();
                for (var i = 0; i < n; ++i)
                {
                    var card = cardSet[i];
                    string result;
                    if (cardsNames.ContainsKey(card.Name))
                    {
                        result = createDuplicates ? card.GenerateFile("", " " + (++cardsNames[card.Name])) : $"Nie stworzono duplikatu karty {card.Name}.";
                    }
                    else
                    {
                        result = card.GenerateFile();
                        cardsNames.Add(card.Name, 1);
                    }
                    UpdateProgressBar(minProgressBar + (double)(i + 1) * (maxProgressBar - minProgressBar) / n);
                    AppendTextBlockResultMessage(result);
                }
                return $"Pomyślnie stworzono {n} kart{GetPolishEnding(n)}.";
            }
            catch (Exception ex)
            {
                return "W czasie generowania kart wystąpił błąd: " + ex.Message;
            }
        }

        private string PreparePdf(string dirPath, int minProgressBar, int maxProgressBar)
        {
            try
            {
                AppendTextBlockResultMessage("Trwa przygotowywanie PDFa do wydruku...");
                using (var pdf = new PdfDocument())
                {
                    PdfPage pdfPage = null;
                    var filesPath = Directory.GetFiles(dirPath, "*.png");
                    var n = filesPath.Length;
                    if (n == 0)
                    {
                        UpdateProgressBar(100);
                        return "Brak plików do wygenerowania PDFa.";
                    }

                    var nCard = 0;
                    for (var i = 0; i < n; ++i)
                    {
                        var filePath = Path.GetFullPath(filesPath[i]);
                        int nToPrint;
                        if (filePath.ToLower().Contains("skills") && filePath.ToLower().Contains("podstawow"))
                            nToPrint = 20;
                        else if (filePath.ToLower().Contains("equipment") && filePath.ToLower().Contains("początkow"))
                            nToPrint = 3;
                        else
                            nToPrint = 1;
                        for (var j = 0; j < nToPrint; ++j)
                        {
                            if (nCard % 9 == 0)
                                pdfPage = pdf.AddPage();
                            var xRect = new XRect((nCard % 3) * 185 + 20, ((nCard % 9) / 3) * 258 + 20, 184, 257);
                            using (var xGraphics = XGraphics.FromPdfPage(pdfPage))
                            {
                                using (var xImage = XImage.FromFile(filePath))
                                {
                                    xGraphics.DrawImage(xImage, xRect);
                                    ++nCard;
                                }
                            }
                        }
                        UpdateProgressBar(minProgressBar + (double)(i + 1) * (maxProgressBar - minProgressBar) / n);
                    }
                    pdf.Save(Path.GetFullPath(dirPath + "/AllFilesPrint.pdf"));
                }
                return "Przygotowano dokumenty do druku.";
            }
            catch (DirectoryNotFoundException)
            {
                return $"Nie znaleziono katalogu {dirPath}.";
            }
        }

        private long progressBarLastRefresh;
        private void UpdateProgressBar(double value, Visibility visibility = Visibility.Visible)
        {
            ProgressBar.Visibility = visibility;
            ProgressBarText.Visibility = visibility;
            ProgressBar.Value = value;
            var time = DateTime.Now.Ticks;
            if (time - progressBarLastRefresh <= 1000) return;

            ProgressBar.Refresh();
            ProgressBarText.Refresh();
            progressBarLastRefresh = time;
        }

        private long textBoxResultMessageLastRefresh;
        private void UpdateTextBlockResultMessage(string text, Visibility visibility = Visibility.Visible)
        {
            TextBoxResultMessage.Visibility = visibility;
            TextBoxResultMessage.Text = text;
            var time = DateTime.Now.Ticks;
            if (time - textBoxResultMessageLastRefresh <= 1000) return;

            TextBoxResultMessage.Refresh();
            textBoxResultMessageLastRefresh = time;
        }
        private void AppendTextBlockResultMessage(string text, Visibility visibility = Visibility.Visible)
        {
            TextBoxResultMessage.Visibility = visibility;
            if (string.IsNullOrEmpty(text)) return;

            TextBoxResultMessage.Text = (string.IsNullOrEmpty(TextBoxResultMessage.Text) ? "" : (TextBoxResultMessage.Text + "\n")) + text;
            TextBoxResultMessage.ScrollToEnd();
            var time = DateTime.Now.Ticks;
            if (time - textBoxResultMessageLastRefresh <= 1000) return;

            TextBoxResultMessage.Refresh();
            textBoxResultMessageLastRefresh = time;
        }

        private static string GetPolishEnding(int n)
        {
            if (n == 1) return "ę";
            if (n % 10 >= 2 && n % 10 < 5) return "y";
            return "";
        }
    }
}
