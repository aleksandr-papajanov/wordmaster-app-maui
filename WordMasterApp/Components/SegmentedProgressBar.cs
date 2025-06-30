using DynamicData;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMasterApp.Helpers;

namespace WordMasterApp.Components
{
    public class SegmentedProgressBar : ContentView
    {
        public static readonly BindableProperty StartColorProperty =
            BindableProperty.Create(nameof(StartColor), typeof(Color), typeof(SegmentedProgressBar), Colors.Transparent,
                propertyChanged: (bindable, _, newValue) =>
                {
                    if (bindable is not SegmentedProgressBar control)
                        return;

                    control.BuildSegments();
                });

        public static readonly BindableProperty EndColorProperty =
            BindableProperty.Create(nameof(EndColor), typeof(Color), typeof(SegmentedProgressBar), Colors.Transparent,
                propertyChanged: (bindable, _, newValue) =>
                {
                    if (bindable is not SegmentedProgressBar control)
                        return;

                    control.BuildSegments();
                });

        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create(nameof(Progress), typeof(double), typeof(SegmentedProgressBar), 0.0,
                propertyChanged: (bindable, _, newValue) =>
                {
                    if (bindable is not SegmentedProgressBar control)
                        return;

                    control.BuildSegments();
                });
        
        public static readonly BindableProperty SegmentsProperty =
            BindableProperty.Create(nameof(Segments), typeof(int), typeof(SegmentedProgressBar), 5,
                propertyChanged: (bindable, _, newValue) =>
                {
                    if (bindable is not SegmentedProgressBar control)
                        return;

                    control.BuildSegments();
                });

        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set => SetValue(StartColorProperty, value);
        }

        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set => SetValue(EndColorProperty, value);
        }

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }
        
        public int Segments
        {
            get => (int)GetValue(SegmentsProperty);
            set => SetValue(SegmentsProperty, value);
        }

        public SegmentedProgressBar()
        {
            BuildSegments();
        }

        private void BuildSegments()
        {
            var position = Segments * Progress;

            var grid = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection()
            };

            for (int i = 0; i < Segments; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                var segmentPosition = (float)i / Segments;
                var interpolatedColor = StartColor.ColorLerp(EndColor, segmentPosition).WithAlpha(i < position ? 1.0f : 0.2f);
                
                var segment = new Border
                {
                    Background = interpolatedColor,
                    StrokeThickness = i < position ? 2 : 6,
                    StrokeShape = new RoundRectangle { CornerRadius = 5 },
                };

                grid.SetColumn(segment, i);
                grid.Children.Add(segment);
            }

            Content = grid;
        }
    }
}
