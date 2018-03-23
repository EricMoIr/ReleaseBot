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

        private static List<string> sourceURLs = new List<string>();
        public static string FindSource(string arg)
        {
            arg = arg.Trim().ToLower();
            if (sourceURLs.Count == 0)
            {
                sourceURLs = sources.Get().Select(x => x.URL).OrderBy(x => x).ToList();
            }
            foreach (string sourceURL in sourceURLs)
            {
                if (sourceURL.ToLower().IndexOf(arg) > -1)
                {
                    return sourceURL;
                }
            }
            return null;
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
        private static List<string> categories = new List<string>();
        public static string FindCategory(string arg)
        {
            arg = arg.Trim().ToLower();
            if (categories.Count == 0)
            {
                categories = sources.Get().Select(x => x.Category).Distinct().OrderBy(x => x).ToList();
            }
            foreach(string category in categories)
            {
                if (category.ToLower().IndexOf(arg) > -1)
                {
                    return category;
                }
            }
            return null;
        }
    }
}