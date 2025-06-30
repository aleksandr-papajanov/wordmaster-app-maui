internal class StripeAnimation
{
    private static readonly Random _random = new Random();

    
    private readonly float _pulseAngleStep = Rnd(0.03f, 0.13f);
    private readonly float _pulseFactorSmooth = Rnd(0.8f, 0.9f);
    private readonly float _startOffset = Rnd(0.3f, 0.7f);
    private readonly int _blurLayers = 10;
    private readonly float _stepOffset = 8 * Rnd(0.75f, 1.5f);

    private int _step = 0;
    private ICanvas _canvas;
    private RectF _area;

    public Color Color { get; set; } = Colors.Transparent;
    public StripeAnimationAlignment Alignment { get; set; }

    private float CurrentOffset => Normalize(_startOffset + _step * _stepOffset, 0, _area.Width);
    private float PulseAngle => Normalize(_step * _pulseAngleStep, 0,  (float)Math.PI * 2); // phase shift for sine wave
    private float PulseFactor => 1.0f + _pulseFactorSmooth * (float)Math.Sin(PulseAngle); // add 1 to move sin to positive range and limit range to 0.8 for smoother effect
    private int BlurLayers => (int)(PulseFactor * _pulseFactorSmooth * _blurLayers);

    public StripeAnimation(Color color, StripeAnimationAlignment alignment)
    {
        _canvas = null!; // will be set in Update method
        Color = color;
        Alignment = alignment;
    }


    public void Update(ICanvas canvas, RectF area)
    {
        // ToDo: check paddings and centering behavior
        _canvas = canvas;
        _area = area;

        var padding = _blurLayers; // padding around the area

        //_area.Height -= padding * 2;
        //_area.Height = _area.Height < 1 ? 1 : _area.Height;
        _area.Height = 2;

        for (int i = BlurLayers; i >= 0; i--)
        {
            var alphaShift = 1f / (i + 1); // the farther, the more transparent
            var blurShift = i * PulseFactor;

            for (var x = CurrentOffset - _area.Width - blurShift; x < _area.Width + blurShift; x += _area.Width)
            {
                var gradient = new LinearGradientPaint([
                    new PaintGradientStop(0.3f,  Colors.Transparent),
                    new PaintGradientStop(0.5f, Color.WithAlpha(alphaShift * PulseFactor)),
                    new PaintGradientStop(0.7f, Colors.Transparent)
                ])
                {
                    StartPoint = new PointF(0, 0),
                    EndPoint = new PointF(1, 0)
                };

                var alingmentShift = padding * Alignment switch
                {
                    StripeAnimationAlignment.Center => 1,
                    StripeAnimationAlignment.Bottom => 2,
                    _ => 0
                };

                var rect = new RectF(
                    x - blurShift / 2,  // сдвигаем в рабочую область
                    -blurShift / 2 + alingmentShift,
                    _area.Width + blurShift,
                    _area.Height + blurShift);

                _canvas.SetFillPaint(gradient, rect);
                _canvas.FillRectangle(rect);
            }
        }

        _step++;
    }

    public static float Normalize(float value, float min, float max)
    {
        float range = max - min;
        float result = (value - min) % range;
        if (result < 0)
            result += range;
        return result + min;
    }

    private static float Rnd(float min = 0f, float max = 1f)
    {
        return (float)(_random.NextDouble() * (max - min) + min);
    }
}