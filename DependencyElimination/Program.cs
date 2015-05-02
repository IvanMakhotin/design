using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DependencyElimination
{
    internal class Program
    {
        private static int totalLinks = 0;
        private static void Main(string[] args)
        {
            string filePath = "links.txt";
            int pageCount = 5;

            List<string> pageLinks;
            List<List<string>> allLinks;
            var sw = Stopwatch.StartNew();
            using (var http = new HttpClient())
            {
                pageLinks = GenerateLinks(pageCount).ToList();
                allLinks = GetAllLinks(pageLinks, http);
            }
            WriteStatisticToTextWriter(allLinks, pageLinks, Console.Out);
            File.WriteAllLines(filePath, allLinks.SelectMany(x => x));
            Console.WriteLine(sw.Elapsed);
        }

        private static void WriteStatisticToTextWriter(List<List<string>> allLinks, List<string> pageLinks, TextWriter textWriter)
        {
            for (int i = 0; i < pageLinks.Count; i++)
            {
                textWriter.WriteLine(pageLinks[i]);
                if (!(allLinks[i][0].Contains("Error")))
                    textWriter.WriteLine("found {0} links", allLinks[i].Count);
                else textWriter.WriteLine(allLinks[i][0]);
            }
            textWriter.WriteLine("Total links found: {0}", allLinks.SelectMany(x => x).Count());
            textWriter.WriteLine("Finished");
        }
        private static List<List<string>> GetAllLinks(List<string> pageLinks, HttpClient http)
        {
            var allLinks = new List<List<string>>();
            foreach (var pageLink in pageLinks)
            {
                var linksOnPage = new List<string>();

                string pageContent = GetPageContent(http, pageLink);
                if (pageContent != "")
                    linksOnPage = GetLinksOnPage(pageContent);
                allLinks.Add(linksOnPage);
            }
            return allLinks;
        }
        private static void WriteToFile(List<string> links, StreamWriter output)
        {
            foreach (var link in links) output.WriteLine(link);
        }


        private static IEnumerable<string> GenerateLinks(int n)
        {
            for (var i = 1; i <= n; i++)
                yield return "http://habrahabr.ru/top/page" + i;
        }

        private static string GetPageContent(HttpClient http, string pageLink)
        {
            var habrResponse = http.GetAsync(pageLink).Result;
            if (habrResponse.IsSuccessStatusCode)
                return habrResponse.Content.ReadAsStringAsync().Result;
            else
            {
                return "Error: " + habrResponse.StatusCode + " " + habrResponse.ReasonPhrase;
            }

        }
        private static List<string> GetLinksOnPage(string content)
        {

            var matches = Regex.Matches(content, @"\Whref=[""'](.*?)[""'\s>]").Cast<Match>();
            List<string> linksOnPage = matches.Select(x => x.Groups[1].Value).ToList();
            if (linksOnPage.Count == 0)
                return new List<string>() { content };
            return linksOnPage;
        }
    }
}