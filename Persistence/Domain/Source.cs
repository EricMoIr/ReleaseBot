using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Domain
{
    public class Source : DomainEntity
    {
        [Key]
        public virtual string URL { get; set; }
        public string ReleaseHolder { get; set; }
        public string ChapterNumberHolder { get; set; }
        public string DateTimeHolder { get; set; }
        public string Category { get; set; }
        public string AuthorHolder { get; set; }

        public override int GetHashCode()
        {
            return URL.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if(obj is Source)
            {
                return URL.Equals(((Source)obj).URL);
            }
            return false;
        }
        public override string ToString()
        {
            return URL;
        }
    }
}
