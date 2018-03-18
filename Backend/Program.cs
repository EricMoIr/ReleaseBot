using Persistence;
using Persistence.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Timers;

namespace Backend
{
    class Program
    {
        const double HALF_AN_HOUR = 1000 * 60 * 30;
        const double HOUR = HALF_AN_HOUR * 2;
        const double INTERVAL = HOUR;

        private static List<Source> releaseURLs = new List<Source>();
        static void Mains(string[] args)
        {
            ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
            DOM b1 = new DOM() { Tag = "asasdd" };
            Source s = new Source() { ReleaseHolder = b1, URL = "http://jkanime.net" };
            Releasable r = new Releasable()
            {
                Title = "Test"
            }; Releasable rr = new Releasable()
            {
                Title = "Tests"
            };
            Release r1 = new Release()
            {
                Source = s,
                Chapter = 11,
                Item = r,
                Time = DateTime.Now
            };
            Release r2 = new Release()
            {
                Source = s,
                Chapter = 12,
                Item = rr,
                Time = DateTime.Now
            };
            uow.Releases.Insert(r1);
            uow.Releases.Insert(r2);
            /*User u1 = new User() { Name = "Eric" };
            User u2 = new User() { Name = "Mord" };
            DOM b1 = new DOM() { Tag = "asasdd" };
            DOM b2 = new DOM() { ClassAttribute = "asd" };
            Source s1 = new Source() { ReleaseHolder = b1, URL = "http://jkanime.net" };
            Source s2 = new Source() { ReleaseHolder = b2, URL = "https://animeflv.net" };
            SourceSubscription a1 = new SourceSubscription() { Source = s1, Subscribee = u1 };
            SourceSubscription a2 = new SourceSubscription() { Source = s1, Subscribee = u2 };
            uow.Sources.Insert(s1);
            uow.Sources.Insert(s2);
            uow.Users.Insert(u1);
            uow.Users.Insert(u2);
            uow.SourceSubscriptions.Insert(a1);
            uow.SourceSubscriptions.Insert(a2);*/
            uow.Save();
            Console.WriteLine("asd");
            Console.Read();
        }

        static void Main(string[] args)
        {
            releaseURLs.Add(new Source()
            {
                URL = "https://animeflv.net",
                ReleaseHolder = new DOM()
                {
                    Tag = "strong",
                    ClassAttribute = "Title"
                },
                ChapterNumberHolder = new DOM()
                {
                    Tag = "span",
                    ClassAttribute = "Capi"
                }
            });
            releaseURLs.Add(new Source()
            {
                URL = "http://jkanime.net",
                ReleaseHolder = new DOM()
                {
                    Tag = "a",
                    ClassAttribute = "rated_title"
                },
                ChapterNumberHolder = new DOM()
                {
                    Tag = "span",
                    ParentTag = "div",
                    ParentClassAttribute = "rated_stars",
                    ChildPosition = 1
                }
            });
            LookForReleases();
            Console.WriteLine("End");
            Timer checkForTime = new Timer(INTERVAL);
            checkForTime.Elapsed += new ElapsedEventHandler(LookForReleasesEvent);
            checkForTime.Enabled = true;
            Console.ReadKey();
        }
        private static void LookForReleases()
        {
            notifiedTitles.Clear();
            //this works assuming the next LookForReleases runs after the current
            //one is over
            time = DateTime.Now;
            foreach (Source source in releaseURLs)
            {
                List<FoundRelease> releases = ReleaseFinder.GetReleases(source);
                StoreReleases(releases, source);
                NotifySubscribers(source, releases);
            }
        }

        private static void StoreReleases(List<FoundRelease> releases, Source source)
        {
            ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
            InsertSource(source);
            IEnumerable<Release> lastReleases = uow.Releases.Get(r => r.SourceURL == source.URL);
            foreach (FoundRelease release in releases)
            {
                Releasable item = GetOrCreateReleasable(release);
                Release toAdd = new Release()
                {
                    ReleasableTitle = item.Title,
                    Time = time,
                    Chapter = release.Chapter,
                    SourceURL = source.URL
                };
                if (lastReleases.Contains(toAdd))
                    return;
                uow.Releases.Insert(toAdd);
                uow.Save();
            }
        }

        private static void InsertSource(Source source)
        {
            ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
            bool exists = uow.Sources.Get(s => s.URL == source.URL).FirstOrDefault() != null;
            if (!exists)
                uow.Sources.Insert(source);
            uow.Save();
        }

        private static Releasable GetOrCreateReleasable(FoundRelease release)
        {
            ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
            var releasables = uow.Releasables.Get(x => x.Title == release.Title || x.AlternativeTitlesAsString.IndexOf(release.Title) > -1);
            var releasable = releasables.FirstOrDefault();
            if (releasable == null)
            {
                releasable = new Releasable()
                {
                    Title = release.Title
                };
            }
            return releasable;
        }

        private static HashSet<string> notifiedTitles = new HashSet<string>();
        private static DateTime time;

        private static void NotifySubscribers(Source source, List<FoundRelease> releases)
        {
            ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
            IEnumerable<SourceSubscription> SourceSubscribers = uow.SourceSubscriptions.Get(
                x => x.Source.URL == source.URL, null, "Subscribee");
            foreach (FoundRelease release in releases)
            {
                if (notifiedTitles.Contains(release.Title)) continue;
                notifiedTitles.Add(release.Title);
                IEnumerable<ItemSubscription> ItemSubscribers =
                    uow.ItemSubscriptions.Get(x => x.Item.Title == release.Title || x.Item.AlternativeTitlesAsString.IndexOf(release.Title) > -1);
                foreach (ItemSubscription subs in ItemSubscribers)
                {
                    Console.WriteLine("To: " + subs.Subscribee + "\n" + release + " was released at " + source.ToString());
                }
            }
            foreach (SourceSubscription subs in SourceSubscribers)
            {
                Console.WriteLine("To: " + subs.Subscribee.ToString() + "\nThe following releases were made at " + source.ToString());
                foreach (FoundRelease release in releases)
                {
                    Console.WriteLine("- " + release.Title);
                }
            }
        }

        private static void LookForReleasesEvent(object sender, ElapsedEventArgs e)
        {
            LookForReleases();
        }
    }
}
