using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistence.Domain;

namespace Services.Domain
{
    public class SourceView
    {
        public SourceView(string sourceURL, bool clean = true)
        {
            if (clean)
                URL = CleanURL(sourceURL);
            else
                URL = sourceURL;
        }

        public SourceView(Source source)
        {
            URL = CleanURL(source.URL);
            Category = source.Category;
        }

        public string URL { get; set; }
        public string Category { get; set; }

        public static string CleanURL(string sourceURL)
        {
            int firstSlash = sourceURL.Substring(8).IndexOf("/") + 8;
            if (firstSlash == 7) firstSlash = sourceURL.Length;
            return "<" + sourceURL.Substring(0, firstSlash) + ">";
        }
    }
}
