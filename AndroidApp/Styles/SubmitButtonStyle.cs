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

                var layoutParams = btn.MatchParentWrapContent();
                btn.LayoutParameters = layoutParams;
            }
        }
    }
}