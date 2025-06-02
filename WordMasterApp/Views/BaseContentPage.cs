using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMasterApp.Infrastructure.Interfaces;

namespace WordMasterApp.Views
{
    public abstract class BaseContentPage : ContentPage
    {
        private bool _isInitialized = false;

        protected BaseContentPage()
        {
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_isInitialized && BindingContext is IAsyncInitialize viewModel)
            {
                _isInitialized = true;
                await viewModel.InitializeAsync();
            }
        }
    }
}
