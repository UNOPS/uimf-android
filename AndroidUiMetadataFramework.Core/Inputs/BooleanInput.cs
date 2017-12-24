namespace AndroidUiMetadataFramework.Core.Inputs
{
    using System.Collections.Generic;
    using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadataFramework.Core.Attributes;
	using AndroidUiMetadataFramework.Core.Managers;
	using AndroidUiMetadataFramework.Core.Models;
	using UiMetadataFramework.Core;

	[Input(Type = "boolean")]
	public class BooleanInput : IInputManager
	{
		private CheckBox InputBoolean { get; set; }

		public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
		{
			this.InputBoolean = new CheckBox(Application.Context);
            myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("CheckBox", this.InputBoolean);
			return this.InputBoolean;
		}

		public bool IsValid(InputFieldMetadata inputFieldMetadata)
		{
			return !inputFieldMetadata.Required || !string.IsNullOrEmpty(this.GetValue()?.ToString());
		}

		public object GetValue()
		{
			return this.InputBoolean.Checked;
		}

		public void SetValue(object value)
		{
            var isChecked = value.CastTObject<bool>();
            this.InputBoolean.Checked = isChecked;
		}
	}
}