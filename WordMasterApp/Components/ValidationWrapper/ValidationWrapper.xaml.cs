using ReactiveUI;
using System.ComponentModel;
using System.Reactive.Linq;

namespace WordMasterApp.Components.ValidationWrapper;

public partial class ValidationWrapper : ContentView, INotifyPropertyChanged
{
    public static readonly BindableProperty InnerContentProperty =
        BindableProperty.Create(nameof(InnerContent), typeof(View), typeof(ValidationEntry), default(View));

    public static readonly BindableProperty ValidationMessageProperty =
        BindableProperty.Create(nameof(ValidationMessage), typeof(string), typeof(ValidationEntry), string.Empty);

    public static readonly BindableProperty ValidationIsVisibleProperty =
        BindableProperty.Create(nameof(ValidationIsVisible), typeof(bool), typeof(ValidationEntry), false);

    public View InnerContent
    {
        get => (View)GetValue(InnerContentProperty);
        set => SetValue(InnerContentProperty, value);
    }

    public string ValidationMessage
    {
        get => (string)GetValue(ValidationMessageProperty);
        set => SetValue(ValidationMessageProperty, value);
    }

    public bool ValidationIsVisible
    {
        get => (bool)GetValue(ValidationIsVisibleProperty);
        set => SetValue(ValidationIsVisibleProperty, value);
    }


    private bool _isButtonVisible;
    public bool IsButtonVisible
    {
        get => _isButtonVisible;
        set
        {
            if (_isButtonVisible != value)
            {
                _isButtonVisible = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isFrameVisible;
    public bool IsFrameVisible
    {
        get => _isFrameVisible;
        set
        {
            if (_isFrameVisible != value)
            {
                _isFrameVisible = value;
                OnPropertyChanged();
            }
        }
    }
    
    private bool _showDetails;
    public bool ShowDetails
    {
        get => _showDetails;
        set
        {
            if (_showDetails != value)
            {
                _showDetails = value;
                OnPropertyChanged();
            }
        }
    }

    public ValidationWrapper()
    {
        InitializeComponent();

        Observable
            .CombineLatest(
                this.WhenAnyValue(x => x.ValidationMessage)
                    .Select(x => string.IsNullOrWhiteSpace(x)),
                this.WhenAnyValue(x => x.ValidationIsVisible),
                this.WhenAnyValue(x => x.ShowDetails))
            .Subscribe(x =>
            {
                var valid = x[0];
                var validate = x[1];
                var showDetails = x[2];

                if (validate && !valid)
                {
                    IsButtonVisible = true;
                    IsFrameVisible = ShowDetails;
                }
                else
                {
                    IsFrameVisible = false;
                    IsButtonVisible = false;
                }
            });
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        ShowDetails = !ShowDetails;

        // Perform some internal logic
        //if (Command?.CanExecute(CommandParameter) == true)
        //{
        //    Command.Execute(CommandParameter);
        //}
    }
}