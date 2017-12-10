namespace AndroidUiMetadataFramework.Core.Outputs
{
	using System.Collections.Generic;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadataFramework.Core.Attributes;
	using AndroidUiMetadataFramework.Core.Managers;
	using AndroidUiMetadataFramework.Core.Models;
	using UiMetadataFramework.Core;

	[Output(Type = "list")]
	public class ListOutput : IOutputManager
	{
		private TextView OutputText { get; set; }

		public View GetView(OutputFieldMetadata outputField, 
			object value, 
			MyFormHandler myFormHandler, 
			FormMetadata formMetadata, 
			List<FormInputManager> inputsManager)
		{
			var list = value.CastTObject<IEnumerable<object>>();
			this.OutputText = new TextView(Application.Context)
			{
				Text = outputField.Label + ": " + string.Join(", ", list)
			};
			return this.OutputText;
		}
	}

}