namespace AndroidUiMetadataFramework.Core.Outputs
{
    using System.Collections.Generic;
    using Android.App;
    using Android.Graphics;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core;

    [Output(Type = "formlink")]
    public class FormLinkOutput : IOutputManager
    {
        private LinearLayout Layout { get; set; }

        public View GetView(OutputFieldMetadata outputField,
            object value,
            MyFormHandler myFormHandler,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager)
        {
            var formLink = value.CastTObject<FormLink>();
            this.Layout = new LinearLayout(Application.Context) { Orientation = Orientation.Horizontal };
            this.Layout.AddView(new TextView(Application.Context) { Text = outputField.Label + ": " });
            var text = formLink != null ? this.InitializeLink(formLink, myFormHandler) : new TextView(Application.Context) { Text = "" };

            this.Layout.AddView(text);
            return this.Layout;
        }

        public TextView InitializeLink(FormLink btn, MyFormHandler myFormHandler)
        {
            var text = new TextView(Application.Context) { Text = btn.Label };
            text.SetBackgroundColor(Color.Transparent);
            text.SetTextColor(Color.LightBlue);

            text.Click += async (sender, args) =>
            {
                var formMetadata = myFormHandler.GetFormMetadata(btn.Form);
                myFormHandler.FormWrapper.UpdateView(myFormHandler, new FormParameter(formMetadata, btn.InputFieldValues));
            };
            return text;
        }
    }
}