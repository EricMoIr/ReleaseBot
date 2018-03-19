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
        public static List<ReleaseView> Get(string serverId)
        {
            //Actually, I only need one column, not everything. I need to look into this
            List<SourceSubscription> sourcesSubscribedTo = 
                sourcesSub.Get(x => x.Subscribee.Name == serverId).ToList();
            List<ReleaseView> releasesSubscribedTo = new List<ReleaseView>();
            foreach(SourceSubscription sourceSub in sourcesSubscribedTo)
            {
                releasesSubscribedTo.AddRange(releases.Get(x => x.SourceURL == sourceSub.Source.URL).Select(x => new ReleaseView(x)));
            }
            return releasesSubscribedTo;
        }
    }
}
