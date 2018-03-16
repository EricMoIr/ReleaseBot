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
            User u1 = new User() { Name = "Eric" };
            User u2 = new User() { Name = "Mord" };
            DOM b1 = new DOM() { Tag = "asasdd" };
            DOM b2 = new DOM() { ClassAttribute = "asd" };
            Source s1 = new Source() { ReleaseHolder = b1, URL = "http://jkanime.net" };
            Source s2 = new Source() { ReleaseHolder = b2, URL = "https://animeflv.net" };
            SourceSubscription a1 = new SourceSubscription() { Source = s1, Subscribee = u1 };
            SourceSubscription a2 = new SourceSubscription() { Source = s1, Subscribee = u2 };
            uow.Sources.Insert(s1);
            uow.Sources.Insert(s2);
            /*uow.Users.Insert(u1);
            uow.Users.Insert(u2);*/
            uow.SourceSubscriptions.Insert(a1);
            uow.SourceSubscriptions.Insert(a2);
            uow.Save();
            Console.WriteLine("asd");
            Console.Read();
        }

        private static ReleaseUnitOfWork uow;
        static void Main(string[] args)
        {
            releaseURLs.Add(new Source()
            {
                URL = "https://animeflv.net",
                ReleaseHolder = new DOM()
                {
                    Tag = "strong",
                    ClassAttribute = "Title"
                }
            });
            releaseURLs.Add(new Source()
            {
                URL = "http://jkanime.net",
                ReleaseHolder = new DOM()
                {
                    Tag = "a",
                    ClassAttribute = "rated_title"
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
            foreach (Source source in releaseURLs)
            {
                List<string> releases = ReleaseFinder.GetReleases(source);
                NotifySubscribers(source, releases);
            }
        }
        private static HashSet<string> notifiedTitles = new HashSet<string>();
        private static void NotifySubscribers(Source source, List<string> releases)
        {
            uow = new ReleaseUnitOfWork();
            IEnumerable<SourceSubscription> SourceSubscribers = uow.SourceSubscriptions.Get(
                x => x.Source.URL == source.URL, null, "Subscribee");
            foreach (string title in releases)
            {
                if (notifiedTitles.Contains(title)) continue;
                notifiedTitles.Add(title);
                IEnumerable<ItemSubscription> ItemSubscribers =
                    //this is wrooooooooong
                    uow.ItemSubscriptions.Get(x => x.Item.Title == title || x.Item.AlternativeTitlesAsString.IndexOf(title)>-1);
                foreach (ItemSubscription subs in ItemSubscribers)
                {
                    Console.WriteLine("To: " + subs.Subscribee + "\n" + title + " was released at " + source.ToString());
                }
            }
            foreach (SourceSubscription subs in SourceSubscribers)
            {
                Console.WriteLine("To: " + subs.Subscribee.ToString() + "\nThe following releases were made at " + source.ToString());
                foreach (string title in releases)
                {
                    Console.WriteLine("- " + title);
                }
            }
        }

        private static void LookForReleasesEvent(object sender, ElapsedEventArgs e)
        {
            LookForReleases();
        }
    }
}
