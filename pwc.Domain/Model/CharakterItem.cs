using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Model
{
    public class CharakterItem
    {
        public int CharakterId { get; set; }
        public int ItemId { get; set; }
        public virtual Charakter Charakter { get; set; } = null!;
        public virtual Item Item { get; set; } = null!;

    }
}
