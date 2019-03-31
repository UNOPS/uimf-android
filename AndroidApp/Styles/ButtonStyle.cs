namespace AndroidApp.Styles
{
    using Android.Graphics;
    using Android.Views;
    using Android.Widget;
    using App.Style;
    using Com.Google.Android.Flexbox;

    [Style(Name = "Button")]
    public class ButtonStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            if (element is Button btn)
            {
                btn.SetTextColor(Color.ParseColor("#138496"));
                btn.SetAllCaps(false);
                var param = new FlexboxLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent);
                param.SetMargins(0, 0, 10, 0);
                btn.LayoutParameters = param;
                btn.TextSize = 12;
                btn.SetPadding(20,15,20,15);
                btn.SetBackgroundResource(Resource.Drawable.button_bg);
                btn.SetMinHeight(0);
                btn.SetMinWidth(0);
                btn.SetMinimumHeight(0);
                btn.SetMinimumWidth(0);
                btn.Typeface = Typeface.Default;
            }
        }
    }
}