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
        public SourceView(string sourceURL)
        {
            URL = "<" + sourceURL + ">";
        }

        public SourceView(Source source)
        {
            int firstSlash = source.URL.Substring(8).IndexOf("/") + 8;
            if (firstSlash == 7) firstSlash = source.URL.Length;
            URL = "<" + source.URL.Substring(0, firstSlash) + ">";
            Category = source.Category;
        }

        public string URL { get; set; }
        public string Category { get; set; }
    }
}
