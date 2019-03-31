namespace AndroidApp.Styles
{
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Models;
    using App.Style;

    [Style(Name = "ResultsLayout")]
    public class ResultsLayoutStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            if (element is LinearLayout el)
            {
                el.SetPadding(20, 10, 20, 10);
                var layoutParams = el.MatchParentWrapContent();
                el.LayoutParameters = layoutParams;
            }
        }
    }
}