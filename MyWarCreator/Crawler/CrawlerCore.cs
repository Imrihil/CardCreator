using MyWarCreator.Helpers;
using MyWarCreator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyWarCreator.Crawler
{
    class CrawlerCore
    {
        public string MainPageUrl { get; set; }
        public string MonstersIndexUrl { get; set; }
        public int Count { get { return MonstersLinks.Count; } }
        private List<string> MonstersLinks;
        private int MonstersIdx;
        private string DirPath;

        public CrawlerCore(string mainPageUrl, string monstersIndexUrl, string dirPath)
        {
            MainPageUrl = mainPageUrl;
            MonstersIndexUrl = monstersIndexUrl;
            DirPath = dirPath;
            List<MonsterData> monsters = new List<MonsterData>();
            string webText = GetWebText(MainPageUrl + MonstersIndexUrl);
            ISet<string> links = GetNewLinks(webText, "/srd/monsters/");
            MonstersLinks = links.ToList();
            MonstersLinks.Sort();
            MonstersIdx = 0;
        }

        public bool HasNext()
        {
            return MonstersIdx < MonstersLinks.Count;
        }

        public List<MonsterData> GetNextMonsters()
        {
            string webText = GetWebText(MainPageUrl + MonstersLinks[MonstersIdx++]);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(webText);

            var tableHtmlNodes = doc.DocumentNode.SelectNodes("//table");
            var tableHtmlNode = tableHtmlNodes.FirstOrDefault(x => x.HasClass("statBlock"));
            var tableDescendants = tableHtmlNode.Descendants("tr");
            List<List<string>> table = tableDescendants
                        .Where(tr => tr.SelectNodes("th|td").Count() > 1)
                        .Select(tr => tr.SelectNodes("th|td").Select(td => td.InnerText.Trim()).ToList())
                        .ToList();

            var imageNode = doc.DocumentNode.SelectSingleNode("//a[@class='monsterImage']");
            string imageUrl = null;
            string imageFullUrl = null;
            if (imageNode != null)
            {
                string imageHref = imageNode.GetAttributeValue("href", null);
                if (imageHref.Contains("javascript:ShowImage"))
                {
                    int showImageStartIdx = imageHref.IndexOf("javascript:ShowImage");
                    showImageStartIdx = imageHref.IndexOf("(", showImageStartIdx);
                    int showImageColon = imageHref.IndexOf(",", showImageStartIdx);
                    imageUrl = imageHref.Substring(showImageStartIdx + 1, showImageColon - showImageStartIdx - 1);
                    imageUrl = imageUrl.Replace("'", "");
                    imageFullUrl = "http://www.wizards.com/dnd/images/" + imageUrl;
                }
                else if (imageHref.Contains("http://www.wizards.com"))
                {
                    imageUrl = imageHref.Substring(34);
                    imageFullUrl = imageHref;
                }
                if (!string.IsNullOrWhiteSpace(imageFullUrl) && !string.IsNullOrWhiteSpace(imageUrl))
                {
                    using (WebClient client = new WebClient())
                    {
                        if (!Directory.Exists(DirPath))
                        {
                            Directory.CreateDirectory(DirPath);
                        }
                        string directories = imageUrl.Substring(0, imageUrl.LastIndexOf("/"));
                        if (!Directory.Exists(DirPath + "/" + directories))
                        {
                            Directory.CreateDirectory(DirPath + "/" + directories);
                        }
                        if (!File.Exists(DirPath + "/" + imageUrl))
                        {
                            client.DownloadFile(new Uri(imageFullUrl), DirPath + "/" + imageUrl);
                        }
                    }
                }
            }
            var docH1 = doc.DocumentNode.Descendants("h1").FirstOrDefault();
            var defaultName = docH1.ChildNodes.FirstOrDefault().InnerText.Trim();
            List<MonsterData> monsters = new List<MonsterData>();
            if (table.Count > 0)
            {
                for (int i = 1; i < table[0].Count; ++i)
                {
                    monsters.Add(new MonsterData(table, defaultName + (i > 1 ? " " + i.ToString() : ""), imageUrl != null ? DirPath + "/" + imageUrl : null, i));
                }
            }

            return monsters;
        }

        public string GetLastUrl()
        {
            if (MonstersIdx > 0)
                return MonstersLinks[MonstersIdx - 1];
            else return MonstersLinks[0];
        }

        //http://ericsowell.com/blog/2007/8/14/how-to-write-a-web-crawler-in-csharp
        private static string GetWebText(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.UserAgent = "MyWarCreator .NET Web Crawler";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string htmlText = reader.ReadToEnd();
            return htmlText;
        }

        //https://stackoverflow.com/questions/10452749/simple-web-crawler-in-c-sharp
        public ISet<string> GetNewLinks(string content, string filterRegex)
        {
            Regex regexLink = new Regex("(?<=<a\\s*?href=(?:'|\"))[^'\"]*?(?=(?:'|\"))");
            Regex regexFilter = new Regex(filterRegex);

            ISet<string> newLinks = new HashSet<string>();
            foreach (var match in regexLink.Matches(content))
            {
                if (match.ToString().Contains(filterRegex) && !newLinks.Contains(match.ToString()))
                    newLinks.Add(match.ToString());
            }

            return newLinks;
        }
    }
}
