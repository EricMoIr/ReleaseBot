using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistence;
using Persistence.Domain;

namespace Services
{
    public class SubscriptionService
    {
        private static ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
        private static ReleaseRepository<SourceSubscription> sourceSubscriptions = uow.SourceSubscriptions;
        public static void SubscribeToAllSources(string userId)
        {
            User user = UserService.GetOrCreate(userId);
            IEnumerable<Source> sources = SourceService.GetAll();
            foreach(Source source in sources)
            {
                CreateSubscription(source.URL, user.Name);
            }
        }
        private static void CreateSubscription(string sourceURL, string userName)
        {
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
                    SubscribeeName = userName
                };
                sourceSubscriptions.Insert(sub);
            }
            uow.Save();
        }

        public static void SubscribeToSource(string v1, string v2)
        {
            throw new NotImplementedException();
        }
    }
}
