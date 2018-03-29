using Persistence.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Services
{
    public class BackendRunner
    {
        private static bool isRunning = false;
        private static double INTERVAL;
        public static void RunBackend(double interval)
        {
            INTERVAL = interval;
            if (!isRunning)
            {
                Timer checkForTime = new Timer(INTERVAL);
                checkForTime.Elapsed += new ElapsedEventHandler(LookForReleasesEvent);
                checkForTime.Start();
                LookForReleases();
            }
            isRunning = true;
        }

        private static void LookForReleasesEvent(object sender, ElapsedEventArgs e)
        {
            LookForReleases();
        }

        private static DateTime time;
        private static void LookForReleases()
        {
            IEnumerable<Source> releaseURLs = SourceService.GetAll();
            Console.WriteLine(DateTime.Now.ToLongTimeString() + "Started looking for releases");
            //This works under the assumption that the next LookForReleases runs after the current
            //one is over

            //Another way of doing this is only getting the releases of the sites that have
            //subscribers
            time = DateTime.Now;
            foreach (Source source in releaseURLs)
            {
                List<FoundRelease> releases = ReleaseFinder.GetReleases(source);
                StoreReleases(releases, source);
            }
            Console.WriteLine(DateTime.Now.ToLongTimeString() + "Finished looking for releases");
        }
        private static void StoreReleases(List<FoundRelease> releases, Source source)
        {
            foreach (FoundRelease release in releases)
            {
                Releasable item = ReleasableService.GetOrCreateReleasable(release);
                Release toAdd = new Release()
                {
                    ReleasableTitle = item.Title,
                    Time = time,
                    Chapter = release.Chapter,
                    SourceURL = source.URL,
                    DatePublished = release.Date,
                    Link = ParseLink(release.Link, source.URL)
                };
                //Because all the following releases would also already be in the DB
                if (!ReleaseService.Create(toAdd)) return;
            }
        }

        private static string ParseLink(string link, string sourceURL)
        {
            if (link == null) return null;
            if (link[0] == '/')
            {
                if (link[1] == '/')
                {
                    string protocol = sourceURL.Substring(0, sourceURL.IndexOf("//") + 2);
                    return protocol + link.Substring(2);
                }
                else
                {
                    int firstSlash = sourceURL.Substring(8).IndexOf("/") + 8;
                    if (firstSlash == 7) firstSlash = sourceURL.Length;
                    return sourceURL.Substring(0, firstSlash) + link;
                }
            }
            else
                return link;
        }
    }
}
