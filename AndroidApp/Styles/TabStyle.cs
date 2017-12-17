namespace AndroidApp.Styles
{
    using Android.Graphics;
    using Android.Views;
    using Android.Widget;
    using App.Style;
    using Com.Google.Android.Flexbox;

    [Style(Name = "Tab")]
    public class TabStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            if (element is TextView textView)
            {
                textView.SetTextColor(Color.ParseColor("#838c98"));
                textView.SetPadding(20, 20, 20, 20);
                var param = new FlexboxLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent);
                param.SetMargins(0, 0, 10, 10);
                textView.LayoutParameters = param;
                textView.SetBackgroundResource(Resource.Drawable.tab_border);
            }
        }
    }
}