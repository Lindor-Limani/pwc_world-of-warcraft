using pwc.Domain.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.DTOs
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ItemCategory Category { get; set; }
        public int Staerke { get; set; }
        public int Ausdauer { get; set; }
        public int Geschicklichkeit { get; set; }
    }
}
