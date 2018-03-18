using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Persistence.Domain
{
    public class Releasable : DomainEntity
    {
        [Key]
        public virtual string Title { get; set; }
        public List<string> AlternativeTitles { get; set; }
        public string AlternativeTitlesAsString { get
            {
                return string.Join("^", AlternativeTitles);
            }
            set
            {
                AlternativeTitles = value.Split('^').ToList();
            }
        }
        //This is not against Liscow... There won't be a different behavior according to 
        //the type. It's only used for filters of releasables
        public string Type { get; set; }
        public Releasable()
        {
            AlternativeTitles = new List<string>();
        }

        public bool HasTitle(string title)
        {
            return title.Equals(Title) || AlternativeTitles.Contains(title);
        }
    }
}