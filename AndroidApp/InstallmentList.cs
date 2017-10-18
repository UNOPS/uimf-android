namespace AndroidApp
{
	using System.Collections.Generic;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;
	using UiMetadataFramework.Core;

	[Output(Type = "instalment-list")]
	public class InstallmentList : IOutputManager
	{
		private TextView OutputText { get; set; }

		public View GetView(OutputFieldMetadata outputField,
			object value,
			MyFormHandler myFormHandler,
			FormMetadata formMetadata,
			List<FormInputManager> inputsManager)
		{
			this.OutputText = new TextView(Application.Context) { Text = "installments" };
			return this.OutputText;
		}
	}
}