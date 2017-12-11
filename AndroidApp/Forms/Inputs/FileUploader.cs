namespace AndroidApp.Forms.Inputs
{
    using System.Collections.Generic;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;

    [Input(Type = "file-uploader")]
    public class FileUploader : IInputManager
    {
        private LinearLayout Layout { get; set; }

        public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
        {
            this.Layout = new LinearLayout(Application.Context)
            {
                Orientation = Orientation.Horizontal
            };
            var button = new Button(Application.Context)
            {
                Text = "Choose file"
            };
            button.SetAllCaps(false);
            button.Click += (sender, args) => { };
            this.Layout.AddView(button);
            return this.Layout;
        }

        public object GetValue()
        {
            return null;
        }

        public void SetValue(object value)
        {
        }
    }
}