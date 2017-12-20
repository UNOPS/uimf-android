namespace AndroidUiMetadataFramework.Core.Inputs
{
    using System.Collections.Generic;
    using Android.App;
	using Android.Text;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadataFramework.Core.Attributes;
	using AndroidUiMetadataFramework.Core.Managers;
	using AndroidUiMetadataFramework.Core.Models;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Core;

	[Input(Type = "password")]
	public class PasswordInput : IInputManager
	{
		private EditText InputText { get; set; }

		public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
		{
			this.InputText = new EditText(Application.Context)
			{
				InputType = InputTypes.TextVariationPassword | InputTypes.ClassText
			};
		    myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("EditText", this.InputText);
            return this.InputText;
		}


		public bool IsValid(InputFieldMetadata inputFieldMetadata)
		{
			return !inputFieldMetadata.Required || string.IsNullOrEmpty(this.GetValue()?.ToString());
		}


		public object GetValue()
		{
			return new Password
			{
				Value = this.InputText.Text
			};
		}

		public void SetValue(object value)
		{
			this.InputText.Text = value?.ToString();
		}

	}
}