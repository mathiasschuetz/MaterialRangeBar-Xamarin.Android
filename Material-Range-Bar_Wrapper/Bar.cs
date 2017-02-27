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

using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Material_Range_Bar_Wrapper.Views;

namespace Material_Range_Bar_Wrapper
{
    /// <summary>
    /// This class represents the underlying gray bar in the RangeBar (without the
    /// thumbs).
    /// </summary>
    public class Bar
    {
        #region fields

        private readonly Paint _barPaint;
        private readonly Paint _tickPaint;

        // Left-coordinate of the horizontal bar.
        private readonly float _leftX;
        private readonly float _rightX;
        private readonly float _y;
        private int _numSegments;
        private float _tickDistance;
        private readonly float _tickHeight;

        #endregion

        #region properties

        #endregion

        #region ctor
        
        /// <summary>
        /// Bar constructor
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="x">the start x co-ordinate</param>
        /// <param name="y">the y co-ordinate</param>
        /// <param name="length">the length of the bar in px</param>
        /// <param name="tickCount">the number of ticks on the bar</param>
        /// <param name="tickHeightDp">the height of each tick</param>
        /// <param name="tickColor">the color of each tick</param>
        /// <param name="barWeight">the weight of the bar</param>
        /// <param name="barColor">the color of the bar</param>
        public Bar(Context context,
            float x,
            float y,
            float length,
            int tickCount,
            float tickHeightDp,
            Color tickColor,
            float barWeight,
            Color barColor)
        {
            this._leftX = x;
            this._rightX = x + length;
            this._y = y;

            this._numSegments = tickCount - 1;
            this._tickDistance = length / this._numSegments;
            this._tickHeight = TypedValue.ApplyDimension(ComplexUnitType.Dip, tickHeightDp,
                context.Resources.DisplayMetrics);

            // Initialize the paint.
            this._barPaint = new Paint
            {
                Color = barColor,
                StrokeWidth = barWeight,
                AntiAlias = true
            };

            this._tickPaint = new Paint
            {
                Color = tickColor,
                StrokeWidth = barWeight,
                AntiAlias = true
            };
        }

        #endregion

        #region methods
        
        /// <summary>
        /// Draws the bar on the given Canvas.
        /// </summary>
        /// <param name="canvas">Canvas to draw on; should be the Canvas passed into <see cref="View.OnDraw"/></param>
        public void Draw(Canvas canvas)
        {
            canvas.DrawLine(this._leftX, this._y, this._rightX, this._y, this._barPaint);
        }

        /// <summary>
        /// Get the x-coordinate of the left edge of the bar.
        /// </summary>
        /// <returns>x-coordinate of the left edge of the bar</returns>
        public float GetLeftX()
        {
            return this._leftX;
        }

        /// <summary>
        /// Get the x-coordinate of the right edge of the bar.
        /// </summary>
        /// <returns>x-coordinate of the right edge of the bar</returns>
        public float GetRightX()
        {
            return this._rightX;
        }

        /// <summary>
        /// Gets the x-coordinate of the nearest tick to the given x-coordinate.
        /// </summary>
        /// <param name="thumb">the thumb to find the nearest tick for</param>
        /// <returns>the x-coordinate of the nearest tick</returns>
        public float GetNearestTickCoordinate(PinView thumb)
        {
            var nearestTickIndex = this.GetNearestTickIndex(thumb);

            return this._leftX + nearestTickIndex * this._tickDistance;
        }

        /// <summary>
        /// Gets the zero-based index of the nearest tick to the given thumb.
        /// </summary>
        /// <param name="thumb">the Thumb to find the nearest tick for</param>
        /// <returns>the zero-based index of the nearest tick</returns>
        public int GetNearestTickIndex(PinView thumb)
        {

            return (int)((thumb.GetX() - this._leftX + this._tickDistance / 2f) / this._tickDistance);
        }

        /// <summary>
        /// Set the number of ticks that will appear in the RangeBar.
        /// </summary>
        /// <param name="tickCount">tickCount the number of ticks</param>
        public void SetTickCount(int tickCount)
        {
            var barLength = this._rightX - this._leftX;

            this._numSegments = tickCount - 1;
            this._tickDistance = barLength / this._numSegments;
        }

        public void DrawTicks(Canvas canvas)
        {
            // Loop through and draw each tick (except final tick).
            for (var i = 0; i < this._numSegments; i++)
            {
                var x = i * this._tickDistance + this._leftX;
                canvas.DrawCircle(x, this._y, this._tickHeight, this._tickPaint);
            }
            // Draw final tick. We draw the final tick outside the loop to avoid any
            // rounding discrepancies.
            canvas.DrawCircle(this._rightX, this._y, this._tickHeight, this._tickPaint);
        }

        #endregion
    }
}