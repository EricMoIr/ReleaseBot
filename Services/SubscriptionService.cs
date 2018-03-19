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
            //Attention repeated subscriptions
            User user = UserService.GetOrCreate(userId);
            IEnumerable<Source> sources = SourceService.GetAll();
            foreach(Source source in sources)
            {
                SourceSubscription sub = new SourceSubscription()
                {
                    Source = source,
                    Subscribee = user
                };
                sourceSubscriptions.Insert(sub);
            }
            uow.Save();
        }
    }
}
