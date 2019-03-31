namespace AndroidApp.Styles
{
    using Android.Graphics;
    using Android.Widget;
    using App.Style;

    [Style(Name = "ValidationError")]
    public class ValidationErrorStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            var editText = element as EditText;
            editText?.Background.SetColorFilter(Color.ParseColor(AppColors.Pink), PorterDuff.Mode.SrcAtop);

            var spinner = element as Spinner;
            spinner?.Background.SetColorFilter(Color.ParseColor(AppColors.Pink), PorterDuff.Mode.SrcAtop);
        }
    }
}