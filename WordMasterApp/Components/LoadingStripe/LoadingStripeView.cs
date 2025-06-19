using Microsoft.Maui.Dispatching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMasterApp.Helpers;

namespace WordMasterApp.Components.LoadingStripe
{
    public class LoadingStripeView : GraphicsView
    {
        private readonly LoadingStripeDrawable _drawable;
        private readonly IDispatcherTimer _timer;

        public static readonly BindableProperty StripeTypeProperty =
            BindableProperty.Create(nameof(StripeType), typeof(LoadingStripeType), typeof(LoadingStripeView), LoadingStripeType.HttpRequest,
                propertyChanged: (bindable, _, newValue) =>
                {
                    if (bindable is not LoadingStripeView control || newValue is not LoadingStripeType stripeType)
                        return;

                    control.UpdateStripeColorsByType();
                });

        public static readonly BindableProperty StripeColorsProperty =
            BindableProperty.Create(nameof(StripeColors), typeof(IList<Color>), typeof(LoadingStripeView), new ObservableCollection<Color> { Colors.Blue },
                propertyChanged: (bindable, _, newValue) =>
                {
                    if (bindable is not LoadingStripeView control || newValue is not bool isAnimating)
                        return;

                    control.UpdateStripeColorsByType();
                });

        public static readonly BindableProperty SpeedProperty =
            BindableProperty.Create(nameof(Speed), typeof(double), typeof(LoadingStripeView), 15.0,
                propertyChanged: (bindable, _, newValue) =>
                {
                    if (bindable is not LoadingStripeView control)
                        return;

                    control.UpdateStripeColorsByType();
                });

        public static readonly BindableProperty IsAnimatingProperty =
            BindableProperty.Create(nameof(IsAnimating), typeof(bool), typeof(LoadingStripeView), false,
                propertyChanged: (bindable, _, newValue) =>
                {
                    if (bindable is not LoadingStripeView control)
                        return;

                    control.UpdateTimerState();
                });

        public IList<Color> StripeColors
        {
            get => (IList<Color>)GetValue(StripeColorsProperty);
            set => SetValue(StripeColorsProperty, value);
        }
        
        public LoadingStripeType StripeType
        {
            get => (LoadingStripeType)GetValue(StripeTypeProperty);
            set => SetValue(StripeColorsProperty, value);
        }

        public double Speed
        {
            get => (double)GetValue(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }

        public bool IsAnimating
        {
            get => (bool)GetValue(IsAnimatingProperty);
            set => SetValue(IsAnimatingProperty, value);
        }

        public LoadingStripeView()
        {
            _drawable = new LoadingStripeDrawable();
            Drawable = _drawable;

            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(20);
            _timer.Tick += (s, e) => Invalidate();

            UpdateStripeColorsByType();
            IsVisible = false;
        }

        private void UpdateStripeColorsByType()
        {
            StripeColors = StripeType switch
            {
                LoadingStripeType.HttpRequest => new List<Color> { this.GetThemeColor("Primary"), this.GetThemeColor("Secondary") },
                _ => new List<Color> { Colors.Transparent }
            };

            _drawable.UpdateAnimations(StripeColors, Speed);
        }


        private void UpdateTimerState()
        {
            bool canAnimate = IsAnimating &&
                              StripeColors != null && StripeColors.Any();

            if (canAnimate && !_timer.IsRunning)
            {
                _timer.Start();
                IsVisible = true;
            }
            else if (!canAnimate && _timer.IsRunning)
            {
                _timer.Stop();
                IsVisible = false;
            }
        }
    }
}
