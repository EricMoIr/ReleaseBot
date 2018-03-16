using Persistence;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using Persistence.Domain;

namespace Backend
{
    class ReleaseFinder
    {
        //public List<string> LastReleases { get;}
        //int maxSize = 10;

        public ReleaseFinder()
        {
            //LastReleases = new List<string>();
        }
        public static List<string> GetReleases(Source source)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(source.URL);
            string singleReleaseXPATH = source.ReleaseHolder.toXPATH();
            HtmlNodeCollection allSingleReleases = htmlDoc.DocumentNode.SelectNodes(singleReleaseXPATH);
            if (allSingleReleases.Count < 1)
            {
                throw new ArgumentException("The following XPATH for single release holder is not valid: \n"
                    + singleReleaseXPATH);
            }

            List<string> foundReleases = new List<string>();
            for(int i=0; i<allSingleReleases.Count; i++)
            {
                string title = FindTitle(allSingleReleases[i]);
                //if (LastReleases.Contains(title))
                //    break;
                foundReleases.Add(title);
                //LastReleases is screaming for a Deque
                //LastReleases.Insert(0, title);
                //if (LastReleases.Count > maxSize)
                //    LastReleases.RemoveAt(LastReleases.Count-1);
            }
            return foundReleases;
        }

        private static string FindTitle(HtmlNode htmlNode)
        {
            return htmlNode.InnerText;
        }
    }
}
