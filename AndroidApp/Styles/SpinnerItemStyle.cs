namespace AndroidApp.Styles
{
    using Android.Widget;
    using App.Style;

    [Style(Name = "SpinnerItem")]
    public class SpinnerItemStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            var textView = element as TextView;
            textView?.SetPadding(10, 0, 0, 0);
        }
    }
}