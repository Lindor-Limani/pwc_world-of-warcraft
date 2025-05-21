using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Exceptions
{
    public class SameEquippmentTwiceException : Exception
    {
        public SameEquippmentTwiceException(string message) : base(message)
        {
        }
        public SameEquippmentTwiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public SameEquippmentTwiceException() : base("You cannot equip the same item twice.")
        {
        }
    }
}
