namespace AndroidApp.Styles
{
    using Android.App;
    using Android.Support.V4.Content;
    using Android.Support.V4.Widget;
    using Android.Widget;
    using App.Style;

    [Style(Name = "CheckBox")]
    public class CheckBoxStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            //checkBox?.Background.SetColorFilter(Color.ParseColor(AppColors.Blue), PorterDuff.Mode.SrcOut);
            ////checkBox.BackgroundTintList. = Color.Black;
            //var colors = new[] { Resource.Color.blue };
            //var state = new[] { Android.Resource.Attribute.StateChecked };
            //var states = new int[][] { state };
            //var colorList = new ColorStateList(states, colors);
            if (element is CheckBox checkBox)
            {
                CompoundButtonCompat.SetButtonTintList(checkBox, ContextCompat.GetColorStateList(Application.Context, Resource.Color.blue));
            }
        }
    }
}