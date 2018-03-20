using System;
using Persistence.Domain;
using Persistence;
using System.Linq;

namespace Services
{
    internal class ReleasableService
    {
        private static ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
        private static ReleaseRepository<Releasable> releasables = uow.Releasables;
        internal static Releasable GetOrCreateReleasable(FoundRelease release)
        {

            var releasable = releasables.Get(x =>
            x.Title == release.Title || x.AlternativeTitlesAsString.IndexOf(release.Title) > -1)
            .FirstOrDefault();
            if (releasable == null)
            {
                releasable = new Releasable(release.Title);
                releasables.Insert(releasable);
                uow.Save();
            }
            return releasable;
        }
    }
}