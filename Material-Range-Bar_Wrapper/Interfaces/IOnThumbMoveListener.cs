using Material_Range_Bar_Wrapper.Views;

namespace Material_Range_Bar_Wrapper.Interfaces
{
    public interface IOnThumbMoveListener
    {
        void OnThumbMovingStart(RangeBar rangeBar, bool isLeftThumb, string pinValue);

        void OnThumbMovingStop(RangeBar rangeBar, bool isLeftThumb, string pinValue);
    }
}