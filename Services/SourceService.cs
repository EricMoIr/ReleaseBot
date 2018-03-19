using System;
using System.Collections.Generic;
using Persistence.Domain;
using Persistence;
using System.Linq;
using System.Text.RegularExpressions;
using Services.Domain;

namespace Services
{
    public class SourceService
    {
        private static ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
        private static ReleaseRepository<Source> sources = uow.Sources;
        internal static IEnumerable<Source> GetAll(
            Func<IQueryable<Source>, IOrderedQueryable<Source>> orderBy = null,
            string includeProperties = "")
        {
            return sources.Get(null, orderBy, includeProperties);
        }

        public static bool IsValidSource(string source)
        {
            //Regex r = new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");
            //if (!r.IsMatch(source)) return false;
            IEnumerable<Source> sources = GetAll();
            Source s = new Source()
            {
                URL = source
            };
            return sources.Contains(s);
        }
        private static List<SourceView> sourceViews = new List<SourceView>();
        public static List<SourceView> GetAllViews()
        {
            if(sourceViews.Count == 0)
            {
                IEnumerable<Source> sources = GetAll();
                foreach (Source s in sources)
                    sourceViews.Add(new SourceView(s));
            }
            return sourceViews;
        }
    }
}