using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Domain;
using Persistence;
using Persistence.Domain;

namespace Services
{
    public class ReleaseService
    {
        private static ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
        private static ReleaseRepository<Release> releases = uow.Releases;
        private static ReleaseRepository<SourceSubscription> sourcesSub = uow.SourceSubscriptions;
        public static List<ReleaseView> GetReleasesOfServer(string serverId)
        {
            //Actually, I only need one column, not everything. I need to look into this
            List<SourceSubscription> sourcesSubscribedTo =
                sourcesSub.Get(x => x.SubscribeeName == serverId).ToList();
            List<ReleaseView> releasesSubscribedTo = new List<ReleaseView>();
            foreach (SourceSubscription sourceSub in sourcesSubscribedTo)
            {
                releasesSubscribedTo.AddRange(releases.Get(x => x.SourceURL == sourceSub.SourceURL).Select(x => new ReleaseView(x)));
            }
            return releasesSubscribedTo;
        }
        internal static List<Release> GetReleasesOfSource(string sourceURL)
        {
            return releases.Get(x => x.SourceURL == sourceURL).ToList();
        }

        internal static bool Create(Release toAdd)
        {
            IEnumerable<Release> repeated = releases
                .Get(x =>
                x.ReleasableTitle == toAdd.ReleasableTitle
                && x.SourceURL == toAdd.SourceURL 
                && x.Chapter == toAdd.Chapter
                && x.TimePublished == toAdd.TimePublished);
            if (repeated.Count() > 0) return false;
            releases.Insert(toAdd);
            uow.Save();
            return true;
        }

        public static List<ReleaseView> GetNewReleasesOfServer(string serverId, double milliseconds)
        {
            //Actually, I only need one column, not everything. I need to look into this
            List<string> sourcesSubscribedTo =
                sourcesSub.Get(x => x.SubscribeeName == serverId)
                .Select(x => x.SourceURL).ToList();
            List<ReleaseView> releasesSubscribedTo = new List<ReleaseView>();
            DateTime oldestTime = DateTime.Now.AddMilliseconds(-milliseconds);
            Console.WriteLine($"Oldest time: {oldestTime.ToLongTimeString()}, Now: {DateTime.Now.ToLongTimeString()}");
            foreach (string sourceURL in sourcesSubscribedTo)
            {
                var releasesFound = releases.Get(x =>
                        x.SourceURL == sourceURL
                        && x.TimeFound > oldestTime);
                releasesSubscribedTo.AddRange(releasesFound.Select(x => new ReleaseView(x)));
            }
            return releasesSubscribedTo;
        }
    }
}
