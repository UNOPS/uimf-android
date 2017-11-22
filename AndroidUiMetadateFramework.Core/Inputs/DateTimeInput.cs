namespace AndroidUiMetadateFramework.Core.Inputs
{
    using System;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadateFramework.Core.Attributes;
    using AndroidUiMetadateFramework.Core.Managers;
    using AndroidUiMetadateFramework.Core.Models;

    [Input(Type = "datetime")]
    public class DateTimeInput : IInputManager
    {
        private DatePicker DateInput { get; set; }

        public View GetView(object inputCustomProperties, MyFormHandler myFormHandler)
        {
            this.DateInput = new DatePicker(Application.Context)
            {
                ScaleX = 0.5f,
                ScaleY = 0.5f
            };
            return this.DateInput;
        }

        public object GetValue()
        {
            return this.DateInput.DateTime;
        }

        public void SetValue(object value)
        {
            this.DateInput.DateTime = value.CastTObject<DateTime>();
        }
    }
}