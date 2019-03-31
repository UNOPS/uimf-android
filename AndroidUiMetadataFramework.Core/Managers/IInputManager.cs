namespace AndroidUiMetadataFramework.Core.Managers
{
	using System.Collections.Generic;
	using Android.Views;
	using AndroidUiMetadataFramework.Core.Models;
	using UiMetadataFramework.Core;

	public interface IInputManager
	{
		object GetValue();
		View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler);
		bool IsValid(InputFieldMetadata inputFieldMetadata);
		void SetValue(object value);
	}
}