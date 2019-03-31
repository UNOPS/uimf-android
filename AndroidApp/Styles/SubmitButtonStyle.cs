namespace AndroidApp.Styles
{
    using Android.Graphics;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Models;
    using App.Style;

    [Style(Name = "SubmitButton")]
    public class SubmitButtonStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            if (element is Button btn)
            {
                btn.Background.SetColorFilter(Color.ParseColor(AppColors.Blue), PorterDuff.Mode.SrcAtop);
                btn.SetTextColor(Color.White);
                btn.SetPadding(20, 25, 20, 25);
                btn.TextSize = 14;
                var layoutParams = btn.MatchParentWrapContent();
                btn.LayoutParameters = layoutParams;
            }
        }
    }
}