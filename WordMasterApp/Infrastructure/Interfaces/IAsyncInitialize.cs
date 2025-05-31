using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Infrastructure.Interfaces
{
    internal interface IAsyncInitialize
    {
        Task InitializeAsync();
    }
}
