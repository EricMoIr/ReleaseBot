using Persistence;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using Persistence.Domain;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

namespace Services
{
    internal class ReleaseFinder
    {
        public ReleaseFinder()
        {
        }
        public static List<FoundRelease> GetReleases(Source source)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = null;
            try
            {
                htmlDoc = web.Load(source.URL);
            }
            catch(System.Net.WebException e)
            {
                Console.WriteLine(e.Message);
                return new List<FoundRelease>();
            }
            string releaseXPATH = source.ReleaseHolder.toXPATH();
            string chapterNumberXPATH = source.ChapterNumberHolder.toXPATH();
            HtmlNodeCollection singleReleases = htmlDoc.DocumentNode.SelectNodes(releaseXPATH);
            HtmlNodeCollection chapterNumbers = htmlDoc.DocumentNode.SelectNodes(chapterNumberXPATH);
            if (singleReleases == null)
            {
                Console.WriteLine("The following XPATH for release holder is not valid or no releases were made: \n"
                    + releaseXPATH + " at " + source.URL);
                return new List<FoundRelease>();
            }
            if (chapterNumbers == null)
            {
                Console.WriteLine("The following XPATH for chapter numbers holder is not valid or no numbers are recorded: \n"
                    + chapterNumberXPATH + " at " + source.URL);
                return new List<FoundRelease>();
            }
            List<FoundRelease> foundReleases = new List<FoundRelease>();
            for (int i = 0; i < singleReleases.Count; i++)
            {
                string[] releaseDetails = FindDetails(singleReleases[i], chapterNumbers[i]);
                try
                {
                    FoundRelease release = new FoundRelease()
                    {
                        Title = releaseDetails[0],
                        Chapter = double.Parse(releaseDetails[1])
                    };
                    foundReleases.Add(release);
                }
                catch(Exception e)
                {
                    Console.WriteLine("FUCK "+string.Join("_",releaseDetails));
                }
            }
            return foundReleases;
        }

        internal static string[] FindDetails(HtmlNode releaseNode, HtmlNode chapterNode)
        {
            string[] ret = new string[2];
            ret[1] = FindChapter(chapterNode.InnerText.Trim());
            if (releaseNode.Equals(chapterNode))
            {
                string text = releaseNode.InnerText.Trim();
                int maxIndex = GetLastIndex(text, ret[1]);
                if (maxIndex == -1) //Doesn't have chapter
                    ret[0] = RemoveSeparator(text);
                else
                    ret[0] = RemoveSeparator(text.Substring(0, maxIndex).Trim());
            }
            else
            {
                ret[0] = FindTitle(releaseNode.InnerText.Trim());
            }
            return ret;
        }

        private static string RemoveSeparator(string text)
        {
            if (text[text.Length-1] == ':' || text[text.Length-1] == '-')
                text = text.Substring(0, text.Length - 1);
            return text.Trim();
        }

        private static int GetLastIndex(string text, string chapter)
        {
            //all these words should go into an array and/or be added with ML
            text = text.ToLower();
            int i = text.IndexOf("episode");
            if (i > -1) return i;
            i = text.IndexOf("chapter");
            if (i > -1) return i;
            i = text.IndexOf("episodio");
            if (i > -1) return i;
            i = text.IndexOf("capitulo");
            if (i > -1) return i;
            return text.LastIndexOf(chapter);
        }

        private static string FindChapter(string text)
        {
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
                if(decimals.Length > 0)
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

        private static string FindTitle(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Trim();
        }
    }
}
