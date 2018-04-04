using MyWarCreator.DataSet;
using MyWarCreator.Extensions;
using MyWarCreator.Helpers;
using MyWarCreator.Models;
using MyWarCreator.Crawler;
using OfficeOpenXml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace MyWarCreator
{
    // https://www.codeproject.com/Articles/299436/WPF-Localization-for-Dummies
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeFonts();
            progressBar.Visibility = Visibility.Collapsed;
            progressBarText.Visibility = Visibility.Collapsed;
            textBoxResultMessage.Visibility = Visibility.Visible;
            textBoxResultMessage.IsReadOnly = true;
        }

        private void InitializeFonts()
        {
            updateTextBlockResultMessage("");
            FontsHelper.AddFont(Properties.Resources.Akvaléir_Normal_v2007);
            appendTextBlockResultMessage($"Pobrano czcionkę {FontsHelper.pfc.Families.LastOrDefault().Name}.");
            FontsHelper.AddFont(Properties.Resources.colonna_mt);
            appendTextBlockResultMessage($"Pobrano czcionkę {FontsHelper.pfc.Families.LastOrDefault().Name}.");
            FontsHelper.AddFont(Properties.Resources.runic);
            appendTextBlockResultMessage($"Pobrano czcionkę {FontsHelper.pfc.Families.LastOrDefault().Name}.");
            FontsHelper.AddFont(Properties.Resources.runic_altno);
            appendTextBlockResultMessage($"Pobrano czcionkę {FontsHelper.pfc.Families.LastOrDefault().Name}.");
            FontsHelper.AddFont(Properties.Resources.trebuc);
            FontsHelper.AddFont(Properties.Resources.trebucbd);
            FontsHelper.AddFont(Properties.Resources.trebucbi);
            FontsHelper.AddFont(Properties.Resources.trebucit);
            appendTextBlockResultMessage($"Pobrano czcionkę {FontsHelper.pfc.Families.LastOrDefault().Name}.");
        }

        private void buttonGenerateWeapons_Click(object sender, RoutedEventArgs e)
        {
            string dirPath = @"./equipment";
#if DEBUG
            dirPath = @"../../AppData/equipment";
#endif
            try
            {
                string filePath = dirPath + "/equipment.xlsx";
                if (Directory.Exists(dirPath))
                {
                    EquipmentSet equipmentSet = new EquipmentSet();
                    updateProgressBar(0);
                    updateTextBlockResultMessage("");
                    appendTextBlockResultMessage(loadCards(dirPath, filePath, equipmentSet, 0, 50));
                    appendTextBlockResultMessage(generateCards(dirPath, equipmentSet, 50, 100));
                }
                else
                {
                    appendTextBlockResultMessage("Nie znaleziono katalogu o nazwie equipments!");
                }
            }
            catch (Exception ex)
            {
                appendTextBlockResultMessage("Podczas generowania ekwipunku wystąpił błąd: " + ex.Message);
            }
        }

        private void buttonGenerateSkills_Click(object sender, RoutedEventArgs e)
        {
            string dirPath = @"./skills";
#if DEBUG
            dirPath = @"../../AppData/skills";
#endif
            try
            {
                string filePath = dirPath + "/skills.xlsx";
                if (Directory.Exists(dirPath))
                {
                    SkillsSet skillsSet = new SkillsSet();
                    updateProgressBar(0);
                    updateTextBlockResultMessage("");
                    appendTextBlockResultMessage(loadCards(dirPath, filePath, skillsSet, 0, 50));
                    appendTextBlockResultMessage(generateCards(dirPath, skillsSet, 50, 100));
                }
                else
                {
                    appendTextBlockResultMessage("Nie znaleziono katalogu o nazwie skills!");
                }
            }
            catch (Exception ex)
            {
                appendTextBlockResultMessage("Podczas generowania umiejętności wystąpił błąd: " + ex.Message);
            }
        }

        private void buttonDownloadMonsters_Click(object sender, RoutedEventArgs e)
        {
            string dirPath = @"./monsters";
#if DEBUG
            dirPath = @"../../AppData/monsters";
#endif
            if (File.Exists(Path.Combine(dirPath, "monsters_dd.xlsx")))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Czy na pewno chcesz ponownie pobrać statystyki przeciwników ze strony http://www.d20srd.org? \n\nSpowoduje to usunięcie obecnego pliku monsters_dd.xlsx i stworzenie nowego.", "Potwierdzenie pobierania", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    File.Delete(Path.Combine(dirPath, "monsters_dd.xlsx"));
                    downloadMonsters(dirPath);
                }
                else
                {
                    appendTextBlockResultMessage("Anulowano pobieranie przeciwników.");
                }
            }
            else
            {
                downloadMonsters(dirPath);
            }
        }

        private void buttonGenerateAll_Click(object sender, RoutedEventArgs e)
        {
            string dirPath = @"./equipment";
#if DEBUG
            dirPath = @"../../AppData/equipment";
#endif
            try
            {
                string filePath = dirPath + "/equipment.xlsx";
                if (Directory.Exists(dirPath))
                {
                    EquipmentSet equipmentSet = new EquipmentSet();
                    updateProgressBar(0);
                    updateTextBlockResultMessage("");
                    appendTextBlockResultMessage(loadCards(dirPath, filePath, equipmentSet, 0, 17));
                    appendTextBlockResultMessage(generateCards(dirPath, equipmentSet, 17, 33));
                }
                else
                {
                    appendTextBlockResultMessage("Nie znaleziono katalogu o nazwie equipments!");
                }
            }
            catch (Exception ex)
            {
                appendTextBlockResultMessage("Podczas generowania ekwipunku wystąpił błąd: " + ex.Message);
            }

            dirPath = @"./skills";
#if DEBUG
            dirPath = @"../../AppData/skills";
#endif
            try
            {
                string filePath = dirPath + "/skills.xlsx";
                if (Directory.Exists(dirPath))
                {
                    SkillsSet skillsSet = new SkillsSet();
                    appendTextBlockResultMessage(loadCards(dirPath, filePath, skillsSet, 33, 50));
                    appendTextBlockResultMessage(generateCards(dirPath, skillsSet, 50, 67));
                }
                else
                {
                    appendTextBlockResultMessage("Nie znaleziono katalogu o nazwie skills!");
                }
            }
            catch (Exception ex)
            {
                appendTextBlockResultMessage("Podczas generowania umiejętności wystąpił błąd: " + ex.Message);
            }

            dirPath = @"./monsters";
#if DEBUG
            dirPath = @"../../AppData/monsters";
#endif
            try
            {
                string filePathDD = dirPath + "/monsters_dd.xlsx";
                string filePath = dirPath + "/monsters.xlsx";
                if (Directory.Exists(dirPath))
                {
                    MonstersSet monstersSet = new MonstersSet();
                    appendTextBlockResultMessage(loadCards(dirPath, filePathDD, monstersSet, 67, 78, true));
                    appendTextBlockResultMessage(loadCards(dirPath, filePath, monstersSet, 78, 89));
                    appendTextBlockResultMessage(generateCards(dirPath, monstersSet, 89, 100));
                }
                else
                {
                    appendTextBlockResultMessage("Nie znaleziono katalogu o nazwie skills!");
                }
            }
            catch (Exception ex)
            {
                appendTextBlockResultMessage("Podczas generowania kart potworów wystąpił błąd: " + ex.Message);
            }
        }

        private void downloadMonsters(string dirPath)
        {
            try
            {
                CrawlerCore crawler = new CrawlerCore("http://www.d20srd.org", "/indexes/monsters.htm", dirPath);
                int i = 0;
                int extraI = 0;
                int n = crawler.Count;
                using (var package = new ExcelPackage(new FileInfo(Path.Combine(dirPath, "monsters_dd.xlsx"))))
                {
                    HashSet<string> monsterNames = new HashSet<string>();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Monsters");
                    var font = worksheet.Cells[1, 1].Style.Font;
                    font.Bold = true;
                    worksheet.Row(1).Style.Font = font;
                    for (int j = 0; j < MonsterData.Headers.Count; ++j)
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
                                IList<string> row = monster.Row;
                                for (int j = 0; j < row.Count; ++j)
                                {
                                    worksheet.Cells[i + extraI + 1, j + 1].Value = row[j];
                                }
                                monsterNames.Add(monster.Name);
                                appendTextBlockResultMessage($"{monster.Name} downloaded.");
                            }
                        }
                        catch (Exception ex)
                        {
                            appendTextBlockResultMessage(string.Format("Przy przetwarzaniu strony {0} wystąpił błąd: {1}", crawler.GetLastUrl(), ex.Message));
                        }
                        finally
                        {
                            --extraI;
                            updateProgressBar((double)(++i) * 100 / n);
                        }
                    }
                    for (int j = 0; j < MonsterData.Headers.Count; ++j)
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
                appendTextBlockResultMessage($"Pomyślnie pobrano potwory ze strony d&d ({n}).");
            }
            catch (Exception ex)
            {
                appendTextBlockResultMessage("W czasie generowania kart potworów wystąpił błąd: " + ex.Message);
            }
        }

        private void buttonGenerateMonsters_Click(object sender, RoutedEventArgs e)
        {
            string dirPath = @"./monsters";
#if DEBUG
            dirPath = @"../../AppData/monsters";
#endif
            try
            {
                string filePathDD = dirPath + "/monsters_dd.xlsx";
                string filePath = dirPath + "/monsters.xlsx";
                if (Directory.Exists(dirPath))
                {
                    MonstersSet monstersSet = new MonstersSet();
                    updateProgressBar(0);
                    updateTextBlockResultMessage("");
                    //appendTextBlockResultMessage(loadCards(dirPath, filePathDD, monstersSet, 0, 33, true));
                    appendTextBlockResultMessage(loadCards(dirPath, filePath, monstersSet, 0, 50));
                    appendTextBlockResultMessage(generateCards(dirPath, monstersSet, 50, 100));
                }
                else
                {
                    appendTextBlockResultMessage("Nie znaleziono katalogu o nazwie skills!");
                }
            }
            catch (Exception ex)
            {
                appendTextBlockResultMessage("Podczas generowania kart potworów wystąpił błąd: " + ex.Message);
            }
        }

        private void buttonWeaponsPdf_Click(object sender, RoutedEventArgs e)
        {
            // Equipment
            string dirPath = @"./equipment";
#if DEBUG
            dirPath = @"../../AppData/equipment";
#endif
            updateProgressBar(0);
            updateTextBlockResultMessage("");
            appendTextBlockResultMessage(preparePdf(dirPath + "/results", 0, 100));
        }

        private void buttonSkillsPdf_Click(object sender, RoutedEventArgs e)
        {
            // Skills
            string dirPath = @"./skills";
#if DEBUG
            dirPath = @"../../AppData/skills";
#endif
            updateProgressBar(0);
            updateTextBlockResultMessage("");
            appendTextBlockResultMessage(preparePdf(dirPath + "/results", 0, 100));
        }

        private void buttonMonstersPdf_Click(object sender, RoutedEventArgs e)
        {
            // Skills
            string dirPath = @"./monsters";
#if DEBUG
            dirPath = @"../../AppData/monsters";
#endif
            updateProgressBar(0);
            updateTextBlockResultMessage("");
            appendTextBlockResultMessage(preparePdf(dirPath + "/results", 0, 100));
        }

        private void buttonGeneratePdf_Click(object sender, RoutedEventArgs e)
        {
            // Equipment
            string dirPath = @"./equipment";
#if DEBUG
            dirPath = @"../../AppData/equipment";
#endif
            updateProgressBar(0);
            updateTextBlockResultMessage("");
            appendTextBlockResultMessage(preparePdf(dirPath + "/results", 0, 33));

            // Skills
            dirPath = @"./skills";
#if DEBUG
            dirPath = @"../../AppData/skills";
#endif
            appendTextBlockResultMessage(preparePdf(dirPath + "/results", 33, 66));

            // Monsters
            dirPath = @"./monsters";
#if DEBUG
            dirPath = @"../../AppData/monsters";
#endif
            appendTextBlockResultMessage(preparePdf(dirPath + "/results", 66, 100));
        }

        private void buttonCustomPdf_Click(object sender, RoutedEventArgs e)
        {
            // Custom
            string dirPath = @"./custom";
#if DEBUG
            dirPath = @"../../AppData/custom";
#endif
            updateProgressBar(0);
            updateTextBlockResultMessage("");
            appendTextBlockResultMessage(preparePdf(dirPath, 0, 100));
        }

        private string loadCards(string dirPath, string filePath, CardSet cardSet, int minProgressBar, int maxProgressBar)
        {
            return loadCards(dirPath, filePath, cardSet, minProgressBar, maxProgressBar, false);
        }

        private string loadCards(string dirPath, string filePath, CardSet cardSet, int minProgressBar, int maxProgressBar, bool quiet)
        {
            if (Directory.Exists(dirPath))
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(filePath)))
                        {
                            var myWorksheet = xlPackage.Workbook.Worksheets.First(); //select sheet here
                            var totalRows = myWorksheet.Dimension.End.Row;
                            var totalColumns = myWorksheet.Dimension.End.Column;

                            for (int rowNum = 2; rowNum <= totalRows; ++rowNum) //selet starting row here
                            {
                                List<string> row = new List<string>();
                                for (int colNum = 1; colNum <= totalColumns; ++colNum)
                                {
                                    var cell = myWorksheet.Cells[rowNum, colNum];
                                    row.Add(cell.Value == null ? string.Empty : cell.Value.ToString());
                                }
                                if (cardSet.AddRow(row, dirPath))
                                {
                                    if (!quiet)
                                        appendTextBlockResultMessage($"Wczytano kartę {cardSet.LastOrDefault().Name}.");
                                }
                                else
                                {
                                    appendTextBlockResultMessage($"Błąd przy wczytywaniu karty {rowNum}.");
                                }
                                updateProgressBar(minProgressBar + (double)(rowNum - 1) * (maxProgressBar - minProgressBar) / (totalRows - 1));
                            }
                        }
                        return $"Pomyślnie wczytano plik {filePath}.";
                    }
                    catch (IOException ex)
                    {
                        return ex.Message;
                    }
                }
                else
                {
                    return $"Nie znaleziono pliku {filePath}.";
                }
            }
            else
            {
                return $"Nie znaleziono katalogu {dirPath}!";
            }
        }

        private string generateCards(string dirPath, CardSet cardSet, int minProgressBar, int maxProgressBar)
        {
            return generateCards(dirPath, cardSet, minProgressBar, maxProgressBar, true);
        }

        private string generateCards(string dirPath, CardSet cardSet, int minProgressBar, int maxProgressBar, bool createDuplicates)
        {
            try
            {
                int n = cardSet.Count;
                string result;
                Dictionary<string, int> cardsNames = new Dictionary<string, int>();
                for (int i = 0; i < n; ++i)
                {
                    Card card = cardSet[i];
                    if (cardsNames.ContainsKey(card.Name))
                    {
                        if (createDuplicates)
                            result = card.GenerateFile("", " " + (++cardsNames[card.Name]).ToString());
                        else
                            result = $"Nie stworzono duplikatu karty {card.Name}.";
                    }
                    else
                    {
                        result = card.GenerateFile();
                        cardsNames.Add(card.Name, 1);
                    }
                    updateProgressBar(minProgressBar + (double)(i + 1) * (maxProgressBar - minProgressBar) / n);
                    appendTextBlockResultMessage(result);
                }
                return $"Pomyślnie stworzono {n} kart{GetPolishEnding(n)}.";
            }
            catch (Exception ex)
            {
                return "W czasie generowania kart wystąpił błąd: " + ex.Message;
            }
        }

        private string preparePdf(string dirPath, int minProgressBar, int maxProgressBar)
        {
            try
            {
                appendTextBlockResultMessage($"Trwa przygotowywanie pdfa do wydruku...");
                using (PdfDocument pdf = new PdfDocument())
                {
                    PdfPage pdfPage = null;
                    XRect xrect;
                    string[] filesPath = Directory.GetFiles(dirPath, "*.png");
                    int n = filesPath.Length;
                    int nCard = 0;
                    int nToPrint = 1;
                    for (int i = 0; i < n; ++i)
                    {
                        string filePath = Path.GetFullPath(filesPath[i]);
                        if (filePath.ToLower().Contains("skills") && filePath.ToLower().Contains("podstawow"))
                            nToPrint = 20;
                        else if (filePath.ToLower().Contains("equipment") && filePath.ToLower().Contains("początkow"))
                            nToPrint = 3;
                        else
                            nToPrint = 1;
                        for (int j = 0; j < nToPrint; ++j)
                        {
                            if (nCard % 9 == 0)
                                pdfPage = pdf.AddPage();
                            xrect = new XRect((nCard % 3) * 185 + 20, ((nCard % 9) / 3) * 258 + 20, 184, 257); // (2.5 x 3.5 inches) * 72 pt/inch
                            using (XGraphics xgraphics = XGraphics.FromPdfPage(pdfPage))
                            {
                                using (XImage ximage = XImage.FromFile(filePath))
                                {
                                    xgraphics.DrawImage(ximage, xrect);
                                    ++nCard;
                                }
                            }
                        }
                        updateProgressBar(minProgressBar + (double)(i + 1) * (maxProgressBar - minProgressBar) / n);
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

        private long progressBarLastRefresh = 0;
        private void updateProgressBar(double value, Visibility visibility = Visibility.Visible)
        {
            progressBar.Visibility = visibility;
            progressBarText.Visibility = visibility;
            progressBar.Value = value;
            long time = DateTime.Now.Ticks;
            if (time - progressBarLastRefresh > 1000)
            {
                progressBar.Refresh();
                progressBarText.Refresh();
                progressBarLastRefresh = time;
            }
        }

        private long textBoxResultMessageLastRefresh = 0;
        private void updateTextBlockResultMessage(string text, Visibility visibility = Visibility.Visible)
        {
            textBoxResultMessage.Visibility = visibility;
            textBoxResultMessage.Text = text;
            long time = DateTime.Now.Ticks;
            if (time - textBoxResultMessageLastRefresh > 1000)
            {
                textBoxResultMessage.Refresh();
                textBoxResultMessageLastRefresh = time;
            }
        }
        private void appendTextBlockResultMessage(string text, Visibility visibility = Visibility.Visible)
        {
            textBoxResultMessage.Visibility = visibility;
            if (!string.IsNullOrEmpty(text))
            {
                textBoxResultMessage.Text = (string.IsNullOrEmpty(textBoxResultMessage.Text) ? "" : (textBoxResultMessage.Text + "\n")) + text;
                textBoxResultMessage.ScrollToEnd();
                long time = DateTime.Now.Ticks;
                if (time - textBoxResultMessageLastRefresh > 1000)
                {
                    textBoxResultMessage.Refresh();
                    textBoxResultMessageLastRefresh = time;
                }
            }
        }

        private string GetPolishEnding(int n)
        {
            if (n == 1) return "ę";
            if (n % 10 >= 2 && n % 10 < 5) return "y";
            return "";
        }
    }
}
