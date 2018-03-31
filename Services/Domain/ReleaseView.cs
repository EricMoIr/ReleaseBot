using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistence.Domain;

namespace Services.Domain
{
    public class ReleaseView
    {

        public ReleaseView(Release release)
        {
            Name = release.ReleasableTitle;
            Chapter = release.Chapter;
            DatePublished = release.TimePublished;
            DateFound = release.TimeFound;
            Source = new SourceView(release.SourceURL, release.Link);
        }

        public string Name { get; set; }
        public double Chapter { get; set; }
        public SourceView Source { get; set; }
        public string DatePublished { get; set; }
        public DateTime DateFound { get; set;}

        public override bool Equals(object obj)
        {
            if (obj is ReleaseView)
            {
                ReleaseView other = (ReleaseView)obj;
                return other.Chapter == Chapter
                    && other.Name == Name
                    && other.DatePublished == DatePublished
                    && other.Source.Equals(Source);
            }
            return false;
        }
        public override int GetHashCode()
        {
            long hash = 0;
            hash += Chapter.GetHashCode();
            hash += Name.GetHashCode();
            hash += DatePublished.GetHashCode();
            return hash.GetHashCode();
        }
    }
}
