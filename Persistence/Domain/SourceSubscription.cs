using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Domain
{
    public class SourceSubscription : DomainEntity
    {
        [Key]
        public string SubscribeeName { get; set; }
        [ForeignKey("SubscribeeName")]
        public User Subscribee { get; set; }

        [Key]
        public string SourceURL { get; set; }
        [ForeignKey("SourceURL")]
        public Source Source { get; set; }
    }
}
