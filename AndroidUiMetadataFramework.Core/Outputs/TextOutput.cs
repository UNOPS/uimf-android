﻿namespace AndroidUiMetadataFramework.Core.Outputs
{
    using System.Collections.Generic;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Core;

    [Output(Type = "text")]
    public class TextOutput : IOutputManager
    {
        private TextView OutputText { get; set; }

        public View GetView(OutputFieldMetadata outputField,
            object value,
            MyFormHandler myFormHandler,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager)
        {
            this.OutputText = new TextView(Application.Context);
            myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("TextView", this.OutputText);
            if (value != null)
            {
                this.OutputText.Text = outputField.Label + ": " + value;
            }
            return this.OutputText;
        }
    }
}