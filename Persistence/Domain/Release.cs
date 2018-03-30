using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Domain
{
    public class Release : DomainEntity
    {
        [Key]
        public string ReleasableTitle { get; set; }
        [Key]
        public double Chapter { get; set; }
        [Key]
        public string SourceURL { get; set; }
        [Key]
        public string TimePublished { get; set; }

        public DateTime TimeFound { get; set; }

        [ForeignKey("ReleasableTitle")]
        public Releasable Item { get; set; }
        [ForeignKey("SourceURL")]
        public Source Source { get; set; }
        public string Link { get; set; }
        public string Author { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is Release)
            {
                Release other = (Release)obj;
                return other.Chapter == Chapter
                    && other.ReleasableTitle == ReleasableTitle
                    && other.SourceURL == SourceURL;
            }
            return false;
        }
    }
}
