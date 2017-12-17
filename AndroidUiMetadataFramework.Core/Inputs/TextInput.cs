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

    [Input(Type = "text")]
	public class TextInput : IInputManager
	{
		private EditText InputText { get; set; }

		public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
		{
			this.InputText = new EditText(Application.Context) { InputType = InputTypes.ClassText };
		    myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("EditText", this.InputText);
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