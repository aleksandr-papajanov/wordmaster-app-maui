using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Generation.Interfaces
{
    public interface IRequest
    {
        void SetToContext(IContext context);
    }
}
