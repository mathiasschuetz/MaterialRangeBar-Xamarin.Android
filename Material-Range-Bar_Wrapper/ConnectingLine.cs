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
using Material_Range_Bar_Wrapper.Views;

namespace Material_Range_Bar_Wrapper
{
    /// <summary>
    /// Class representing the blue connecting line between the two thumbs.
    /// </summary>
    public class ConnectingLine
    {
        #region private 

        private readonly Paint _paint;
        private readonly float _y;

        #endregion

        #region properties

        #endregion

        #region ctor

        public ConnectingLine(Context context, float y, float connectingLineWeight, Color connectingLineColor)
        {
            var connectingLineWeight1 = TypedValue.ApplyDimension(ComplexUnitType.Dip, connectingLineWeight,
                context.Resources.DisplayMetrics);

            // Initialize the paint, set values
            this._paint = new Paint
            {
                Color = connectingLineColor,
                StrokeWidth = connectingLineWeight1,
                StrokeCap = Paint.Cap.Round,
                AntiAlias = true
            };

            this._y = y;
        }

        #endregion

        #region methods

        /// <summary>
        /// Draw the connecting line between the two thumbs in rangebar.
        /// </summary>
        /// <param name="canvas">the Canvas to draw to</param>
        /// <param name="leftThumb">the left thumb</param>
        /// <param name="rightThumb">the right thumb</param>
        public void Draw(Canvas canvas, PinView leftThumb, PinView rightThumb)
        {
            canvas.DrawLine(leftThumb.GetX(), this._y, rightThumb.GetX(), this._y, this._paint);
        }

        /// <summary>
        /// Draw the connecting line between for single slider.
        /// </summary>
        /// <param name="canvas">the Canvas to draw to</param>
        /// <param name="leftMargin">the right thumb</param>
        /// <param name="rightThumb">the left margin</param>
        public void Draw(Canvas canvas, float leftMargin, PinView rightThumb)
        {
            canvas.DrawLine(leftMargin, this._y, rightThumb.GetX(), this._y, this._paint);
        }

        #endregion
    }
}