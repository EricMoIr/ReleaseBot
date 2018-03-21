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

        private static Releasable GetReleasable(string title)
        {
            //Assuming the separator of AlternativeTitlesAsString
            //is never in a title
            return releasables.Get(x =>
            x.Title == title || x.AlternativeTitlesAsString.IndexOf(title) > -1)
            .FirstOrDefault();
        }

        internal static Releasable GetOrCreateReleasable(FoundRelease release)
        {
            var releasable = GetReleasable(release.Title);
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