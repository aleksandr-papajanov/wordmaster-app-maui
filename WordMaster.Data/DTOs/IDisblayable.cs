using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.DTOs
{
    public interface IDisblayable
    {
        Guid Id { get; }
        string Text { get; }
    }
}
