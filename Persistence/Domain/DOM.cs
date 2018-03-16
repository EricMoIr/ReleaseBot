using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Domain
{
    public class DOM : DomainEntity
    {
        [Key]
        public virtual int Id { get; set; }
        public string ClassAttribute { get; set; }
        public string IdAttribute { get; set; }
        public string Tag { get; set; }
        public string toXPATH()
        {
            if (string.IsNullOrEmpty(Tag))
                throw new ArgumentNullException("The tag cannot be null");
            if (!string.IsNullOrEmpty(IdAttribute))
            {
                return "//" + Tag + "[@id='" + IdAttribute + "']";
            }
            return "//" + Tag + "[@class='" + ClassAttribute + "']";
        }
    }
}