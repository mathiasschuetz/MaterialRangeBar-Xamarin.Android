using Material_Range_Bar_Wrapper.Views;

namespace Material_Range_Bar_Wrapper.Interfaces
{
    /// <summary>
    ///     @author robmunro
    ///     A callback that allows getting pin text exernally
    /// </summary>
    public interface IOnRangeBarTextListener
    {
        string GetPinValue(RangeBar rangeBar, int tickIndex);
    }
}