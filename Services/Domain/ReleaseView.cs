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
            Sources = new List<SourceView>();
            DatePublished = release.DatePublished;
            if(release.Link != null)
                Sources.Add(new SourceView(release.Link, false));
            else
                Sources.Add(new SourceView(release.SourceURL));
        }

        public string Name { get; set; }
        public double Chapter { get; set; }
        public List<SourceView> Sources { get; set; }
        public string DatePublished { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ReleaseView)
            {
                ReleaseView other = (ReleaseView)obj;
                return other.Chapter == Chapter
                    && other.Name == Name
                    && other.DatePublished == DatePublished;
            }
            return false;
        }
    }
}
