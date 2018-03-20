using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Domain
{
    public class DOM : DomainEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public virtual int Id { get; set; }
        public string ClassAttribute { get; set; }
        public string IdAttribute { get; set; }
        public string Tag { get; set; }
        public string ParentClassAttribute { get; set; }
        //1-index
        public int ChildPosition { get; set; }
        public string ParentTag { get; set; }

        public string toXPATH()
        {
            if (string.IsNullOrEmpty(Tag))
                throw new ArgumentNullException("The tag cannot be null");
            if (!string.IsNullOrEmpty(IdAttribute))
            {
                return "//" + Tag + "[@id='" + IdAttribute + "']";
            }
            if (!string.IsNullOrEmpty(ClassAttribute))
            {
                return "//" + Tag + "[@class='" + ClassAttribute + "']";
            }
            if (!string.IsNullOrEmpty(ClassAttribute))
            {
                return "//" + ParentTag + "[@class='" + ParentClassAttribute + "']"
                    + "/" + Tag + "[" + ChildPosition + "]";
            }
            return "//" + ParentTag
                + "/" + Tag + "[" + ChildPosition + "]";
        }
    }
}