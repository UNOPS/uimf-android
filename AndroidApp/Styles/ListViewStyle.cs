namespace AndroidApp.Styles
{
    using Android.Graphics;
    using Android.Graphics.Drawables;
    using Android.Widget;
    using App.Style;

    [Style(Name = "ListView")]
    public class ListViewStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            if (element is ListView listView)
            {
                listView.Divider = new ColorDrawable(Color.ParseColor(AppColors.Gray));
                listView.DividerHeight = 2;
                listView.SetPadding(10, 0, 10, 0);
                listView.FastScrollEnabled = true;
            }
        }
    }
}