namespace AndroidApp.Styles
{
    using Android.Graphics;
    using Android.Widget;
    using App.Style;

    [Style(Name = "CurrentTab")]
    public class CurrentTabStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            if (element is TextView textView)
            {
                textView.SetTextColor(Color.ParseColor("#3287c0"));
                textView.SetBackgroundResource(Resource.Drawable.current_tab_border);
                textView.Typeface = Typeface.DefaultBold;

            }
        }
    }
}