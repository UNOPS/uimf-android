namespace AndroidUiMetadataFramework.Core.Inputs
{
    using System;
    using System.Collections.Generic;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;

    [Input(Type = "datetime")]
    public class DateTimeInput : IInputManager
    {
        private DatePicker DateInput { get; set; }

        public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
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