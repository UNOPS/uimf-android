namespace AndroidUiMetadateFramework.Core.Outputs
{
	using System.Collections.Generic;
	using Android.App;
	using Android.Graphics;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;
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
			FormLink formLink = value.CastTObject<FormLink>();
			this.Layout = new LinearLayout(Application.Context){Orientation = Orientation.Horizontal};
			this.Layout.AddView(new TextView(Application.Context){Text = outputField.Label +": "});
			var text = this.InitializeLink(formLink, myFormHandler);
			
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
				if (myFormHandler.AllFormsMetadata != null)
				{
					var formMetadata = myFormHandler.AllFormsMetadata[btn.Form];
					//myFormHandler.ReplaceFragment(formMetadata, btn.InputFieldValues);
				    myFormHandler.FormWrapper.UpdateView(myFormHandler, formMetadata, btn.InputFieldValues);
                }
				else
				{
					await myFormHandler.StartIFormAsync(btn.Form, btn.InputFieldValues);
				}

			};
			return text;
		}
	}
}