using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WordMaster.Data.Services;
using WordMaster.Data.ViewModels;
using WordMasterApp.Infrastructure;

namespace WordMasterApp.Features
{
    public class WordUsageViewModel : ReactiveObject, IValidatableViewModel, IActivatableViewModel, IDisposable
    {
        private readonly IWordUsageService _service;
        private readonly IObservable<Guid> _wordId;


        private readonly ObservableAsPropertyHelper<string> _exampleData;
        public string ExampleData => _exampleData?.Value ?? "string";

        public WordUsageViewModel(IObservable<Guid> wordId, IWordUsageService service)
        {
            _wordId = wordId;
            _service = service;

            _wordId
                .Select(word => $"Слово: {word.ToString()}")
                .ToProperty(this, x => x.ExampleData, out _exampleData);

            //this.WhenActivated((CompositeDisposable disposables) =>
            //{
            //    //_currentWordIdObservable
            //    //    .Select(word => service.GetUsageExamplesAsync(word.Id))
            //    //    .Switch()
            //    //    .ObserveOn(RxApp.MainThreadScheduler)
            //    //    .Subscribe(examples =>
            //    //    {
            //    //        // Here you would typically update a collection or property with the examples
            //    //        // For demonstration, we just log the examples
            //    //        foreach (var example in examples)
            //    //        {
            //    //            System.Diagnostics.Debug.WriteLine($"Example: {example}");
            //    //        }
            //    //    })
            //    //    .DisposeWith(disposables);
            //});
        }

        public void Dispose()
        {
            
        }


        // Implementing IValidatableViewModel requires a ValidationContext property
        public IValidationContext ValidationContext { get; } = new ValidationContext();

        // Implementing IActivatableViewModel requires an Activator property
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
