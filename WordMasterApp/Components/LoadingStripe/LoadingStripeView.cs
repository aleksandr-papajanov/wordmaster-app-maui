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
        
        public static readonly BindableProperty AlignmentProperty =
            BindableProperty.Create(nameof(Alignment), typeof(StripeAnimationAlignment), typeof(LoadingStripeView), StripeAnimationAlignment.Center,
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
        
        public StripeAnimationAlignment Alignment
        {
            get => (StripeAnimationAlignment)GetValue(AlignmentProperty);
            set => SetValue(AlignmentProperty, value);
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
                LoadingStripeType.HttpRequest => new List<Color> { ThemeExtentions.GetColor("Primary"), ThemeExtentions.GetColor("Secondary"), ThemeExtentions.GetColor("PrimaryVariant"), ThemeExtentions.GetColor("Background") },
                LoadingStripeType.UIInteraction => new List<Color> { ThemeExtentions.GetColor("SecondaryVariant"), ThemeExtentions.GetColor("SecondaryVariant"), ThemeExtentions.GetColor("Background") },
                LoadingStripeType.Error => new List<Color> { ThemeExtentions.GetColor("Error"), ThemeExtentions.GetColor("Error"), ThemeExtentions.GetColor("Background") },
                LoadingStripeType.Success => new List<Color> { ThemeExtentions.GetColor("Secondary"), ThemeExtentions.GetColor("Secondary"), ThemeExtentions.GetColor("Background") },
                _ => new List<Color> { Colors.Transparent }
            };

            _drawable.UpdateAnimations(StripeColors, Alignment);
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
