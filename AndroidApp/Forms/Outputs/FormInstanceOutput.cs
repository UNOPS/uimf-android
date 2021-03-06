﻿namespace AndroidApp.Forms.Outputs
{
    using System.Collections.Generic;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Core;

    [Output(Type = "form-instance")]
    public class FormInstanceOutput : IOutputManager
    {
        private LinearLayout OutputView { get; set; }

        public View GetView(OutputFieldMetadata outputField,
            object value,
            MyFormHandler myFormHandler,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager)
        {
            this.OutputView = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };
            var label = new TextView(Application.Context) { Text = outputField.Label };
            label.LayoutParameters = label.WrapContent();
            myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("TextView", label);
            this.OutputView.AddView(label);
            var formInstance = value.CastTObject<FormInstanceModel>();
            foreach (var formInstanceValue in formInstance.Values)
            {
                var textView = new TextView(Application.Context) { Text = formInstanceValue.Label + ": " + formInstanceValue.Value };
                textView.LayoutParameters = textView.WrapContent();
                myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("TextView", textView);
                this.OutputView.AddView(textView);
            }
            return this.OutputView;
        }
    }

    public class FormInstanceValue
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class FormInstanceModel
    {
        public FormInstanceModel(IEnumerable<FormInstanceValue> values)
        {
            this.Values = values;
        }

        public IEnumerable<FormInstanceValue> Values { get; set; }
    }
}