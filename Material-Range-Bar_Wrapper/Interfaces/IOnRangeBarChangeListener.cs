using Material_Range_Bar_Wrapper.Views;

namespace Material_Range_Bar_Wrapper.Interfaces
{
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
}