namespace AndroidApp.Styles
{
    using Android.Widget;
    using App.Style;

    [Style(Name = "ListViewItem")]
    public class ListViewItemStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            if (element is LinearLayout layout)
            {
                layout.SetPadding(20, 10, 10, 10);
                layout.SetBackgroundResource(Resource.Drawable.list_bg);
            }
        }
    }
}