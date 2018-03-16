using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Domain
{
    public class ReleaseDate : DomainEntity
    {
        [Key]
        public Releasable Item { get; set; }
        [Key]
        public DateTime Time { get; set; }
    }
}
