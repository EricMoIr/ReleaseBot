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
        public string SubscribeeName { get; set; }
        [ForeignKey("SubscribeeName")]
        public Subscriber Subscribee { get; set; }

        [Key]
        public string ReleasableTitle { get; set; }
        [ForeignKey("ReleasableTitle")]
        public Releasable Item { get; set; }
    }
}
