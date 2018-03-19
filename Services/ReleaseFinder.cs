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
                FoundRelease release = new FoundRelease()
                {
                    Title = FindTitle(singleReleases[i]),
                    Chapter = FindChapter(chapterNumbers[i])
                };
                foundReleases.Add(release);
            }
            return foundReleases;
        }

        private static double FindChapter(HtmlNode htmlNode)
        {
            string text = htmlNode.InnerText;
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(text);
            if (match.Success)
            {
                return double.Parse(match.Groups[match.Groups.Count - 1].Value);
            }
            throw new ArgumentException("The chapter number holder didn't held the chapter number");
        }

        private static string FindTitle(HtmlNode htmlNode)
        {
            return htmlNode.InnerText;
        }
    }
}
