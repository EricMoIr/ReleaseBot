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
            Name = release.Item.Title;
            Chapter = release.Chapter;
            SourceURL = release.SourceURL;
        }

        public string Name { get; set; }
        public double Chapter { get; set; }
        public string SourceURL { get; set; }
    }
}
