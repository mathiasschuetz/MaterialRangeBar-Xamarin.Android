using Android.App;
using Android.OS;
using Android.Widget;
using Material_Range_Bar_Wrapper.Interfaces;
using Material_Range_Bar_Wrapper.Views;

namespace Example
{
    [Activity(Label = "Example", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IOnRangeBarChangeListener, IPinTextFormatter, IOnThumbMoveListener
    {
        #region lifecycle

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            this.SetContentView(Resource.Layout.Main);

            var rangeBar = this.FindViewById<RangeBar>(Resource.Id.rangebar);
            var notRangeBar = this.FindViewById<RangeBar>(Resource.Id.notrangebar);

            rangeBar.SetOnRangeBarChangeListener(this);
            rangeBar.SetThumbMoveListener(this);

            rangeBar.SetPinTextFormatter(this);

            notRangeBar.SetOnRangeBarChangeListener(this);
            notRangeBar.SetThumbMoveListener(this);
        }

        #endregion

        #region IOnRangeBarChangeListener

        public void OnRangeChangeListener(RangeBar rangeBar, int leftPinIndex, int rightPinIndex, string leftPinValue,
            string rightPinValue)
        {
        }

        #endregion

        #region IPinTextFormatter

        public string GetText(float value)
        {
            return value.ToString("N0");
        }

        #endregion

        #region IOnThumbMoveListener

        public void OnThumbMovingStart(RangeBar rangeBar, bool isLeftThumb, string pinValue)
        {
            Toast.MakeText(this, (isLeftThumb ? "Left" : "Right") + $" started moving: {pinValue}", ToastLength.Short).Show();
        }

        public void OnThumbMovingStop(RangeBar rangeBar, bool isLeftThumb, string pinValue)
        {
            Toast.MakeText(this, (isLeftThumb ? "Left" : "Right") + $" stopped moving: {pinValue}", ToastLength.Short).Show();
        }

        #endregion
    }
}