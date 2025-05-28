using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Exceptions
{
    internal class DataLayerException : Exception
    {
        public DataLayerException(string message) : base(message)
        {
            
        }
    }
}
