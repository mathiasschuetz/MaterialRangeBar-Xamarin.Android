/*
 * Copyright 2014, Appyvet, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this work except in compliance with the License.
 * You may obtain a copy of the License in the LICENSE file, or at:
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" 
 * BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language 
 * governing permissions and limitations under the License. 
 */

using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Material_Range_Bar_Wrapper.Interfaces;

namespace Material_Range_Bar_Wrapper.Views
{
    /// <summary>
    ///     Represents a thumb in the RangeBar slider. This is the handle for the slider
    ///     that is pressed and slid.
    /// </summary>
    public class PinView : View
    {
        #region fields

        // The radius (in dp) of the touchable area around the thumb. We are basing
        // this value off of the recommended 48dp Rhythm. See:
        // http://developer.android.com/design/style/metrics-grids.html#48dp-rhythm
        private static readonly float _minimumTargetRadiusDp = 24;

        // Sets the default values for radius, normal, pressed if circle is to be
        // drawn but no value is given.
        private static readonly float _defaultThumbRadiusDp = 14;

        // Member Variables ////////////////////////////////////////////////////////

        // Radius (in pixels) of the touch area of the thumb.
        private float _targetRadiusPx;

        // Indicates whether this thumb is currently pressed and active.
        private bool _isPressed;

        // The y-position of the thumb in the parent view. This should not change.
        private float _y;

        // The current x-position of the thumb in the parent view.
        private float _x;

        // mPaint to draw the thumbs if attributes are selected

        private Paint _textPaint;

        private Drawable _pin;

        private string _value;

        // Radius of the new thumb if selected
        private int _pinRadiusPx;

        private ColorFilter _pinFilter;

        private float _pinPadding;

        private float _textYPadding;

        private readonly Rect _bounds = new Rect();

        private Resources _res;

        private float _density;

        private Paint _circlePaint;

        private float _circleRadiusPx;

        private IRangeBarFormatter _formatter;

        private float _minPinFont = RangeBar.DefaultMinPinFontSp;

        private float _maxPinFont = RangeBar.DefaultMaxPinFontSp;

        private bool _pinsAreTemporary;

        private bool _hasBeenPressed;

        #endregion

        #region properties

        /// <summary>Indicates whether the view is currently in pressed state.</summary>
        /// <value>To be added.</value>
        /// <remarks>
        ///     <para tool="javadoc-to-mdoc: Get method documentation">
        ///         <format type="text/html">
        ///             <b>Get method documentation</b>
        ///             <a href="http://developer.android.com/reference/android/view/View.html#isPressed()" target="_blank">
        ///                 [Android
        ///                 Documentation]
        ///             </a>
        ///             <br />
        ///         </format>
        ///         Indicates whether the view is currently in pressed state. Unless
        ///         <c>
        ///             <see cref="P:Android.Views.View.Pressed" />
        ///         </c>
        ///         is explicitly called, only clickable views can enter
        ///         the pressed state.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc: Set method documentation">
        ///         <format type="text/html">
        ///             <b>Set method documentation</b>
        ///             <a href="http://developer.android.com/reference/android/view/View.html#setPressed(boolean)" target="_blank">
        ///                 [Android
        ///                 Documentation]
        ///             </a>
        ///             <br />
        ///         </format>
        ///         Sets the pressed state for this view.
        ///     </para>
        /// </remarks>
        /// <since version="Added in API level 1" />
        /// <altmember cref="P:Android.Views.View.Pressed" />
        /// <altmember cref="P:Android.Views.View.Clickable" />
        /// <altmember cref="P:Android.Views.View.Clickable" />
        /// <altmember cref="P:Android.Views.View.Clickable" />
        /// <altmember cref="P:Android.Views.View.Clickable" />
        public override bool Pressed => this._isPressed;

        #endregion

        #region ctor

        public PinView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public PinView(Context context) : base(context)
        {
        }

        public PinView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public PinView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public PinView(Context context, IRangeBarFormatter formatter) : base(context)
        {
            this._formatter = formatter;
        }

        #endregion

        #region methods

        public void Init(Context context,
            float y,
            float pinRadiusDp,
            int pinColor,
            Color textColor,
            float circleRadius,
            Color circleColor,
            float minFont,
            float maxFont,
            bool pinsAreTemporary)
        {
            this._res = context.Resources;
            this._pin = ContextCompat.GetDrawable(context, Resource.Drawable.rotate);

            this._density = this.Resources.DisplayMetrics.Density;
            this._minPinFont = minFont / this._density;
            this._maxPinFont = maxFont / this._density;
            this._pinsAreTemporary = pinsAreTemporary;

            this._pinPadding = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 15, this._res.DisplayMetrics);
            this._circleRadiusPx = circleRadius;
            this._textYPadding = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 3.5f, this._res.DisplayMetrics);

            // If one of the attributes are set, but the others aren't, set the
            // attributes to default
            if (Math.Abs(pinRadiusDp - -1) < 0)
            {
                this._pinRadiusPx =
                    (int)
                    TypedValue.ApplyDimension(ComplexUnitType.Dip, _defaultThumbRadiusDp, this._res.DisplayMetrics);
            }
            else
            {
                this._pinRadiusPx =
                    (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, pinRadiusDp, this._res.DisplayMetrics);
            }

            // Set text size in px from dp
            var textSize = (int) TypedValue.ApplyDimension(ComplexUnitType.Sp, 15, this._res.DisplayMetrics);

            // Creates the paint and sets the Paint values
            this._textPaint = new Paint
            {
                Color = textColor,
                AntiAlias = true,
                TextSize = textSize
            };

            // Creates the paint and sets the Paint values
            this._circlePaint = new Paint
            {
                Color = circleColor,
                AntiAlias = true
            };

            //Color filter for the selection pin
            this._pinFilter = new LightingColorFilter(pinColor, pinColor);

            // Sets the minimum touchable area, but allows it to expand based on
            // image size
            var targetRadius = (int) Math.Max(_minimumTargetRadiusDp, this._pinRadiusPx);

            this._targetRadiusPx = TypedValue.ApplyDimension(ComplexUnitType.Dip, targetRadius, this._res.DisplayMetrics);

            this._y = y;
        }

        /// <summary>The visual x position of this view, in pixels.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>
        ///     <para tool="javadoc-to-mdoc">
        ///         The visual x position of this view, in pixels. This is equivalent to the
        ///         <c>
        ///             <see cref="P:Android.Views.View.TranslationX" />
        ///         </c>
        ///         property plus the current
        ///         <c>
        ///             <see cref="P:Android.Views.View.Left" />
        ///         </c>
        ///         property.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc">
        ///         <format type="text/html">
        ///             <a href="http://developer.android.com/reference/android/view/View.html#getX()" target="_blank">
        ///                 [Android
        ///                 Documentation]
        ///             </a>
        ///         </format>
        ///     </para>
        /// </remarks>
        /// <since version="Added in API level 11" />
        public override float GetX()
        {
            return this._x;
        }

        /// <param name="x">The visual x position of this view, in pixels.</param>
        /// <summary>Sets the visual x position of this view, in pixels.</summary>
        /// <remarks>
        ///     <para tool="javadoc-to-mdoc">
        ///         Sets the visual x position of this view, in pixels. This is equivalent to setting the
        ///         <c>
        ///             <see cref="P:Android.Views.View.TranslationX" />
        ///         </c>
        ///         property to be the difference between
        ///         the x value passed in and the current
        ///         <c>
        ///             <see cref="P:Android.Views.View.Left" />
        ///         </c>
        ///         property.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc">
        ///         <format type="text/html">
        ///             <a href="http://developer.android.com/reference/android/view/View.html#setX(float)" target="_blank">
        ///                 [Android
        ///                 Documentation]
        ///             </a>
        ///         </format>
        ///     </para>
        /// </remarks>
        /// <since version="Added in API level 11" />
        public override void SetX(float x)
        {
            this._x = x;
        }

        /// <summary>
        ///     Set the value of the pin
        /// </summary>
        /// <param name="x">String value of the pin</param>
        public void SetXValue(string x)
        {
            this._value = x;
        }

        /// <summary>
        ///     Set size of the pin and padding for use when animating pin enlargement on press
        /// </summary>
        /// <param name="size">the size of the pin radius</param>
        /// <param name="padding">the size of the padding</param>
        public void SetSize(float size, float padding)
        {
            this._pinPadding = (int) padding;
            this._pinRadiusPx = (int) size;
            this.Invalidate();
        }

        /// <summary>
        ///     Sets the state of the pin to pressed
        /// </summary>
        public void Press()
        {
            this._isPressed = true;
            this._hasBeenPressed = true;
        }

        /// <summary>
        ///     Release the pin, sets pressed state to false
        /// </summary>
        public void Release()
        {
            this._isPressed = false;
        }

        /// <summary>
        ///     Determines if the input coordinate is close enough to this thumb to
        ///     consider it a press.
        /// </summary>
        /// <param name="x">the x-coordinate of the user touch</param>
        /// <param name="y">the y-coordinate of the user touch</param>
        /// <returns>
        ///     true if the coordinates are within this thumb's target area;
        ///     false otherwise
        /// </returns>
        public bool IsInTargetZone(float x, float y)
        {
            return (Math.Abs(x - this._x) <= this._targetRadiusPx
                    && Math.Abs(y - this._y + this._pinPadding) <= this._targetRadiusPx);
        }

        /// <param name="canvas">The Canvas to which the View is rendered.</param>
        /// <summary>Manually render this view (and all of its children) to the given Canvas.</summary>
        /// <remarks>
        ///     <para tool="javadoc-to-mdoc">
        ///         Manually render this view (and all of its children) to the given Canvas.
        ///         The view must have already done a full layout before this function is
        ///         called.  When implementing a view, implement
        ///         <c>
        ///             <see cref="M:Android.Views.View.OnDraw(Android.Graphics.Canvas)" />
        ///         </c>
        ///         instead of overriding this method.
        ///         If you do need to override this method, call the superclass version.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc">
        ///         <format type="text/html">
        ///             <a href="http://developer.android.com/reference/android/view/View.html#draw(android.graphics.Canvas)"
        ///                 target="_blank">
        ///                 [Android Documentation]
        ///             </a>
        ///         </format>
        ///     </para>
        /// </remarks>
        /// <since version="Added in API level 1" />
        public override void Draw(Canvas canvas)
        {
            canvas.DrawCircle(this._x, this._y, this._circleRadiusPx, this._circlePaint);

            //Draw pin if pressed
            if (this._pinRadiusPx > 0 && (this._hasBeenPressed || !this._pinsAreTemporary))
            {
                this._bounds.Set((int) this._x - this._pinRadiusPx,
                    (int) this._y - (this._pinRadiusPx * 2) - (int) this._pinPadding,
                    (int) this._x + this._pinRadiusPx, (int) this._y - (int) this._pinPadding);
                this._pin.Bounds = this._bounds;
                var text = this._value;

                if (this._formatter != null)
                {
                    text = this._formatter.Format(text);
                }

                this.CalibrateTextSize(this._textPaint, text, this._bounds.Width());
                this._textPaint.GetTextBounds(text, 0, text.Length, this._bounds);
                this._textPaint.TextAlign = Paint.Align.Center;
                this._pin.SetColorFilter(this._pinFilter);
                this._pin.Draw(canvas);
                canvas.DrawText(text, this._x, this._y - this._pinRadiusPx - this._pinPadding + this._textYPadding,
                    this._textPaint);
            }

            base.Draw(canvas);
        }

        /// <summary>
        ///     Set text size based on available pin width.
        /// </summary>
        /// <param name="paint"></param>
        /// <param name="text"></param>
        /// <param name="boxWidth"></param>
        private void CalibrateTextSize(Paint paint, string text, float boxWidth)
        {
            paint.TextSize = 10;

            var textSize = paint.MeasureText(text);
            var estimatedFontSize = boxWidth * 8 / textSize / this._density;

            if (estimatedFontSize < this._minPinFont)
            {
                estimatedFontSize = this._minPinFont;
            }
            else if (estimatedFontSize > this._maxPinFont)
            {
                estimatedFontSize = this._maxPinFont;
            }
            paint.TextSize = (estimatedFontSize * this._density);
        }

        public void SetFormatter(IRangeBarFormatter mFormatter)
        {
            this._formatter = mFormatter;
        }

        #endregion
    }
}