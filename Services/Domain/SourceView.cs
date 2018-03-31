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
        public SourceView(Source source)
        {
            URL = CleanURL(source.URL);
            Category = source.Category;
        }

        public SourceView(string sourceURL, string link = null)
        {
            URL = CleanURL(sourceURL);
            Link = link;
        }

        public string URL { get; set; }
        public string Link { get; set;}
        public string Category { get; set; }

        public static string CleanURL(string sourceURL)
        {
            int firstSlash = sourceURL.Substring(8).IndexOf("/") + 8;
            if (firstSlash == 7) firstSlash = sourceURL.Length;
            return "<" + sourceURL.Substring(0, firstSlash) + ">";
        }

        public override bool Equals(object obj)
        {
            if (obj is SourceView)
            {
                SourceView other = (SourceView)obj;
                if(string.IsNullOrEmpty(Link) && string.IsNullOrEmpty(other.Link))
                    return other.URL == URL;
                if (string.IsNullOrEmpty(Link) || string.IsNullOrEmpty(other.Link))
                    return false;
                return Link == other.Link;
            }
            else
                return false;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Link))
                return URL;
            return Link;
        }
    }
}
