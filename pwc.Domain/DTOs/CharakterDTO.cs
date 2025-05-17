using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.DTOs
{
    public class CharakterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ItemDto> EquippedItems { get; set; } = new List<ItemDto>();
    }
}
