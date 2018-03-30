using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Domain
{
    public class FoundRelease
    {
        public string Title { get; set; }
        public double Chapter { get; set; }
        public string Date { get; set; }
        public string Link { get; set; }
        public string Author { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is FoundRelease)
            {
                FoundRelease other = (FoundRelease)obj;
                return Title == other.Title && Chapter == other.Chapter;
            }
            return false;
        }
    }
}
