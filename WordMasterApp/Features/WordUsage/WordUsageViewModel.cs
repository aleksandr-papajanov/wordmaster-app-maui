using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WordMaster.Data.Services;
using WordMaster.Data.ViewModels;

namespace WordMasterApp.Features
{
    public class WordUsageViewModel : ReactiveObject, IActivatableViewModel, IDisposable
    {
        private readonly ObservableAsPropertyHelper<string> _exampleData;
        public string ExampleData => _exampleData.Value;

        public ViewModelActivator Activator => new ViewModelActivator();

        public WordUsageViewModel(IObservable<WordViewModel> currentWordStream, IWordUsageService service)
        {
            currentWordStream
                .Select(word => $"Слово: {word.Text}")
                .ToProperty(this, x => x.ExampleData, out _exampleData);

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                // Here you can set up your subscriptions or other resources
                // that need to be disposed of when the ViewModel is deactivated.
            });
        }

        public void Dispose()
        {
            
        }
    }
}
