namespace AndroidUiMetadateFramework.Core.Inputs
{
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;

	[Input(Type = "boolean")]
	public class BooleanInput : IInputManager
	{
		private CheckBox InputBoolean { get; set; }

		public View GetView(object inputCustomProperties, MyFormHandler myFormHandler)
		{
			this.InputBoolean = new CheckBox(Application.Context);
			return this.InputBoolean;
		}

		public object GetValue()
		{
			return this.InputBoolean.Checked;
		}

		public void SetValue(object value)
		{
			this.InputBoolean.Checked = value.CastTObject<bool>();
		}
	}
}