using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MyWarCreator.Helpers;

namespace MyWarCreator.Crawler
{
    public class CrawlerCore
    {
        private string MainPageUrl { get; }
        private string MonstersIndexUrl { get; }
        public int Count => monstersLinks.Count;
        private readonly List<string> monstersLinks;
        private int monstersIdx;
        private readonly string dirPath;

        public CrawlerCore(string mainPageUrl, string monstersIndexUrl, string dirPath)
        {
            MainPageUrl = mainPageUrl;
            MonstersIndexUrl = monstersIndexUrl;
            this.dirPath = dirPath;
            var webText = GetWebText(MainPageUrl + MonstersIndexUrl);
            var links = GetNewLinks(webText, "/srd/monsters/");
            monstersLinks = links.ToList();
            monstersLinks.Sort();
            monstersIdx = 0;
        }

        public bool HasNext()
        {
            return monstersIdx < monstersLinks.Count;
        }

        public IEnumerable<MonsterData> GetNextMonsters()
        {
            var monsters = new List<MonsterData>();

            var webText = GetWebText(MainPageUrl + monstersLinks[monstersIdx++]);
            var doc = new HtmlDocument();
            doc.LoadHtml(webText);

            var tableHtmlNodes = doc.DocumentNode.SelectNodes("//table");
            var tableHtmlNode = tableHtmlNodes.FirstOrDefault(x => x.HasClass("statBlock"));
            var tableDescendants = tableHtmlNode?.Descendants("tr");
            if (tableDescendants == null) return monsters;

            var table = tableDescendants
                        .Where(tr => tr.SelectNodes("th|td").Count > 1)
                        .Select(tr => tr.SelectNodes("th|td").Select(td => td.InnerText.Trim()).ToList())
                        .ToList();

            var imageNode = doc.DocumentNode.SelectSingleNode("//a[@class='monsterImage']");
            string imageUrl = null;
            string imageFullUrl = null;
            if (imageNode != null)
            {
                var imageHref = imageNode.GetAttributeValue("href", null);
                if (imageHref.Contains("javascript:ShowImage"))
                {
                    var showImageStartIdx = imageHref.IndexOf("javascript:ShowImage", StringComparison.InvariantCultureIgnoreCase);
                    showImageStartIdx = imageHref.IndexOf("(", showImageStartIdx, StringComparison.InvariantCultureIgnoreCase);
                    var showImageColon = imageHref.IndexOf(",", showImageStartIdx, StringComparison.InvariantCultureIgnoreCase);
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
                    using (var client = new WebClient())
                    {
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        var directories = imageUrl.Substring(0, imageUrl.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase));
                        if (!Directory.Exists(dirPath + "/" + directories))
                        {
                            Directory.CreateDirectory(dirPath + "/" + directories);
                        }
                        if (!File.Exists(dirPath + "/" + imageUrl))
                        {
                            client.DownloadFile(new Uri(imageFullUrl), dirPath + "/" + imageUrl);
                        }
                    }
                }
            }
            var docH1 = doc.DocumentNode.Descendants("h1").FirstOrDefault();
            var defaultName = docH1?.ChildNodes.FirstOrDefault()?.InnerText.Trim();

            if (table.Count <= 0) return monsters;

            for (var i = 1; i < table[0].Count; ++i)
            {
                monsters.Add(new MonsterData(table, defaultName + (i > 1 ? " " + i : ""), imageUrl != null ? dirPath + "/" + imageUrl : null, i));
            }

            return monsters;
        }

        public string GetLastUrl()
        {
            return monstersIdx > 0 ? monstersLinks[monstersIdx - 1] : monstersLinks[0];
        }

        //http://ericsowell.com/blog/2007/8/14/how-to-write-a-web-crawler-in-csharp
        private static string GetWebText(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "MyWarCreator .NET Web Crawler";
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null) return null;
                    using (var reader = new StreamReader(stream))
                    {
                        var htmlText = reader.ReadToEnd();
                        return htmlText;
                    }
                }
            }
        }

        //https://stackoverflow.com/questions/10452749/simple-web-crawler-in-c-sharp
        private static IEnumerable<string> GetNewLinks(string content, string filterRegex)
        {
            var regexLink = new Regex("(?<=<a\\s*?href=(?:'|\"))[^'\"]*?(?=(?:'|\"))");

            ISet<string> newLinks = new HashSet<string>();
            foreach (var match in regexLink.Matches(content))
            {
                var link = match.ToString();
                var idx = link.LastIndexOf('#');
                if (idx > 0)
                    link = link.Substring(0, idx);
                if (link.Contains(filterRegex) && !newLinks.Contains(link))
                    newLinks.Add(link);
            }

            return newLinks;
        }
    }
}
