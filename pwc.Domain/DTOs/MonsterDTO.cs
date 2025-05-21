using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.DTOs
{
    public class MonsterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Health { get; set; } // zusaetzlich, war nicht teil der Aufgabe
        public int Damage { get; set; } // zusaetzlich, war nicht teil der Aufgabe 
        public ICollection<ItemDto> Drops { get; set; } = new List<ItemDto>();
    }
}
