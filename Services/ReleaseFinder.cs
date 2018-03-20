using Persistence;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using Persistence.Domain;
using System.Text.RegularExpressions;

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
            HtmlDocument htmlDoc = web.Load(source.URL);
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
                FoundRelease release = new FoundRelease()
                {
                    Title = releaseDetails[0],
                    Chapter = double.Parse(releaseDetails[1])
                };
                foundReleases.Add(release);
            }
            return foundReleases;
        }

        internal static string[] FindDetails(HtmlNode releaseNode, HtmlNode chapterNode)
        {
            string text = releaseNode.InnerText;
            string[] ret = new string[2];
            ret[1] = FindChapter(chapterNode.InnerText);
            if (releaseNode.Equals(chapterNode))
            {
                int maxIndex = GetLastIndex(text, ret[1]);
                ret[0] = text.Substring(0, maxIndex).Trim();
            }
            else
            {
                ret[0] = FindTitle(releaseNode.InnerText);
                ret[1] = FindChapter(chapterNode.InnerText);
            }
            return ret;
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

        internal static string FindChapter(string text)
        {
            if (string.IsNullOrEmpty(text)) return "0";
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(text);
            string chapter = "0";
            while(match != null && match.Success)
            {
                chapter = match.Groups[match.Groups.Count - 1].Value;
                match = match.NextMatch();
            }
            return chapter.Trim();
        }

        internal static string FindTitle(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Trim();
        }
    }
}
