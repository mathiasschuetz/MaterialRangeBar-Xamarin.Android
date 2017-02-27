/*
 * Copyright 2013, Edmodo, Inc. 
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
using System.Collections.Generic;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Java.Lang;
using Material_Range_Bar_Wrapper.Interfaces;
using Material_Range_Bar_Wrapper.Listener;
using Newtonsoft.Json;
using Math = System.Math;

namespace Material_Range_Bar_Wrapper.Views
{
    /*
     * Copyright 2015, Appyvet, Inc.
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

    /// <summary>
    ///     The MaterialRangeBar is a single or double-sided version of a <see cref="Android.Widget.SeekBar" />
    ///     with discrete values. Whereas the thumb for the SeekBar can be dragged to any
    ///     position in the bar, the RangeBar only allows its thumbs to be dragged to
    ///     discrete positions (denoted by tick marks) in the bar. When released, a
    ///     RangeBar thumb will snap to the nearest tick mark.
    ///     This version is forked from edmodo range bar
    ///     https://github.com/edmodo/range-bar.git
    ///     Clients of the RangeBar can attach a
    ///     <see cref="IOnRangeBarChangeListener" />  to be notified when the pins
    ///     have
    ///     been moved.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RangeBar : View, RangeBar.IPinTextFormatter
    {
        #region fields

        // Member Variables ////////////////////////////////////////////////////////

        private const string RangeBarTag = "RangeBar";

        // Default values for variables
        private const float DefaultTickStart = 0;
        private const float DefaultTickEnd = 5;
        private const float DefaultTickInterval = 1;
        private const float DefaultTickHeightDp = 1;
        private const float DefaultPinPaddingDp = 16;
        public const float DefaultMinPinFontSp = 8;
        public const float DefaultMaxPinFontSp = 24;
        private const float DefaultBarWeightPx = 2;
        private static readonly Color DefaultBarColor = Color.LightGray;
        private static readonly Color DefaultTextColor = Color.White;
        private static readonly Color DefaultTickColor = Color.Black;
        // Corresponds to material indigo 500.
        private static readonly Color DefaultPinColor = Color.ParseColor("#ff3f51b5");
        private const float DefaultConnectingLineWeightPx = 4;
        // Corresponds to material indigo 500.
        private static readonly Color DefaultConnectingLineColor = Color.ParseColor("#ff3f51b5");
        private const float DefaultExpandedPinRadiusDp = 12;
        private const float DefaultCircleSizeDp = 5;
        private const float DefaultBarPaddingBottomDp = 24;
        // Instance variables for all of the customizable attributes
        private float _mTickHeightDp = DefaultTickHeightDp;
        private float _mTickStart = DefaultTickStart;
        private float _mTickEnd = DefaultTickEnd;
        private float _mTickInterval = DefaultTickInterval;
        private float _mBarWeight = DefaultBarWeightPx;
        private Color _mBarColor = DefaultBarColor;
        private Color _mPinColor = DefaultPinColor;
        private Color _mTextColor = DefaultTextColor;
        private float _mConnectingLineWeight = DefaultConnectingLineWeightPx;
        private Color _mConnectingLineColor = DefaultConnectingLineColor;
        private float _mThumbRadiusDp = DefaultExpandedPinRadiusDp;
        private Color _mTickColor = DefaultTickColor;
        private float _mExpandedPinRadius = DefaultExpandedPinRadiusDp;
        private Color _mCircleColor = DefaultConnectingLineColor;
        private float _mCircleSize = DefaultCircleSizeDp;
        private float _mMinPinFont = DefaultMinPinFontSp;
        private float _mMaxPinFont = DefaultMaxPinFontSp;
        // setTickCount only resets indices before a thumb has been pressed or a
        // setThumbIndices() is called, to correspond with intended usage
        private bool _mFirstSetTickCount = true;
        private const int MDefaultWidth = 500;
        private const int MDefaultHeight = 150;
        private int _mTickCount;
        private PinView _mLeftThumb;
        private PinView _mRightThumb;
        private Bar _mBar;
        private ConnectingLine _mConnectingLine;
        private IOnRangeBarChangeListener _mListener;
        private IOnRangeBarTextListener _mPinTextListener;
        private Dictionary<float, string> _mTickMap;
        private int _mLeftIndex;
        private int _mRightIndex;
        private bool _mIsRangeBar = true;
        private float _mPinPadding = DefaultPinPaddingDp;
        private float _mBarPaddingBottom = DefaultBarPaddingBottomDp;
        private Color _mActiveConnectingLineColor;
        private Color _mActiveBarColor;
        private Color _mActiveTickColor;
        private Color _mActiveCircleColor;
        //Used for ignoring vertical move
        private int _mDiffX;
        private int _mDiffY;
        private float _mLastX;
        private float _mLastY;
        private IRangeBarFormatter _mFormatter;
        private bool _drawTicks = true;
        private bool _mArePinsTemporary = true;
        private IPinTextFormatter _mPinTextFormatter;
        private bool _enabled = true;

        #endregion

        #region ctor

        public RangeBar(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            this._mTickCount = (int) ((this._mTickEnd - this._mTickStart) / this._mTickInterval) + 1;
            this._mPinTextFormatter = this;
        }

        public RangeBar(Context context) : base(context)
        {
            this._mTickCount = (int) ((this._mTickEnd - this._mTickStart) / this._mTickInterval) + 1;
            this._mPinTextFormatter = this;
        }

        public RangeBar(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this._mTickCount = (int) ((this._mTickEnd - this._mTickStart) / this._mTickInterval) + 1;
            this._mPinTextFormatter = this;

            this.RangeBarInit(context, attrs);
        }

        public RangeBar(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            this._mTickCount = (int) ((this._mTickEnd - this._mTickStart) / this._mTickInterval) + 1;
            this._mPinTextFormatter = this;

            this.RangeBarInit(context, attrs);
        }

        #endregion

        #region methods

        /// <summary>
        ///     Hook allowing a view to generate a representation of its internal state
        ///     that can later be used to create a new instance with that same state.
        /// </summary>
        /// <returns>To be added.</returns>
        /// <remarks>
        ///     <para tool="javadoc-to-mdoc">
        ///         Hook allowing a view to generate a representation of its internal state
        ///         that can later be used to create a new instance with that same state.
        ///         This state should only contain information that is not persistent or can
        ///         not be reconstructed later. For example, you will never store your
        ///         current position on screen because that will be comPuted again when a
        ///         new instance of the view is placed in its view hierarchy.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc">
        ///         Some examples of things you may store here: the current cursor position
        ///         in a text view (but usually not the text itself since that is stored in a
        ///         content provider or other persistent storage), the currently selected
        ///         item in a list view.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc">
        ///         <format type="text/html">
        ///             <a href="http://developer.android.com/reference/android/view/View.html#onSaveInstanceState()"
        ///                 target="_blank">
        ///                 [Android Documentation]
        ///             </a>
        ///         </format>
        ///     </para>
        /// </remarks>
        /// <since version="Added in API level 1" />
        /// <altmember cref="M:Android.Views.View.OnRestoreInstanceState(Android.OS.IParcelable)" />
        /// <altmember cref="P:Android.Views.View.SaveEnabled" />
        protected override IParcelable OnSaveInstanceState()
        {
            var bundle = new Bundle();

            bundle.PutParcelable("instanceState", base.OnSaveInstanceState());

            bundle.PutInt("TICK_COUNT", this._mTickCount);
            bundle.PutFloat("TICK_START", this._mTickStart);
            bundle.PutFloat("TICK_END", this._mTickEnd);
            bundle.PutFloat("TICK_INTERVAL", this._mTickInterval);
            bundle.PutString("TICK_COLOR", JsonConvert.SerializeObject(this._mTickColor));

            bundle.PutFloat("TICK_HEIGHT_DP", this._mTickHeightDp);
            bundle.PutFloat("BAR_WEIGHT", this._mBarWeight);
            bundle.PutString("BAR_COLOR", JsonConvert.SerializeObject(this._mBarColor));
            bundle.PutFloat("CONNECTING_LINE_WEIGHT", this._mConnectingLineWeight);
            bundle.PutString("CONNECTING_LINE_COLOR", JsonConvert.SerializeObject(this._mConnectingLineColor));

            bundle.PutFloat("CIRCLE_SIZE", this._mCircleSize);
            bundle.PutString("CIRCLE_COLOR", JsonConvert.SerializeObject(this._mCircleColor));
            bundle.PutFloat("THUMB_RADIUS_DP", this._mThumbRadiusDp);
            bundle.PutFloat("EXPANDED_PIN_RADIUS_DP", this._mExpandedPinRadius);
            bundle.PutFloat("PIN_PADDING", this._mPinPadding);
            bundle.PutFloat("BAR_PADDING_BOTTOM", this._mBarPaddingBottom);
            bundle.PutBoolean("IS_RANGE_BAR", this._mIsRangeBar);
            bundle.PutBoolean("ARE_PINS_TEMPORARY", this._mArePinsTemporary);
            bundle.PutInt("LEFT_INDEX", this._mLeftIndex);
            bundle.PutInt("RIGHT_INDEX", this._mRightIndex);

            bundle.PutBoolean("FIRST_SET_TICK_COUNT", this._mFirstSetTickCount);

            bundle.PutFloat("MIN_PIN_FONT", this._mMinPinFont);
            bundle.PutFloat("MAX_PIN_FONT", this._mMaxPinFont);

            return bundle;
        }

        /// <param name="state">
        ///     The frozen state that had previously been returned by
        ///     <c>
        ///         <see cref="M:Android.Views.View.OnSaveInstanceState" />
        ///     </c>
        ///     .
        /// </param>
        /// <summary>
        ///     Hook allowing a view to re-apply a representation of its internal state that had previously
        ///     been generated by
        ///     <c>
        ///         <see cref="M:Android.Views.View.OnSaveInstanceState" />
        ///     </c>
        ///     .
        /// </summary>
        /// <remarks>
        ///     <para tool="javadoc-to-mdoc">
        ///         Hook allowing a view to re-apply a representation of its internal state that had previously
        ///         been generated by
        ///         <c>
        ///             <see cref="M:Android.Views.View.OnSaveInstanceState" />
        ///         </c>
        ///         . This function will never be called with a
        ///         null state.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc">
        ///         <format type="text/html">
        ///             <a
        ///                 href="http://developer.android.com/reference/android/view/View.html#onRestoreInstanceState(android.os.Parcelable)"
        ///                 target="_blank">
        ///                 [Android Documentation]
        ///             </a>
        ///         </format>
        ///     </para>
        /// </remarks>
        /// <since version="Added in API level 1" />
        /// <altmember cref="M:Android.Views.View.OnSaveInstanceState" />
        protected override void OnRestoreInstanceState(IParcelable state)
        {
            var state1 = state as Bundle;
            if (state1 != null)
            {
                var bundle = state1;

                this._mTickCount = bundle.GetInt("TICK_COUNT");
                this._mTickStart = bundle.GetFloat("TICK_START");
                this._mTickEnd = bundle.GetFloat("TICK_END");
                this._mTickInterval = bundle.GetFloat("TICK_INTERVAL");
                this._mTickColor = JsonConvert.DeserializeObject<Color>(bundle.GetString("TICK_COLOR"));
                this._mTickHeightDp = bundle.GetFloat("TICK_HEIGHT_DP");
                this._mBarWeight = bundle.GetFloat("BAR_WEIGHT");
                this._mBarColor = JsonConvert.DeserializeObject<Color>(bundle.GetString("BAR_COLOR"));
                this._mCircleSize = bundle.GetFloat("CIRCLE_SIZE");
                this._mCircleColor = JsonConvert.DeserializeObject<Color>(bundle.GetString("CIRCLE_COLOR"));
                this._mConnectingLineWeight = bundle.GetFloat("CONNECTING_LINE_WEIGHT");
                this._mConnectingLineColor =
                    JsonConvert.DeserializeObject<Color>(bundle.GetString("CONNECTING_LINE_COLOR"));

                this._mThumbRadiusDp = bundle.GetFloat("THUMB_RADIUS_DP");
                this._mExpandedPinRadius = bundle.GetFloat("EXPANDED_PIN_RADIUS_DP");
                this._mPinPadding = bundle.GetFloat("PIN_PADDING");
                this._mBarPaddingBottom = bundle.GetFloat("BAR_PADDING_BOTTOM");
                this._mIsRangeBar = bundle.GetBoolean("IS_RANGE_BAR");
                this._mArePinsTemporary = bundle.GetBoolean("ARE_PINS_TEMPORARY");

                this._mLeftIndex = bundle.GetInt("LEFT_INDEX");
                this._mRightIndex = bundle.GetInt("RIGHT_INDEX");
                this._mFirstSetTickCount = bundle.GetBoolean("FIRST_SET_TICK_COUNT");

                this._mMinPinFont = bundle.GetFloat("MIN_PIN_FONT");
                this._mMaxPinFont = bundle.GetFloat("MAX_PIN_FONT");

                this.SetRangePinsByIndices(this._mLeftIndex, this._mRightIndex);
                base.OnRestoreInstanceState((IParcelable) bundle.GetParcelable("instanceState"));
            }
            else
            {
                base.OnRestoreInstanceState(state);
            }
        }

        /// <param name="widthMeasureSpec">
        ///     horizontal space requirements as imposed by the parent.
        ///     The requirements are encoded with
        ///     <c>
        ///         <see cref="T:Android.Views.View+MeasureSpec" />
        ///     </c>
        ///     .
        /// </param>
        /// <param name="heightMeasureSpec">
        ///     vertical space requirements as imposed by the parent.
        ///     The requirements are encoded with
        ///     <c>
        ///         <see cref="T:Android.Views.View+MeasureSpec" />
        ///     </c>
        ///     .
        /// </param>
        /// <summary />
        /// <remarks>
        ///     <para tool="javadoc-to-mdoc" />
        ///     <para tool="javadoc-to-mdoc">
        ///         Measure the view and its content to determine the measured width and the
        ///         measured height. This method is invoked by
        ///         <c>
        ///             <see cref="M:Android.Views.View.Measure(System.Int32, System.Int32)" />
        ///         </c>
        ///         and
        ///         should be overriden by subclasses to provide accurate and efficient
        ///         measurement of their contents.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc">
        ///         <i>CONTRACT:</i> When overriding this method, you
        ///         <i>must</i> call
        ///         <c>
        ///             <see cref="M:Android.Views.View.SetMeasuredDimension(System.Int32, System.Int32)" />
        ///         </c>
        ///         to store the
        ///         measured width and height of this view. Failure to do so will trigger an
        ///         <c>IllegalStateException</c>, thrown by
        ///         <c>
        ///             <see cref="M:Android.Views.View.Measure(System.Int32, System.Int32)" />
        ///         </c>
        ///         . Calling the superclass'
        ///         <c>
        ///             <see cref="M:Android.Views.View.OnMeasure(System.Int32, System.Int32)" />
        ///         </c>
        ///         is a valid use.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc">
        ///         The base class implementation of measure defaults to the background size,
        ///         unless a larger size is allowed by the MeasureSpec. Subclasses should
        ///         override
        ///         <c>
        ///             <see cref="M:Android.Views.View.OnMeasure(System.Int32, System.Int32)" />
        ///         </c>
        ///         to provide better measurements of
        ///         their content.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc">
        ///         If this method is overridden, it is the subclass's responsibility to make
        ///         sure the measured height and width are at least the view's minimum height
        ///         and width (
        ///         <c>
        ///             <see cref="M:Android.Views.View.get_SuggestedMinimumHeight" />
        ///         </c>
        ///         and
        ///         <c>
        ///             <see cref="M:Android.Views.View.get_SuggestedMinimumWidth" />
        ///         </c>
        ///         ).
        ///     </para>
        ///     <para tool="javadoc-to-mdoc">
        ///         <format type="text/html">
        ///             <a href="http://developer.android.com/reference/android/view/View.html#onMeasure(int, int)" target="_blank">
        ///                 [Android
        ///                 Documentation]
        ///             </a>
        ///         </format>
        ///     </para>
        /// </remarks>
        /// <since version="Added in API level 1" />
        /// <altmember cref="P:Android.Views.View.MeasuredWidth" />
        /// <altmember cref="P:Android.Views.View.MeasuredHeight" />
        /// <altmember cref="M:Android.Views.View.SetMeasuredDimension(System.Int32, System.Int32)" />
        /// <altmember cref="M:Android.Views.View.get_SuggestedMinimumHeight" />
        /// <altmember cref="M:Android.Views.View.get_SuggestedMinimumWidth" />
        /// <altmember cref="M:Android.Views.View.MeasureSpec.GetMode(System.Int32)" />
        /// <altmember cref="M:Android.Views.View.MeasureSpec.GetSize(System.Int32)" />
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int width;
            int height;

            // Get measureSpec mode and size values.
            var measureWidthMode = MeasureSpec.GetMode(widthMeasureSpec);
            var measureHeightMode = MeasureSpec.GetMode(heightMeasureSpec);
            var measureWidth = MeasureSpec.GetSize(widthMeasureSpec);
            var measureHeight = MeasureSpec.GetSize(heightMeasureSpec);

            // The RangeBar width should be as large as possible.
            if (measureWidthMode == Android.Views.MeasureSpecMode.AtMost)
            {
                width = measureWidth;
            }
            else if (measureWidthMode == Android.Views.MeasureSpecMode.Exactly)
            {
                width = measureWidth;
            }
            else
            {
                width = MDefaultWidth;
            }

            // The RangeBar height should be as small as possible.
            if (measureHeightMode == Android.Views.MeasureSpecMode.AtMost)
            {
                height = Math.Min(MDefaultHeight, measureHeight);
            }
            else if (measureHeightMode == Android.Views.MeasureSpecMode.Exactly)
            {
                height = measureHeight;
            }
            else
            {
                height = MDefaultHeight;
            }

            this.SetMeasuredDimension(width, height);
        }

        /// <param name="w">Current width of this view.</param>
        /// <param name="h">Current height of this view.</param>
        /// <param name="oldw">Old width of this view.</param>
        /// <param name="oldh">Old height of this view.</param>
        /// <summary>This is called during layout when the size of this view has changed.</summary>
        /// <remarks>
        ///     <para tool="javadoc-to-mdoc">
        ///         This is called during layout when the size of this view has changed. If
        ///         you were just added to the view hierarchy, you're called with the old
        ///         values of 0.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc">
        ///         <format type="text/html">
        ///             <a href="http://developer.android.com/reference/android/view/View.html#onSizeChanged(int, int, int, int)"
        ///                 target="_blank">
        ///                 [Android Documentation]
        ///             </a>
        ///         </format>
        ///     </para>
        /// </remarks>
        /// <since version="Added in API level 1" />
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            var ctx = this.Context;

            // This is the initial point at which we know the size of the View.

            // Create the two thumb objects and position line in view
            var density = this.Resources.DisplayMetrics.Density;
            var expandedPinRadius = this._mExpandedPinRadius / density;

            var yPos = h - this._mBarPaddingBottom;
            if (this._mIsRangeBar)
            {
                this._mLeftThumb = new PinView(ctx);
                this._mLeftThumb.SetFormatter(this._mFormatter);
                this._mLeftThumb.Init(ctx, yPos, expandedPinRadius, this._mPinColor, this._mTextColor, this._mCircleSize,
                    this._mCircleColor, this._mMinPinFont, this._mMaxPinFont, this._mArePinsTemporary);
            }
            this._mRightThumb = new PinView(ctx);
            this._mRightThumb.SetFormatter(this._mFormatter);
            this._mRightThumb.Init(ctx, yPos, expandedPinRadius, this._mPinColor, this._mTextColor, this._mCircleSize,
                this._mCircleColor, this._mMinPinFont, this._mMaxPinFont, this._mArePinsTemporary);

            // Create the underlying bar.
            var marginLeft = Math.Max(this._mExpandedPinRadius, this._mCircleSize);

            var barLength = w - (2 *
                                 marginLeft)
                ;
            this._mBar = new Bar(ctx, marginLeft, yPos, barLength, this._mTickCount, this._mTickHeightDp,
                this._mTickColor,
                this._mBarWeight, this._mBarColor);

            // Initialize thumbs to the desired indices
            if (this._mIsRangeBar)
            {
                this._mLeftThumb.SetX(marginLeft + (this._mLeftIndex / (float) (this._mTickCount - 1)) *
                                      barLength)
                    ;
                this._mLeftThumb.SetXValue(this.GetPinValue(this._mLeftIndex));
            }
            this._mRightThumb.SetX(marginLeft + (this._mRightIndex / (float) (this._mTickCount - 1)) *
                                   barLength)
                ;
            this._mRightThumb.SetXValue(this.GetPinValue(this._mRightIndex));

            // Set the thumb indices.
            var newLeftIndex = this._mIsRangeBar ? this._mBar.GetNearestTickIndex(this._mLeftThumb) : 0;
            var newRightIndex = this._mBar.GetNearestTickIndex(this._mRightThumb);

            // Call the listener.
            if (newLeftIndex != this._mLeftIndex || newRightIndex != this._mRightIndex)
            {
                this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                    this.GetPinValue(this._mLeftIndex), this.GetPinValue(this._mRightIndex));
            }

            // Create the line connecting the two thumbs.
            this._mConnectingLine = new ConnectingLine(ctx, yPos, this._mConnectingLineWeight,
                this._mConnectingLineColor);
        }

        /// <param name="canvas">the canvas on which the background will be drawn</param>
        /// <summary>Implement this to do your drawing.</summary>
        /// <remarks>
        ///     <para tool="javadoc-to-mdoc">Implement this to do your drawing.</para>
        ///     <para tool="javadoc-to-mdoc">
        ///         <format type="text/html">
        ///             <a href="http://developer.android.com/reference/android/view/View.html#onDraw(android.graphics.Canvas)"
        ///                 target="_blank">
        ///                 [Android Documentation]
        ///             </a>
        ///         </format>
        ///     </para>
        /// </remarks>
        /// <since version="Added in API level 1" />
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            this._mBar.Draw(canvas);
            if (this._mIsRangeBar)
            {
                this._mConnectingLine.Draw(canvas, this._mLeftThumb, this._mRightThumb);
                if (this._drawTicks)
                {
                    this._mBar.DrawTicks(canvas);
                }
                this._mLeftThumb.Draw(canvas);
            }
            else
            {
                this._mConnectingLine.Draw(canvas, this.GetMarginLeft(), this._mRightThumb);
                if (this._drawTicks)
                {
                    this._mBar.DrawTicks(canvas);
                }
            }
            this._mRightThumb.Draw(canvas);
        }

        /// <param name="event">The motion event.</param>
        /// <summary>Implement this method to handle touch screen motion events.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>
        ///     <para tool="javadoc-to-mdoc">Implement this method to handle touch screen motion events.</para>
        ///     <para tool="javadoc-to-mdoc">
        ///         <format type="text/html">
        ///             <a
        ///                 href="http://developer.android.com/reference/android/view/View.html#onTouchEvent(android.view.MotionEvent)"
        ///                 target="_blank">
        ///                 [Android Documentation]
        ///             </a>
        ///         </format>
        ///     </para>
        /// </remarks>
        /// <since version="Added in API level 1" />
        public override bool OnTouchEvent(MotionEvent @event)
        {
            if (this.Enabled == false)
                return false;

            switch (@event.Action)
            {
                case MotionEventActions.Down:
                    this._mDiffX = 0;
                    this._mDiffY = 0;

                    this._mLastX = @event.GetX();
                    this._mLastY = @event.GetY();

                    this.OnActionDown(@event.GetX(), @event.GetY());

                    return true;
                case MotionEventActions.Up:
                    this.Parent.RequestDisallowInterceptTouchEvent(false);
                    this.OnActionUp(@event.GetX());
                    return true;
                case MotionEventActions.Cancel:
                    this.Parent.RequestDisallowInterceptTouchEvent(false);
                    this.OnActionUp(@event.GetX());
                    return true;
                case MotionEventActions.Move:
                    this.OnActionMove(@event.GetX());
                    this.Parent.RequestDisallowInterceptTouchEvent(true);
                    var curX = @event.GetX();
                    var curY = @event.GetY();
                    this._mDiffX += (int) Math.Abs(curX - this._mLastX);
                    this._mDiffY += (int) Math.Abs(curY - this._mLastY);
                    this._mLastX = curX;
                    this._mLastY = curY;

                    if (this._mDiffX < this._mDiffY)
                    {
                        //vertical touch
                        this.Parent.RequestDisallowInterceptTouchEvent(false);
                        return false;
                    }
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Sets a listener to receive notifications of changes to the RangeBar. This
        ///     will overwrite any existing set listeners.
        /// </summary>
        /// <param name="listener">
        ///     the RangeBar notification listener; null to remove any
        ///     existing listener
        /// </param>
        public void SetOnRangeBarChangeListener(IOnRangeBarChangeListener listener)
        {
            this._mListener = listener;
        }

        /// <summary>
        ///     Sets a listener to modify the text
        /// </summary>
        /// <param name="mPinTextListener">
        ///     the RangeBar pin text notification listener; null to remove any
        ///     existing listener
        /// </param>
        public void SetPinTextListener(IOnRangeBarTextListener mPinTextListener)
        {
            this._mPinTextListener = mPinTextListener;
        }


        public void SetFormatter(IRangeBarFormatter formatter)
        {
            this._mLeftThumb?.SetFormatter(formatter);

            this._mRightThumb?.SetFormatter(formatter);

            this._mFormatter = formatter;
        }

        public void SetDrawTicks(bool drawTicks)
        {
            this._drawTicks = drawTicks;
        }

        /// <summary>
        ///     Sets the start tick in the RangeBar.
        /// </summary>
        /// <param name="tickStart">Integer specifying the number of ticks.</param>
        public void SetTickStart(float tickStart)
        {
            var tickCount = (int) ((this._mTickEnd - tickStart) / this._mTickInterval) + 1;
            if (this.IsValidTickCount(tickCount))
            {
                this._mTickCount = tickCount;
                this._mTickStart = tickStart;

                // Prevents resetting the indices when creating new activity, but
                // allows it on the first setting.
                if (this._mFirstSetTickCount)
                {
                    this._mLeftIndex = 0;
                    this._mRightIndex = this._mTickCount - 1;

                    this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                        this.GetPinValue(this._mLeftIndex), this.GetPinValue(this._mRightIndex));
                }
                if (this.IndexOutOfRange(this._mLeftIndex, this._mRightIndex))
                {
                    this._mLeftIndex = 0;
                    this._mRightIndex = this._mTickCount - 1;

                    this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                        this.GetPinValue(this._mLeftIndex), this.GetPinValue(this._mRightIndex));
                }

                this.CreateBar();
                this.CreatePins();
            }
            else
            {
                Log.Error(RangeBarTag, "tickCount less than 2; invalid tickCount.");
                throw new IllegalArgumentException("tickCount less than 2; invalid tickCount.");
            }
        }

        /// <summary>
        ///     Sets the start tick in the RangeBar.
        /// </summary>
        /// <param name="tickInterval">Integer specifying the number of ticks.</param>
        public void SetTickInterval(float tickInterval)
        {
            var tickCount = (int) ((this._mTickEnd - this._mTickStart) / tickInterval) + 1;
            if (this.IsValidTickCount(tickCount))
            {
                this._mTickCount = tickCount;
                this._mTickInterval = tickInterval;

                // Prevents resetting the indices when creating new activity, but
                // allows it on the first setting.
                if (this._mFirstSetTickCount)
                {
                    this._mLeftIndex = 0;
                    this._mRightIndex = this._mTickCount - 1;

                    this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                        this.GetPinValue(this._mLeftIndex),
                        this.GetPinValue(this._mRightIndex));
                }
                if (this.IndexOutOfRange(this._mLeftIndex, this._mRightIndex))
                {
                    this._mLeftIndex = 0;
                    this._mRightIndex = this._mTickCount - 1;

                    this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                        this.GetPinValue(this._mLeftIndex),
                        this.GetPinValue(this._mRightIndex));
                }

                this.CreateBar();
                this.CreatePins();
            }
            else
            {
                Log.Error(nameof(this.SetTickInterval), "tickCount less than 2; invalid tickCount.");
                throw new IllegalArgumentException("tickCount less than 2; invalid tickCount.");
            }
        }

        /// <summary>
        ///     Sets the end tick in the RangeBar.
        /// </summary>
        /// <param name="tickEnd">Integer specifying the number of ticks.</param>
        public void SetTickEnd(float tickEnd)
        {
            var tickCount = (int) ((tickEnd - this._mTickStart) / this._mTickInterval) + 1;
            if (this.IsValidTickCount(tickCount))
            {
                this._mTickCount = tickCount;
                this._mTickEnd = tickEnd;

                // Prevents resetting the indices when creating new activity, but
                // allows it on the first setting.
                if (this._mFirstSetTickCount)
                {
                    this._mLeftIndex = 0;
                    this._mRightIndex = this._mTickCount - 1;

                    this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                        this.GetPinValue(this._mLeftIndex),
                        this.GetPinValue(this._mRightIndex));
                }
                if (this.IndexOutOfRange(this._mLeftIndex, this._mRightIndex))
                {
                    this._mLeftIndex = 0;
                    this._mRightIndex = this._mTickCount - 1;

                    this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                        this.GetPinValue(this._mLeftIndex),
                        this.GetPinValue(this._mRightIndex));
                }

                this.CreateBar();
                this.CreatePins();
            }
            else
            {
                Log.Error(RangeBarTag, "tickCount less than 2; invalid tickCount.");
                throw new IllegalArgumentException("tickCount less than 2; invalid tickCount.");
            }
        }

        /// <summary>
        ///     Sets the height of the ticks in the range bar.
        /// </summary>
        /// <param name="tickHeight">Float specifying the height of each tick mark in dp.</param>
        public void SetTickHeight(float tickHeight)
        {
            this._mTickHeightDp = tickHeight;
            this.CreateBar();
        }

        /// <summary>
        ///     Set the weight of the bar line and the tick lines in the range bar.
        /// </summary>
        /// <param name="barWeight">
        ///     Float specifying the weight of the bar and tick lines in
        ///     px.
        /// </param>
        public void SetBarWeight(float barWeight)
        {
            this._mBarWeight = barWeight;
            this.CreateBar();
        }

        /// <summary>
        ///     Set the color of the bar line and the tick lines in the range bar.
        /// </summary>
        /// <param name="barColor">Integer specifying the color of the bar line.</param>
        public void SetBarColor(Color barColor)
        {
            this._mBarColor = barColor;
            this.CreateBar();
        }

        /// <summary>
        ///     Set the color of the pins.
        /// </summary>
        /// <param name="pinColor">Integer specifying the color of the pin.</param>
        public void SetPinColor(Color pinColor)
        {
            this._mPinColor = pinColor;
            this.CreatePins();
        }

        /// <summary>
        ///     Set the color of the text within the pin.
        /// </summary>
        /// <param name="textColor">Integer specifying the color of the text in the pin.</param>
        public void SetPinTextColor(Color textColor)
        {
            this._mTextColor = textColor;
            this.CreatePins();
        }

        /// <summary>
        ///     Set if the view is a range bar or a seek bar.
        /// </summary>
        /// <param name="isRangeBar">bool - true sets it to rangebar, false to seekbar.</param>
        public void SetRangeBarEnabled(bool isRangeBar)
        {
            this._mIsRangeBar = isRangeBar;
            this.Invalidate();
        }

        /// <summary>
        ///     Set if the pins should dissapear after released
        /// </summary>
        /// <param name="arePinsTemporary">
        ///     bool - true if pins shoudl dissapear after released, false to
        ///     stay
        ///     drawn
        /// </param>
        public void SetTemporaryPins(bool arePinsTemporary)
        {
            this._mArePinsTemporary = arePinsTemporary;
            this.Invalidate();
        }

        /// <summary>
        ///     Set the color of the ticks.
        /// </summary>
        /// <param name="tickColor">Integer specifying the color of the ticks.</param>
        public void SetTickColor(Color tickColor)
        {
            this._mTickColor = tickColor;
            this.CreateBar();
        }

        /// <summary>
        ///     Set the color of the selector.
        /// </summary>
        /// <param name="selectorColor">Integer specifying the color of the ticks.</param>
        public void SetSelectorColor(Color selectorColor)
        {
            this._mCircleColor = selectorColor;
            this.CreatePins();
        }

        /// <summary>
        ///     Set the weight of the connecting line between the thumbs.
        /// </summary>
        /// <param name="connectingLineWeight">
        ///     Float specifying the weight of the connecting
        ///     line.
        /// </param>
        public void SetConnectingLineWeight(float connectingLineWeight)
        {
            this._mConnectingLineWeight = connectingLineWeight;
            this.CreateConnectingLine();
        }

        /// <summary>
        ///     Set the color of the connecting line between the thumbs.
        /// </summary>
        /// <param name="connectingLineColor">
        ///     Integer specifying the color of the connecting
        ///     line.
        /// </param>
        public void SetConnectingLineColor(Color connectingLineColor)
        {
            this._mConnectingLineColor = connectingLineColor;
            this.CreateConnectingLine();
        }

        /// <summary>
        ///     If this is set, the thumb images will be replaced with a circle of the
        ///     specified radius. Default width = 20dp.
        /// </summary>
        /// <param name="pinRadius">Float specifying the radius of the thumbs to be drawn.</param>
        public void SetPinRadius(float pinRadius)
        {
            this._mExpandedPinRadius = pinRadius;
            this.CreatePins();
        }

        /// <summary>
        ///     Gets the start tick.
        /// </summary>
        /// <returns>the start tick.</returns>
        public float GetTickStart()
        {
            return this._mTickStart;
        }

        /// <summary>
        ///     Gets the end tick.
        /// </summary>
        /// <returns>the end tick.</returns>
        public float GetTickEnd()
        {
            return this._mTickEnd;
        }

        /// <summary>
        ///     Gets the tick count.
        /// </summary>
        /// <returns>the tick count</returns>
        public int GetTickCount()
        {
            return this._mTickCount;
        }

        /// <summary>
        ///     Sets the location of the pins according by the supplied index.
        ///     Numbered from 0 to mTickCount - 1 from the left.
        /// </summary>
        /// <param name="leftPinIndex">Integer specifying the index of the left pin</param>
        /// <param name="rightPinIndex">Integer specifying the index of the right pin</param>
        public void SetRangePinsByIndices(int leftPinIndex, int rightPinIndex)
        {
            if (this.IndexOutOfRange(leftPinIndex, rightPinIndex))
            {
                Log.Error(nameof(this.SetRangePinsByIndices),
                    "Pin index left " + leftPinIndex + ", or right " + rightPinIndex
                    + " is out of bounds. Check that it is greater than the minimum ("
                    + this._mTickStart + ") and less than the maximum value ("
                    + this._mTickEnd + ")");
                throw new IllegalArgumentException(
                    "Pin index left " + leftPinIndex + ", or right " + rightPinIndex
                    + " is out of bounds. Check that it is greater than the minimum ("
                    + this._mTickStart + ") and less than the maximum value ("
                    + this._mTickEnd + ")");
            }
            if (this._mFirstSetTickCount)
            {
                this._mFirstSetTickCount = false;
            }
            this._mLeftIndex = leftPinIndex;
            this._mRightIndex = rightPinIndex;
            this.CreatePins();

            this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                this.GetPinValue(this._mLeftIndex),
                this.GetPinValue(this._mRightIndex));

            this.Invalidate();
            this.RequestLayout();
        }

        /// <summary>
        ///     Sets the location of pin according by the supplied index.
        ///     Numbered from 0 to mTickCount - 1 from the left.
        /// </summary>
        /// <param name="pinIndex">Integer specifying the index of the seek pin</param>
        public void SetSeekPinByIndex(int pinIndex)
        {
            if (pinIndex < 0 || pinIndex > this._mTickCount)
            {
                Log.Error(nameof(this.SetSeekPinByIndex),
                    "Pin index " + pinIndex
                    + " is out of bounds. Check that it is greater than the minimum ("
                    + 0 + ") and less than the maximum value ("
                    + this._mTickCount + ")");
                throw new IllegalArgumentException(
                    "Pin index " + pinIndex
                    + " is out of bounds. Check that it is greater than the minimum ("
                    + 0 + ") and less than the maximum value ("
                    + this._mTickCount + ")");
            }
            if (this._mFirstSetTickCount)
            {
                this._mFirstSetTickCount = false;
            }
            this._mRightIndex = pinIndex;
            this.CreatePins();

            this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                this.GetPinValue(this._mLeftIndex),
                this.GetPinValue(this._mRightIndex));
            this.Invalidate();
            this.RequestLayout();
        }

        /// <summary>
        ///     Sets the location of pins according by the supplied values.
        /// </summary>
        /// <param name="leftPinValue">Float specifying the index of the left pin</param>
        /// <param name="rightPinValue">Float specifying the index of the right pin</param>
        public void SetRangePinsByValue(float leftPinValue, float rightPinValue)
        {
            if (this.ValueOutOfRange(leftPinValue, rightPinValue))
            {
                Log.Error(nameof(this.SetRangePinsByValue),
                    "Pin value left " + leftPinValue + ", or right " + rightPinValue
                    + " is out of bounds. Check that it is greater than the minimum ("
                    + this._mTickStart + ") and less than the maximum value ("
                    + this._mTickEnd + ")");
                throw new IllegalArgumentException(
                    "Pin value left " + leftPinValue + ", or right " + rightPinValue
                    + " is out of bounds. Check that it is greater than the minimum ("
                    + this._mTickStart + ") and less than the maximum value ("
                    + this._mTickEnd + ")");
            }
            if (this._mFirstSetTickCount)
            {
                this._mFirstSetTickCount = false;
            }

            this._mLeftIndex = (int) ((leftPinValue - this._mTickStart) / this._mTickInterval);
            this._mRightIndex = (int) ((rightPinValue - this._mTickStart) / this._mTickInterval);
            this.CreatePins();

            this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                this.GetPinValue(this._mLeftIndex),
                this.GetPinValue(this._mRightIndex));
            this.Invalidate();
            this.RequestLayout();
        }

        /// <summary>
        ///     Sets the location of pin according by the supplied value.
        /// </summary>
        /// <param name="pinValue">Float specifying the value of the pin</param>
        public void SetSeekPinByValue(float pinValue)
        {
            if (pinValue > this._mTickEnd || pinValue < this._mTickStart)
            {
                Log.Error(nameof(this.SetSeekPinByValue),
                    "Pin value " + pinValue
                    + " is out of bounds. Check that it is greater than the minimum ("
                    + this._mTickStart + ") and less than the maximum value ("
                    + this._mTickEnd + ")");
                throw new IllegalArgumentException(
                    "Pin value " + pinValue
                    + " is out of bounds. Check that it is greater than the minimum ("
                    + this._mTickStart + ") and less than the maximum value ("
                    + this._mTickEnd + ")");
            }
            if (this._mFirstSetTickCount)
            {
                this._mFirstSetTickCount = false;
            }
            this._mRightIndex = (int) ((pinValue - this._mTickStart) / this._mTickInterval);
            this.CreatePins();

            this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                this.GetPinValue(this._mLeftIndex),
                this.GetPinValue(this._mRightIndex));
            this.Invalidate();
            this.RequestLayout();
        }

        /// <summary>
        ///     Gets the type of the bar.
        /// </summary>
        /// <returns>true if rangebar, false if seekbar.</returns>
        public bool IsRangeBar()
        {
            return this._mIsRangeBar;
        }

        /// <summary>
        ///     Gets the value of the left pin.
        /// </summary>
        /// <returns>the string value of the left pin.</returns>
        public string GetLeftPinValue()
        {
            return this.GetPinValue(this._mLeftIndex);
        }

        /// <summary>
        ///     Gets the value of the right pin.
        /// </summary>
        /// <returns>the string value of the right pin.</returns>
        public string GetRightPinValue()
        {
            return this.GetPinValue(this._mRightIndex);
        }

        /// <summary>
        ///     Gets the index of the left-most pin.
        /// </summary>
        /// <returns>the 0-based index of the left pin</returns>
        public int GetLeftIndex()
        {
            return this._mLeftIndex;
        }

        /// <summary>
        ///     Gets the index of the right-most pin.
        /// </summary>
        /// <returns>the 0-based index of the right pin</returns>
        public int GetRighrtIndex()
        {
            return this._mRightIndex;
        }

        /// <summary>
        ///     Gets the tick interval.
        /// </summary>
        /// <returns>the tick interval</returns>
        public double GetTickInterval()
        {
            return this._mTickInterval;
        }

        /// <summary>Returns the enabled status for this view.</summary>
        /// <value>To be added.</value>
        /// <remarks>
        ///     <para tool="javadoc-to-mdoc: Get method documentation">
        ///         <format type="text/html">
        ///             <b>Get method documentation</b>
        ///             <a href="http://developer.android.com/reference/android/view/View.html#isEnabled()" target="_blank">
        ///                 [Android
        ///                 Documentation]
        ///             </a>
        ///             <br />
        ///         </format>
        ///         Returns the enabled status for this view. The interpretation of the
        ///         enabled state varies by subclass.
        ///     </para>
        ///     <para tool="javadoc-to-mdoc: Set method documentation">
        ///         <format type="text/html">
        ///             <b>Set method documentation</b>
        ///             <a href="http://developer.android.com/reference/android/view/View.html#setEnabled(bool)" target="_blank">
        ///                 [Android
        ///                 Documentation]
        ///             </a>
        ///             <br />
        ///         </format>
        ///         Set the enabled state of this view. The interpretation of the enabled
        ///         state varies by subclass.
        ///     </para>
        /// </remarks>
        /// <since version="Added in API level 1" />
        public override bool Enabled
        {
            get { return this._enabled; }
            set
            {
                if (!this._enabled)
                {
                    this._mBarColor = DefaultBarColor;
                    this._mConnectingLineColor = DefaultBarColor;
                    this._mCircleColor = DefaultBarColor;
                    this._mTickColor = DefaultBarColor;
                }
                else
                {
                    this._mBarColor = this._mActiveBarColor;
                    this._mConnectingLineColor = this._mActiveConnectingLineColor;
                    this._mCircleColor = this._mActiveCircleColor;
                    this._mTickColor = this._mActiveTickColor;
                }

                this._enabled = value;

                this.CreateBar();
                this.CreatePins();
                this.CreateConnectingLine();
            }
        }

        public void SetPinTextFormatter(IPinTextFormatter pinTextFormatter)
        {
            this._mPinTextFormatter = pinTextFormatter;
        }

        // Private Methods /////////////////////////////////////////////////////////

        /// <summary>
        ///     Does all the functions of the constructor for RangeBar. Called by both
        ///     RangeBar constructors in lieu of copying the code for each constructor.
        /// </summary>
        /// <param name="context">Context from the constructor.</param>
        /// <param name="attrs">AttributeSet from the constructor.</param>
        private void RangeBarInit(Context context, IAttributeSet attrs)
        {
            //TODO tick value map
            if (this._mTickMap == null)
            {
                this._mTickMap = new Dictionary<float, string>();
            }

            var ta = attrs == null
                ? context.ObtainStyledAttributes(Resource.Styleable.RangeBar)
                : context.ObtainStyledAttributes(attrs, Resource.Styleable.RangeBar, 0, 0);
            try
            {
                // Sets the values of the user-defined attributes based on the XML
                // attributes.

                var tickStart = ta
                    .GetFloat(Resource.Styleable.RangeBar_tickStart, DefaultTickStart);

                var tickEnd = ta
                    .GetFloat(Resource.Styleable.RangeBar_tickEnd, DefaultTickEnd);

                var tickInterval = ta
                    .GetFloat(Resource.Styleable.RangeBar_tickInterval, DefaultTickInterval);
                var tickCount = (int) ((tickEnd - tickStart) / tickInterval) + 1;
                if (this.IsValidTickCount(tickCount))
                {
                    // Similar functions performed above in setTickCount; make sure
                    // you know how they interact
                    this._mTickCount = tickCount;
                    this._mTickStart = tickStart;
                    this._mTickEnd = tickEnd;
                    this._mTickInterval = tickInterval;
                    this._mLeftIndex = 0;
                    this._mRightIndex = this._mTickCount - 1;

                    this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                        this.GetPinValue(this._mLeftIndex),
                        this.GetPinValue(this._mRightIndex));
                }
                else
                {
                    Log.Error(RangeBarTag, "tickCount less than 2; invalid tickCount. XML input ignored.");
                }

                this._mTickHeightDp = ta
                    .GetDimension(Resource.Styleable.RangeBar_tickHeight, DefaultTickHeightDp);
                this._mBarWeight = ta.GetDimension(Resource.Styleable.RangeBar_barWeight, DefaultBarWeightPx);
                this._mBarColor = ta.GetColor(Resource.Styleable.RangeBar_rangeBarColor, DefaultBarColor);
                this._mTextColor = ta.GetColor(Resource.Styleable.RangeBar_textColor, DefaultTextColor);
                this._mPinColor = ta.GetColor(Resource.Styleable.RangeBar_pinColor, DefaultPinColor);
                this._mActiveBarColor = this._mBarColor;
                this._mCircleSize = ta.GetDimension(Resource.Styleable.RangeBar_selectorSize,
                    TypedValue.ApplyDimension(ComplexUnitType.Dip, DefaultCircleSizeDp, this.Resources.DisplayMetrics)
                );
                this._mCircleColor = ta.GetColor(Resource.Styleable.RangeBar_selectorColor,
                    DefaultConnectingLineColor);
                this._mActiveCircleColor = this._mCircleColor;
                this._mTickColor = ta.GetColor(Resource.Styleable.RangeBar_tickColor, DefaultTickColor);
                this._mActiveTickColor = this._mTickColor;
                this._mConnectingLineWeight = ta.GetDimension(Resource.Styleable.RangeBar_connectingLineWeight,
                    DefaultConnectingLineWeightPx);
                this._mConnectingLineColor = ta.GetColor(Resource.Styleable.RangeBar_connectingLineColor,
                    DefaultConnectingLineColor);
                this._mActiveConnectingLineColor = this._mConnectingLineColor;
                this._mExpandedPinRadius = ta
                    .GetDimension(Resource.Styleable.RangeBar_pinRadius, TypedValue.ApplyDimension(
                        ComplexUnitType.Dip,
                        this._mExpandedPinRadius, this.Resources.DisplayMetrics));
                this._mPinPadding = ta.GetDimension(Resource.Styleable.RangeBar_pinPadding, TypedValue
                    .ApplyDimension(ComplexUnitType.Dip, DefaultPinPaddingDp, this.Resources.DisplayMetrics));
                this._mBarPaddingBottom = ta.GetDimension(Resource.Styleable.RangeBar_rangeBarPaddingBottom,
                    TypedValue.ApplyDimension(ComplexUnitType.Dip,
                        DefaultBarPaddingBottomDp, this.Resources.DisplayMetrics));
                this._mIsRangeBar = ta.GetBoolean(Resource.Styleable.RangeBar_rangeBar, true);
                this._mArePinsTemporary = ta.GetBoolean(Resource.Styleable.RangeBar_temporaryPins, true);

                var density = this.Resources.DisplayMetrics.Density;
                this._mMinPinFont = ta.GetDimension(Resource.Styleable.RangeBar_pinMinFont,
                    this._mMinPinFont * density);
                this._mMaxPinFont = ta.GetDimension(Resource.Styleable.RangeBar_pinMaxFont,
                    this._mMaxPinFont * density);

                this._mIsRangeBar = ta.GetBoolean(Resource.Styleable.RangeBar_rangeBar, true);
            }
            finally
            {
                ta.Recycle();
            }
        }

        /// <summary>
        ///     Creates a new mBar
        /// </summary>
        private void CreateBar()
        {
            this._mBar = new Bar(this.Context, this.GetMarginLeft(), this.GetYPos(), this.GetBarLength(),
                this._mTickCount, this._mTickHeightDp, this._mTickColor, this._mBarWeight, this._mBarColor);
            this.Invalidate();
        }

        /// <summary>
        ///     Creates a new ConnectingLine.
        /// </summary>
        private void CreateConnectingLine()
        {
            this._mConnectingLine = new ConnectingLine(this.Context, this.GetYPos(), this._mConnectingLineWeight,
                this._mConnectingLineColor);
            this.Invalidate();
        }

        /// <summary>
        ///     Creates two new Pins.
        /// </summary>
        private void CreatePins()
        {
            var ctx = this.Context;
            var yPos = this.GetYPos();
            var expandedPinRadius = 0.0f;

            if (this.Enabled)
            {
                expandedPinRadius = this._mExpandedPinRadius / this.Resources.DisplayMetrics.Density;
            }

            if (this._mIsRangeBar)
            {
                this._mLeftThumb = new PinView(ctx);
                this._mLeftThumb.Init(ctx, yPos, expandedPinRadius, this._mPinColor, this._mTextColor, this._mCircleSize,
                    this._mCircleColor, this._mMinPinFont, this._mMaxPinFont, this._mArePinsTemporary);
            }
            this._mRightThumb = new PinView(ctx);
            this._mRightThumb
                .Init(ctx, yPos, expandedPinRadius, this._mPinColor, this._mTextColor, this._mCircleSize, this._mCircleColor,
                    this._mMinPinFont, this._mMaxPinFont, this._mArePinsTemporary);

            var marginLeft = this.GetMarginLeft();
            var barLength = this.GetBarLength();

            // Initialize thumbs to the desired indices
            if (this._mIsRangeBar)
            {
                this._mLeftThumb.SetX(marginLeft + (this._mLeftIndex / (float) (this._mTickCount - 1)) * barLength);
                this._mLeftThumb.SetXValue(this.GetPinValue(this._mLeftIndex));
            }
            this._mRightThumb.SetX(marginLeft + (this._mRightIndex / (float) (this._mTickCount - 1)) * barLength);
            this._mRightThumb.SetXValue(this.GetPinValue(this._mRightIndex));

            this.Invalidate();
        }

        /// <summary>
        ///     Get marginLeft in each of the public attribute methods.
        /// </summary>
        /// <returns>float marginLeft</returns>
        private float GetMarginLeft()
        {
            return Math.Max(this._mExpandedPinRadius, this._mCircleSize);
        }

        /// <summary>
        ///     Get yPos in each of the public attribute methods.
        /// </summary>
        /// <returns>float yPos</returns>
        private float GetYPos()
        {
            return (this.Height - this._mBarPaddingBottom);
        }

        /// <summary>
        ///     Get barLength in each of the public attribute methods.
        /// </summary>
        /// <returns>float barLength</returns>
        private float GetBarLength()
        {
            return (this.Width - 2 * this.GetMarginLeft());
        }

        /// <summary>
        ///     Returns if either index is outside the range of the tickCount.
        /// </summary>
        /// <param name="leftThumbIndex">Integer specifying the left thumb index.</param>
        /// <param name="rightThumbIndex">Integer specifying the right thumb index.</param>
        /// <returns>bool If the index is out of range.</returns>
        private bool IndexOutOfRange(int leftThumbIndex, int rightThumbIndex)
        {
            return (leftThumbIndex < 0 || leftThumbIndex >= this._mTickCount
                    || rightThumbIndex < 0
                    || rightThumbIndex >= this._mTickCount);
        }

        /// <summary>
        ///     Returns if either value is outside the range of the tickCount.
        /// </summary>
        /// <param name="leftThumbValue">Float specifying the left thumb value.</param>
        /// <param name="rightThumbValue">Float specifying the right thumb value.</param>
        /// <returns>bool If the index is out of range.</returns>
        private bool ValueOutOfRange(float leftThumbValue, float rightThumbValue)
        {
            return (leftThumbValue < this._mTickStart || leftThumbValue > this._mTickEnd
                    || rightThumbValue < this._mTickStart || rightThumbValue > this._mTickEnd);
        }

        /// <summary>
        ///     If is invalid tickCount, rejects. TickCount must be greater than 1
        /// </summary>
        /// <param name="tickCount">Integer</param>
        /// <returns>bool: whether tickCount > 1</returns>
        private bool IsValidTickCount(int tickCount)
        {
            return (tickCount > 1);
        }

        /// <summary>
        ///     Handles a <see cref="MotionEventActions.Down" /> event.
        /// </summary>
        /// <param name="x">the x-coordinate of the down action</param>
        /// <param name="y">the y-coordinate of the down action</param>
        private void OnActionDown(float x, float y)
        {
            if (this._mIsRangeBar)
            {
                if (!this._mRightThumb.Pressed && this._mLeftThumb.IsInTargetZone(x, y))
                {
                    this.PressPin(this._mLeftThumb);
                }
                else if (!this._mLeftThumb.Pressed && this._mRightThumb.IsInTargetZone(x, y))
                {
                    this.PressPin(this._mRightThumb);
                }
            }
            else
            {
                if (this._mRightThumb.IsInTargetZone(x, y))
                {
                    this.PressPin(this._mRightThumb);
                }
            }
        }

        /// <summary>
        ///     Handles a <see cref="MotionEventActions.Up" /> or
        ///     <see cref="MotionEventActions.Cancel" /> event.
        /// </summary>
        /// <param name="x">the x-coordinate of the up action</param>
        private void OnActionUp(float x)
        {
            if (this._mIsRangeBar && this._mLeftThumb.Pressed)
            {
                this.ReleasePin(this._mLeftThumb);
            }
            else if (this._mRightThumb.Pressed)
            {
                this.ReleasePin(this._mRightThumb);
            }
            else
            {
                var leftThumbXDistance = this._mIsRangeBar ? Math.Abs(this._mLeftThumb.GetX() - x) : 0;
                var rightThumbXDistance = Math.Abs(this._mRightThumb.GetX() - x);

                if (leftThumbXDistance < rightThumbXDistance)
                {
                    if (this._mIsRangeBar)
                    {
                        this.MovePin(this._mLeftThumb, x);
                        this.ReleasePin(this._mLeftThumb);
                    }
                }
                else
                {
                    this.MovePin(this._mRightThumb, x);
                    this.ReleasePin(this._mRightThumb);
                }

                // Get the updated nearest tick marks for each thumb.

                var newLeftIndex = this._mIsRangeBar ? this._mBar.GetNearestTickIndex(this._mLeftThumb) : 0;

                var newRightIndex = this._mBar.GetNearestTickIndex(this._mRightThumb);
                // If either of the indices have changed, update and call the listener.
                if (newLeftIndex != this._mLeftIndex || newRightIndex != this._mRightIndex)
                {
                    this._mLeftIndex = newLeftIndex;
                    this._mRightIndex = newRightIndex;

                    this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                        this.GetPinValue(this._mLeftIndex),
                        this.GetPinValue(this._mRightIndex));
                }
            }
        }

        /// <summary>
        ///     Handles a <see cref="MotionEventActions.Move" /> event.
        /// </summary>
        /// <param name="x">the x-coordinate of the move event</param>
        private void OnActionMove(float x)
        {
            // Move the pressed thumb to the new x-position.
            if (this._mIsRangeBar && this._mLeftThumb.Pressed)
            {
                this.MovePin(this._mLeftThumb, x);
            }
            else if (this._mRightThumb.Pressed)
            {
                this.MovePin(this._mRightThumb, x);
            }

            // If the thumbs have switched order, fix the references.
            if (this._mIsRangeBar && this._mLeftThumb.GetX() > this._mRightThumb.GetX())
            {
                var temp = this._mLeftThumb;
                this._mLeftThumb = this._mRightThumb;
                this._mRightThumb = temp;
            }

            // Get the updated nearest tick marks for each thumb.
            var newLeftIndex = this._mIsRangeBar ? this._mBar.GetNearestTickIndex(this._mLeftThumb) : 0;
            var newRightIndex = this._mBar.GetNearestTickIndex(this._mRightThumb);


            var componentLeft = this.Left + this.PaddingLeft;

            var componentRight = this.Right - this.PaddingRight - componentLeft;

            if (x <= componentLeft)
            {
                newLeftIndex = 0;
                this.MovePin(this._mLeftThumb, this._mBar.GetLeftX());
            }
            else if (x >= componentRight)
            {
                newRightIndex = this.GetTickCount() - 1;
                this.MovePin(this._mRightThumb, this._mBar.GetRightX());
            }
            // end added code
            // If either of the indices have changed, update and call the listener.
            if (newLeftIndex == this._mLeftIndex && newRightIndex == this._mRightIndex)
                return;

            this._mLeftIndex = newLeftIndex;
            this._mRightIndex = newRightIndex;
            if (this._mIsRangeBar)
            {
                this._mLeftThumb.SetXValue(this.GetPinValue(this._mLeftIndex));
            }
            this._mRightThumb.SetXValue(this.GetPinValue(this._mRightIndex));

            this._mListener?.OnRangeChangeListener(this, this._mLeftIndex, this._mRightIndex,
                this.GetPinValue(this._mLeftIndex),
                this.GetPinValue(this._mRightIndex));
        }

        /// <summary>
        ///     Set the thumb to be in the pressed state and calls invalidate() to redraw
        ///     the canvas to reflect the updated state.
        /// </summary>
        /// <param name="thumb">the thumb to press</param>
        private void PressPin(PinView thumb)
        {
            if (this._mFirstSetTickCount)
            {
                this._mFirstSetTickCount = false;
            }
            if (this._mArePinsTemporary)
            {
                var animator = ValueAnimator.OfFloat(0, this._mExpandedPinRadius);
                animator.AddUpdateListener(new PressPinListener(this, thumb)
                {
                    PinPadding = this._mPinPadding,
                    ThumbRadius = this._mThumbRadiusDp
                });
                animator.Start();
            }

            thumb.Press();
        }

        /// <summary>
        ///     Set the thumb to be in the normal/un-pressed state and calls invalidate()
        ///     to redraw the canvas to reflect the updated state.
        /// </summary>
        /// <param name="thumb">the thumb to release</param>
        private void ReleasePin(PinView thumb)
        {
            var nearestTickX = this._mBar.GetNearestTickCoordinate(thumb);
            thumb.SetX(nearestTickX);
            var tickIndex = this._mBar.GetNearestTickIndex(thumb);
            thumb.SetXValue(this.GetPinValue(tickIndex));

            if (this._mArePinsTemporary)
            {
                var animator = ValueAnimator.OfFloat(this._mExpandedPinRadius, 0);
                animator.AddUpdateListener(new ReleasePinListener(this, thumb)
                {
                    PinPadding = this._mPinPadding,
                    ThumbRadius = this._mThumbRadiusDp
                });
                animator.Start();
            }
            else
            {
                this.Invalidate();
            }

            thumb.Release();
        }

        /// <summary>
        ///     Set the value on the thumb pin, either from map or calculated from the tick intervals
        ///     Integer check to format decimals as whole numbers
        /// </summary>
        /// <param name="tickIndex">the index to set the value for</param>
        /// <returns></returns>
        private string GetPinValue(int tickIndex)
        {
            if (this._mPinTextListener != null)
            {
                return this._mPinTextListener.GetPinValue(this, tickIndex);
            }
            var tickValue = (tickIndex == (this._mTickCount - 1))
                ? this._mTickEnd
                : (tickIndex *
                   this._mTickInterval)
                  + this._mTickStart;
            string xValue;
            if (this._mTickMap.TryGetValue(tickValue, out xValue))
                return this._mPinTextFormatter.GetText(xValue);

            xValue = tickValue.ToString("F");
            return this._mPinTextFormatter.GetText(xValue);
        }

        /// <summary>
        ///     Moves the thumb to the given x-coordinate.
        /// </summary>
        /// <param name="thumb">the thumb to move</param>
        /// <param name="x">the x-coordinate to move the thumb to</param>
        private void MovePin(PinView thumb, float x)
        {
            // If the user has moved their finger outside the range of the bar,
            // do not move the thumbs past the edge.
            if (x < this._mBar.GetLeftX() || x > this._mBar.GetRightX())
            {
                // Do nothing.
            }
            else if (thumb != null)
            {
                thumb.SetX(x);
                this.Invalidate();
            }
        }

// Inner Classes ///////////////////////////////////////////////////////////

        /// <summary>
        ///     A callback that notifies clients when the RangeBar has changed. The
        ///     listener will only be called when either thumb's index has changed - not
        ///     for every movement of the thumb.
        /// </summary>
        public interface IOnRangeBarChangeListener
        {
            void OnRangeChangeListener(RangeBar rangeBar, int leftPinIndex,
                int rightPinIndex, string leftPinValue, string rightPinValue);
        }

        public interface IPinTextFormatter
        {
            string GetText(string value);
        }

        /// <summary>
        ///     @author robmunro
        ///     A callback that allows getting pin text exernally
        /// </summary>
        public interface IOnRangeBarTextListener
        {
            string GetPinValue(RangeBar rangeBar, int tickIndex);
        }

        #endregion

        #region RangeBar.IPinTextFormatter

        public string GetText(string value)
        {
            return value.Length > 4 ? value.Substring(0, 4) : value;
        }

        #endregion
    }
}