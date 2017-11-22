namespace AndroidUiMetadateFramework.Core.Inputs
{
	using Android.App;
	using Android.Text;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;

    [Input(Type = "text")]
	public class TextInput : IInputManager
	{
		private EditText InputText { get; set; }

		public View GetView(object inputCustomProperties, MyFormHandler myFormHandler)
		{
			this.InputText = new EditText(Application.Context) { InputType = InputTypes.ClassText };
			return this.InputText;
		}

		public object GetValue()
		{
			return this.InputText.Text;
		}

		public void SetValue(object value)
		{
			this.InputText.Text = value?.ToString();
		}
	}
}