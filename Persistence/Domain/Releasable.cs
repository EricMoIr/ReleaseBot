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
        public string AlternativeTitlesAsString {
            get
            {
                return string.Join("^", AlternativeTitles);
            }
            set
            {
                AlternativeTitles = value.Split('^').ToList();
            }
        }
        //This is not against Liscow... There won't be a different behavior according to 
        //the type. It's only used for filters of releasables (maybe? hereditance might be
        //a thing later on)
        public string Category { get; set; }
        public Releasable()
        {
            AlternativeTitles = new List<string>();
        }

        public Releasable(string title)
        {
            Title = title;
            AlternativeTitles = AlternativeTitlesFromTitle(title);
        }

        private List<string> AlternativeTitlesFromTitle(string title)
        {
            List<string> titles = new List<string>();
            title = title.ToLower();
            titles.Add(title);
            titles.Add(title.Replace(" ", ""));
            titles.Add(title.Replace(",", ""));
            titles.Add(title.Replace(":", ""));
            titles.Add(title.Replace(";", ""));
            titles.Add(title.Replace("-", " "));
            return titles;
        }

        public override bool Equals(object other)
        {
            if(other is Releasable)
            {
                Releasable obj = (Releasable)other;
                return Title.Equals(obj.Title) || AlternativeTitles.Contains(obj.Title);
            }
            return false;
        }
    }
}