namespace AndroidUiMetadataFramework.Core.Outputs
{
    using System.Collections.Generic;
    using Android.Views;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core;

    [Output(Type = "inline-form")]
    public class InlineFormOutput : IOutputManager
    {
        private View LayouView { get; set; }

        public View GetView(OutputFieldMetadata outputField,
            object value,
            MyFormHandler myFormHandler,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager)
        {
            var inlineForm = value.CastTObject<InlineForm>();
            this.LayouView = myFormHandler.GetIForm(inlineForm.Form, inlineForm.InputFieldValues);

            return this.LayouView;
        }
    }
}