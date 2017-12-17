namespace AndroidApp.Styles
{
    using Android.Graphics;
    using Android.Widget;
    using App.Style;

    [Style(Name = "TextView")]
    public class TextViewStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            var textView = element as TextView;
            textView?.SetTextColor(Color.Black);
        }
    }
}