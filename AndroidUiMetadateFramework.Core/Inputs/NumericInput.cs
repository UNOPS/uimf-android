namespace AndroidUiMetadateFramework.Core.Inputs
{
	using System;
	using Android.App;
	using Android.Text;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;

	[Input(Type = "number")]
	public class NumericInput : IInputManager
	{
		private EditText InputText { get; set; }

		public View GetView( object inputCustomProperties)
		{
			this.InputText = new EditText(Application.Context) { InputType = InputTypes.ClassNumber };
			return this.InputText;
		}

		public object GetValue()
		{
			if (!string.IsNullOrEmpty(this.InputText.Text))
			{
				return Convert.ToInt32(this.InputText.Text);
			}
			return null;
		}

		public void SetValue(object value)
		{
			this.InputText.Text = value?.ToString();
		}
	}
}