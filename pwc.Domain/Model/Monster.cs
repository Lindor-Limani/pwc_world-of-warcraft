using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Model
{
    public class Monster
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Health { get; set; } // zusaetzlich, war nicht teil der Aufgabe
        public int Damage { get; set; } // zusaetzlich, war nicht teil der Aufgabe
        public ICollection<Item> Drops { get; set; } = new List<Item>();
        public ICollection<MonsterItemDrop> MonsterItemDrops { get; set; } = new List<MonsterItemDrop>();
    }
}
