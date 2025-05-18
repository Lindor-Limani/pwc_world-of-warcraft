using pwc.Domain.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Geschicklichkeit { get; set; }
        public int Staerke { get; set; }
        public int Ausdauer { get; set; }
        public ItemCategory Category { get; set; }

        public ICollection<Monster> DroppedBy { get; set; } = new List<Monster>();
        public ICollection<CharakterItem> CharakterItems { get; set; } = new List<CharakterItem>();
        public ICollection<MonsterItemDrop> MonsterItemDrops { get; set; } = new List<MonsterItemDrop>();
    }
}
