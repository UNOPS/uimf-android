namespace AndroidApp.Styles
{
    using Android.Graphics;
    using Android.Widget;
    using App.Style;

    [Style(Name = "Link")]
    public class LinkStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            var textView = element as TextView;
            textView?.SetTextColor(Color.ParseColor(AppColors.DarkBlue));

            var btn = element as Button;
            btn?.SetTextColor(Color.ParseColor(AppColors.DarkBlue));
            btn?.SetBackgroundColor(Color.White);
            btn?.SetBackgroundResource(0);
        }
    }
}