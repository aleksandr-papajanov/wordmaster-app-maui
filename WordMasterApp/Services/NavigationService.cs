using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMasterApp.Infrastructure.Interfaces;

namespace WordMasterApp.Services
{
    public class NavigationService : INavigationService
    {
        public NavigationService()
        {
            
        }


        public async Task NavigateToAsync(string pageName)
        {
            await Shell.Current.GoToAsync(pageName);
        }

        public async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
