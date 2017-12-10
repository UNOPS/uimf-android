using Android.Views;

namespace AndroidUiMetadataFramework.Core.Managers
{
    using System.Collections.Generic;
    using AndroidUiMetadataFramework.Core.Models;

    public interface IInputManager
	{
		View GetView(IDictionary<string, object> inputCustomProperties,  MyFormHandler myFormHandler);
		object GetValue();
		void SetValue(object value);
	}
}