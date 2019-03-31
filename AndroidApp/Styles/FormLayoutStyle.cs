namespace AndroidApp.Styles
{
    using Android.Graphics;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Models;
    using App.Style;

    [Style(Name = "FormLayout")]
    public class FormLayoutStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            if (element is LinearLayout el)
            {
                el.SetBackgroundColor(Color.ParseColor(AppColors.LightGray));
                el.SetPadding(20, 10, 20, 20);
                var layoutParams = el.MatchParentWrapContent();
                el.LayoutParameters = layoutParams;
            }
        }
    }
}