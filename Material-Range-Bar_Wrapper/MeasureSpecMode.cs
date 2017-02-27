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

using Android.Views;

namespace Material_Range_Bar_Wrapper
{
    /// <summary>
    ///     Helper enum class for transforming a measureSpec mode integer value into a
    ///     human-readable String. The human-readable String is simply the name of the
    ///     enum value.
    /// </summary>
    public class MeasureSpecMode
    {
        #region fields

        private readonly int _modeValue;

        #endregion

        #region properties

        #endregion

        #region ctor

        public MeasureSpecMode(int modeValue)
        {
            this._modeValue = modeValue;
        }

        #endregion

        #region methods

        /// <summary>
        ///     Gets the int value associated with this mode.
        /// </summary>
        /// <returns>the int value associated with this mode</returns>
        public int GetModeValue()
        {
            return this._modeValue;
        }

        /// <summary>
        ///     Gets the MeasureSpecMode value that corresponds with the given
        ///     measureSpec int value.
        /// </summary>
        /// <param name="measureSpec">
        ///     the measure specification passed by the platform to <see cref="View.OnMeasure(int, int)" />
        /// </param>
        /// <returns>the MeasureSpecMode that matches with that measure spec</returns>
        public static Android.Views.MeasureSpecMode GetMode(int measureSpec)
        {
            return View.MeasureSpec.GetMode(measureSpec);
        }

        #endregion
    }
}