using Android.Animation;
using Android.Views;
using Java.Lang;
using Material_Range_Bar_Wrapper.Views;

namespace Material_Range_Bar_Wrapper.Listener
{
    internal class ReleasePinListener : Object, ValueAnimator.IAnimatorUpdateListener
    {
        #region fields

        private readonly View _view;
        private readonly PinView _thumb;

        #endregion

        #region properties

        public float PinPadding { private get; set; }

        public float ThumbRadius { private get; set; }

        #endregion

        #region ctor

        public ReleasePinListener(View view, PinView thumb)
        {
            this._view = view;
            this._thumb = thumb;
        }

        #endregion

        #region methods

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            this.ThumbRadius = (float) (animation.AnimatedValue);
            this._thumb.SetSize(this.ThumbRadius, this.PinPadding - this.PinPadding * animation.AnimatedFraction);
            this._view.Invalidate();
        }

        #endregion
    }
}