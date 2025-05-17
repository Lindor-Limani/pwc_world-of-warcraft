using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Model
{
    public class MonsterItemDrop
    {
        public int MonsterId { get; set; }
        public int ItemId { get; set; }
        public virtual Monster Monster { get; set; } = null!;
        public virtual Item Item { get; set; } = null!;
        public double DropChance { get; set; } // 0-100
    }
}
