using System.Collections.ObjectModel;

namespace WordMasterApp.Components.LoadingStripe
{
    public class LoadingStripeDrawable : IDrawable
    {
        private readonly List<StripeAnimation> _animations = new List<StripeAnimation>();

        public void UpdateAnimations(IList<Color> stripeColors, StripeAnimationAlignment alignment)
        {
            int i = 0;
            for (; i < stripeColors.Count; i++)
            {
                if (i < _animations.Count)
                {
                    _animations[i].Color = stripeColors[i];
                    _animations[i].Alignment = alignment;
                }
                else
                {
                    _animations.Add(new StripeAnimation(stripeColors[i], alignment));
                }
            }

            if (i < _animations.Count)
            {
                _animations.RemoveRange(i, _animations.Count - i);
            }
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            foreach (var animation in _animations)
            {
                animation.Update(canvas, dirtyRect);
            }
        }
    }
}