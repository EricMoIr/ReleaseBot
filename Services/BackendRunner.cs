﻿using Persistence.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                LookForReleases();
            }
            isRunning = true;
        }
        private static DateTime time;
        private static void LookForReleases()
        {
            IEnumerable<Source> releaseURLs = SourceService.GetAll(null, "ReleaseHolder,ChapterNumberHolder");
            Console.WriteLine("Started looking for releases");
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
            Console.WriteLine("Finished looking for releases");
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
                    SourceURL = source.URL
                };
                //Because all the following releases would also already be in the DB
                if (!ReleaseService.Create(toAdd)) return;
            }
        }
    }
}
