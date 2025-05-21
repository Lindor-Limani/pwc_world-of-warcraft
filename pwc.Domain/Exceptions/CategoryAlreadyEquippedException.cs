using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Exceptions
{
    public class CategoryAlreadyEquippedException : Exception
    {
        public CategoryAlreadyEquippedException(string message) : base(message)
        {
        }
        public CategoryAlreadyEquippedException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public CategoryAlreadyEquippedException() : base("Category already equipped.")
        {
        }

    }
}
