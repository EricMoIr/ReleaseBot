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
            URL = "<"+source.URL+">";
        }

        public string URL { get; set; }
    }
}
