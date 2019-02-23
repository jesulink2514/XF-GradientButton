using NControl.Abstractions;
using NGraphics;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Color = Xamarin.Forms.Color;
using Point = NGraphics.Point;
using TextAlignment = Xamarin.Forms.TextAlignment;

namespace DemoGradient.Controls
{
    public class GradientButton : NControlView
    {
        public GradientButton()
        {
            HeightRequest = 44;
            WidthRequest = 100;

            _label = new Label
            {
                Text = Text,
                TextColor = TextColor,
                FontSize = 17,
                BackgroundColor = Color.Transparent,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            _background = new NControlView
            {
                DrawingFunction = (canvas, rect) =>
                {
                    var brush = new LinearGradientBrush(
                        Point.Zero,
                        Point.OneX,
                        StartColor.ToNColor(),
                        EndColor.ToNColor());

                    var curveSize = BorderRadius;
                    var width = rect.Width;
                    var height = rect.Height;

                    canvas.DrawPath(new PathOp[]{
                        new MoveTo(curveSize, 0),
                        // Top Right corner
                        new LineTo(width-curveSize, 0),
                        new CurveTo(
                            new Point(width-curveSize, 0),
                            new Point(width, 0),
                            new Point(width, curveSize)
                        ),
                        new LineTo(width, height-curveSize),
                        // Bottom right corner
                        new CurveTo(
                            new Point(width, height-curveSize),
                            new Point(width, height),
                            new Point(width-curveSize, height)
                        ),
                        new LineTo(curveSize, height),
                        // Bottom left corner
                        new CurveTo(
                            new Point(curveSize, height),
                            new Point(0, height),
                            new Point(0, height-curveSize)
                        ),
                        new LineTo(0, curveSize),
                        new CurveTo(
                            new Point(0, curveSize),
                            new Point(0, 0),
                            new Point(curveSize, 0)
                        ),
                        new ClosePath()
                    }, null, brush);


                }
            };

            Content = new Grid
            {
                Children = {
                    _background,
                    _label
                }
            };
        }

        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set
            {
                SetValue(StartColorProperty, value);
                Invalidate();
            }
        }

        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set
            {
                SetValue(EndColorProperty, value);
                Invalidate();
            }
        }

        public static BindableProperty StartColorProperty =
            BindableProperty.Create(nameof(StartColor), typeof(Color), typeof(GradientButton), defaultBindingMode: BindingMode.OneWay,
                propertyChanged: (b, o, n) =>
                {
                    var ctrl = (GradientButton)b;
                    ctrl.StartColor = (Color)n;
                });
        public static BindableProperty EndColorProperty =
            BindableProperty.Create(nameof(EndColor), typeof(Color), typeof(GradientButton), defaultBindingMode: BindingMode.OneWay,
                propertyChanged: (b, o, n) =>
                {
                    var ctrl = (GradientButton)b;
                    ctrl.EndColor = (Color)n;
                });

        public int BorderRadius
        {
            get => (int)GetValue(BorderRadiusProperty);
            set
            {
                SetValue(BorderRadiusProperty, value);
                Invalidate();
            }
        }
        public const int DefaultRadius = 40;
        public static BindableProperty BorderRadiusProperty =
            BindableProperty.Create(nameof(BorderRadius), typeof(int), typeof(GradientButton), DefaultRadius, BindingMode.OneTime,
                propertyChanged: (b, o, n) =>
                {
                    var ctrl = (GradientButton)b;
                    ctrl.BorderRadius = (int)n;
                });

        public string Text
        {
            get => GetValue(TextProperty) as string;
            set
            {
                SetValue(TextProperty, value);
                _label.Text = value;
                Invalidate();
            }
        }

        public static BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(GradientButton), defaultBindingMode: BindingMode.OneWay,
                propertyChanged: (b, o, n) =>
                {
                    var ctrl = (GradientButton)b;
                    ctrl.Text = (string)n;
                });

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set
            {
                SetValue(TextColorProperty, value);
                _label.TextColor = value;
                Invalidate();
            }
        }

        public static BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(GradientButton), defaultBindingMode: BindingMode.OneWay,
                propertyChanged: (b, o, n) =>
                {
                    var ctrl = (GradientButton)b;
                    ctrl.TextColor = (Color)n;
                });

        public static BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(GradientButton), defaultBindingMode: BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (GradientButton)bindable;
                    ctrl.Command = (ICommand)newValue;
                });

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(GradientButton), defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (GradientButton)bindable;
                    ctrl.CommandParameter = newValue;
                });

        private readonly Label _label;
        private readonly NControlView _background;

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public override bool TouchesBegan(IEnumerable<Point> points)
        {
            base.TouchesBegan(points);
            this.ScaleTo(0.96, 65, Easing.CubicInOut);
            return true;
        }

        public override bool TouchesCancelled(IEnumerable<Point> points)
        {
            base.TouchesCancelled(points);
            this.ScaleTo(1.0, 65, Easing.CubicInOut);
            return true;
        }

        public override bool TouchesEnded(IEnumerable<Point> points)
        {
            base.TouchesEnded(points);
            this.ScaleTo(1.0, 65, Easing.CubicInOut);
            if (Command != null && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);

            return true;
        }
    }

    public static class ColorHelpers
    {
        public static NGraphics.Color ToNColor(this Color color)
        {
            return new NGraphics.Color(color.R, color.G, color.B, color.A);
        }
    }
}
