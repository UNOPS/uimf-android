namespace AndroidUiMetadataFramework.Core.Inputs
{
	using System;
	using System.Collections.Generic;
	using Android.App;
	using Android.Text;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadataFramework.Core.Attributes;
	using AndroidUiMetadataFramework.Core.Managers;
	using AndroidUiMetadataFramework.Core.Models;

    [Input(Type = "number")]
	public class NumericInput : IInputManager
	{
		private EditText InputText { get; set; }

		public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
		{
			this.InputText = new EditText(Application.Context) { InputType = InputTypes.ClassNumber };
		    myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("EditText", this.InputText);
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