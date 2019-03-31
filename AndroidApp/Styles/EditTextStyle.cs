namespace AndroidApp.Styles
{
    using Android.Graphics;
    using Android.Widget;
    using App.Style;

    [Style(Name = "EditText")]
    public class EditTextStyle : IStyle
    {
        public void ApplyStyle(object element)
        {
            if (element is EditText editText)
            {
                editText.SetTextColor(Color.Black);
                editText.Background.SetColorFilter(Color.ParseColor(AppColors.Blue), PorterDuff.Mode.SrcAtop);
            }
        }
    }
}