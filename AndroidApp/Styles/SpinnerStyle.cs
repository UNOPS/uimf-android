namespace AndroidApp.Styles
{
    using Android.Graphics;
    using Android.Widget;
    using App.Style;

    [Style(Name = "Spinner")]
    public class SpinnerStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            var spinner = element as Spinner;
            spinner?.Background.SetColorFilter(Color.ParseColor(AppColors.Blue), PorterDuff.Mode.SrcAtop);
            spinner?.SetPopupBackgroundResource(Resource.Color.white);
        }
    }
}