using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Model
{
    public class Charakter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CharakterItem> CharakterItems { get; set; } = new List<CharakterItem>();
    }
}
