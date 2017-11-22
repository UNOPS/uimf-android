namespace AndroidUiMetadateFramework.Core.Inputs
{
	using Android.App;
	using Android.Text;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;
	using UiMetadataFramework.Basic.Input;

	[Input(Type = "password")]
	public class PasswordInput : IInputManager
	{
		private EditText InputText { get; set; }

		public View GetView(object inputCustomProperties, MyFormHandler myFormHandler)
		{
			this.InputText = new EditText(Application.Context)
			{
				InputType = InputTypes.TextVariationPassword | InputTypes.ClassText
			};
			return this.InputText;
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