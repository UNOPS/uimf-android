namespace AndroidUiMetadataFramework.Core.Inputs
{
    using System.Collections.Generic;
    using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadataFramework.Core.Attributes;
	using AndroidUiMetadataFramework.Core.Managers;
	using AndroidUiMetadataFramework.Core.Models;

	[Input(Type = "boolean")]
	public class BooleanInput : IInputManager
	{
		private CheckBox InputBoolean { get; set; }

		public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
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