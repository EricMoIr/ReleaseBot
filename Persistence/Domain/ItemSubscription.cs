using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Domain
{
    public class ItemSubscription : DomainEntity
    {
        [Key]
        public virtual int Id { get; set; }
        public User Subscribee { get; set; }
        public Releasable Item { get; set; }
    }
}
