using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistence;
using Persistence.Domain;
using Services.Domain;

namespace Services
{
    public class SubscriptionService
    {
        private static ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
        private static ReleaseRepository<SourceSubscription> sourceSubscriptions = uow.SourceSubscriptions;
        public static void SubscribeToAllSources(string userId)
        {
            Subscriber user = SubscriberService.GetOrCreate(userId);
            IEnumerable<Source> sources = SourceService.GetAll();
            foreach(Source source in sources)
            {
                SubscribeToSource(source.URL, user.Name);
            }
        }
        public static void SubscribeToSource(string sourceURL, string userName)
        {
            Subscriber user = SubscriberService.GetOrCreate(userName);
            SourceSubscription sub =
                sourceSubscriptions
                .Get(x =>
                x.SourceURL == sourceURL && x.SubscribeeName == userName)
                .FirstOrDefault();
            if (sub == null)
            {
                sub = new SourceSubscription()
                {
                    SourceURL = sourceURL,
                    SubscribeeName = user.Name
                };
                sourceSubscriptions.Insert(sub);
                uow.Save();
            }
        }

        public static void SubscribeToCategory(string category, string userName)
        {
            var sources = uow.Sources.Get(x => x.Category == category);
            foreach(Source source in sources)
            {
                SubscribeToSource(source.URL, userName);
            }
        }

        public static void UnsubscribeFromCategory(string category, string userName)
        {
            var sources = uow.Sources.Get(x => x.Category == category);
            foreach (Source source in sources)
            {
                UnsubscribeFromSource(source.URL, userName);
            }
        }

        public static void UnsubscribeFromSource(string sourceURL, string userName)
        {
            SourceSubscription sub =
                sourceSubscriptions
                .Get(x =>
                x.SourceURL == sourceURL && x.SubscribeeName == userName)
                .FirstOrDefault();
            if (sub != null)
            {
                sourceSubscriptions.Delete(sub);
                uow.Save();
            }
        }

        public static List<SourceView> GetAll(string userName)
        {
            return sourceSubscriptions
                .Get(x => x.SubscribeeName == userName)
                .Select(x => new SourceView(x.SourceURL))
                .ToList();
        }

        public static void UnsubscribeFromAllSources(string userName)
        {
            var sourceURLs = sourceSubscriptions
                .Get(x => x.SubscribeeName == userName)
                .Select(x => x.SourceURL);
            foreach(string sourceURL in sourceURLs)
            {
                UnsubscribeFromSource(sourceURL, userName);
            }
        }
    }
}
