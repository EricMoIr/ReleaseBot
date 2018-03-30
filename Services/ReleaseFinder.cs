using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using Persistence.Domain;
using System.Text;
using System.Linq;
using System.Web;

namespace Services
{
    internal class ReleaseFinder
    {
        public static List<FoundRelease> GetReleases(Source source)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = null;
            try
            {
                htmlDoc = web.Load(source.URL);
            }
            catch (System.Net.WebException e)
            {
                Console.WriteLine($"Couldn't connect to the source {source.URL}");
                Console.WriteLine(e.Message);
                return new List<FoundRelease>();
            }
            HtmlNodeCollection singleReleases = htmlDoc.DocumentNode.SelectNodes(source.ReleaseHolder);
            HtmlNodeCollection chapterNumbers = null;
            HtmlNodeCollection dateTimes = null;
            HtmlNodeCollection authors = null;
            if (!string.IsNullOrEmpty(source.ChapterNumberHolder))
                chapterNumbers = htmlDoc.DocumentNode.SelectNodes(source.ChapterNumberHolder);
            if (!string.IsNullOrEmpty(source.DateTimeHolder))
                dateTimes = htmlDoc.DocumentNode.SelectNodes(source.DateTimeHolder);
            if (!string.IsNullOrEmpty(source.AuthorHolder))
                authors = htmlDoc.DocumentNode.SelectNodes(source.AuthorHolder);

            if (singleReleases == null)
            {
                Console.WriteLine("The following XPATH for release holder is not valid or no releases were made: \n"
                    + source.ReleaseHolder + " at " + source.URL);
                return new List<FoundRelease>();
            }
            List<FoundRelease> foundReleases = new List<FoundRelease>();
            int maxI = MinSize(singleReleases, chapterNumbers, dateTimes, authors);
            for (int i = 0; i < maxI; i++)
            {
                FoundRelease release = new FoundRelease()
                {
                    Title = FindTitle(singleReleases[i], chapterNumbers, i),
                    Chapter = double.Parse(FindChapter(chapterNumbers, i)),
                    Date = FindDate(dateTimes, i),
                    Link = FindLink(singleReleases[i], source),
                    Author = FindAuthor(authors, i)
                };
                foundReleases.Add(release);
            }
            return foundReleases;
        }

        private static string FindAuthor(HtmlNodeCollection authors, int i)
        {
            if (authors == null)
                return "";
            return authors[i].InnerText;
        }

        private static string FindLink(HtmlNode node, Source source)
        {
            string link = node.GetAttributeValue("href", "");
            if (link == "") return "";
            if (link[0] == '/')
            {
                if (link[1] == '/')
                {
                    string protocol = source.URL.Substring(0, source.URL.IndexOf("//") + 2);
                    return protocol + link.Substring(2);
                }
                else
                {
                    int firstSlash = source.URL.Substring(8).IndexOf("/") + 8;
                    if (firstSlash == 7) firstSlash = source.URL.Length;
                    return source.URL.Substring(0, firstSlash) + link;
                }
            }
            else
                return link;
        }

        private static string FindDate(HtmlNodeCollection dateTimes, int i)
        {
            if (dateTimes == null)
                return "";
            return dateTimes[i].InnerText;
        }

        private static int MinSize(params HtmlNodeCollection[] nodes)
        {
            if (nodes == null || nodes.Length == 0 || nodes[0] == null)
                throw new ArgumentException();
            int min = nodes[0].Count;
            for (int i = 1; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    min = Math.Min(min, nodes[i].Count);
                }
            }
            return min;
        }

        private static string RemoveSeparator(string text)
        {
            if (text[text.Length - 1] == ':' || text[text.Length - 1] == '-')
                text = text.Substring(0, text.Length - 1);
            return text.Trim();
        }
        private static string[] episodeWords = new string[]
        {
            "episode",
            "chapter",
            "episodio",
            "capitulo"
        };
        private static int GetLastIndex(string text, string chapter)
        {
            text = text.ToLower();
            foreach (string word in episodeWords)
            {
                int i = text.IndexOf(word);
                if (i > -1) return i;
            }
            return text.LastIndexOf(chapter);
        }

        private static string FindChapter(HtmlNodeCollection nodes, int i)
        {
            if (nodes == null) return "0";
            var node = nodes[i];
            string text = node.InnerText.Trim();
            if (string.IsNullOrEmpty(text)) return "0";
            string chapter = GetLastDigits(text);
            int index = text.LastIndexOf(chapter);
            if (index < 2)
                return chapter;
            if (char.IsDigit(text[index - 2]) &&
                (text[index - 1] == '.' || text[index - 1] == ','))
            {
                string sub = text.Substring(0, index - 1);
                string decimals = GetLastDigits(sub);
                if (decimals.Length > 0)
                    return decimals + "." + chapter;
                return chapter;
            }
            return chapter;
        }

        private static string GetLastDigits(string text)
        {
            StringBuilder chapterBuilder = new StringBuilder();
            List<char> digits = new List<char>();
            for (int i = text.Length - 1; i > -1; i--)
            {
                if (char.IsDigit(text[i]))
                    digits.Add(text[i]);
                else if (text[i] == ' ' || text[i] == ':')
                    continue;
                else
                    break;
            }
            if (digits.Count == 0) return "0";
            foreach (char digit in Enumerable.Reverse(digits))
            {
                chapterBuilder.Append(digit);
            }
            return chapterBuilder.ToString();
        }

        private static string FindTitle(HtmlNode titleNode, HtmlNodeCollection chapterNodes, int i)
        {
            if (chapterNodes != null && titleNode.Equals(chapterNodes[i]))
            {
                string chapter = FindChapter(chapterNodes, i);
                string text = HttpUtility.HtmlDecode(titleNode.InnerText.Trim());
                int maxIndex = GetLastIndex(text, chapter);
                if (maxIndex == -1) //Doesn't have chapter
                    return RemoveSeparator(text);
                else
                    return RemoveSeparator(text.Substring(0, maxIndex).Trim());
            }
            else
            {
                string text = titleNode.InnerText.Trim();
                if (string.IsNullOrEmpty(text)) return "Empty Title";
                return text.Trim();
            }
        }
    }
}
