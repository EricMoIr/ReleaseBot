using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Domain
{
    //This would be the discord server running my bot
    public class User : DomainEntity
    {
        [Key]
        public virtual string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
