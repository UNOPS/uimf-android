namespace AndroidUiMetadataFramework.Core.Outputs
{
	using System.Collections.Generic;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadataFramework.Core.Attributes;
	using AndroidUiMetadataFramework.Core.Managers;
	using AndroidUiMetadataFramework.Core.Models;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;

	[Output(Type = "text-value")]
	public class TextValueOutput : IOutputManager
	{
		private TextView OutputText { get; set; }

		public View GetView(OutputFieldMetadata outputField, object value, MyFormHandler myFormHandler, FormMetadata formMetadata, List<FormInputManager> inputsManager)
		{
			var textValue = value.CastTObject<TextValue<object>>();
			this.OutputText = new TextView(Application.Context) { Text = outputField.Label + ": " + textValue.Value };
		    myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("TextView", this.OutputText);
            return this.OutputText;
		}
	}
}