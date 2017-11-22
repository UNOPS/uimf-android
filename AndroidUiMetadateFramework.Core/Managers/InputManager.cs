using Android.Views;

namespace AndroidUiMetadateFramework.Core.Managers
{
    using AndroidUiMetadateFramework.Core.Models;

    public interface IInputManager
	{
		View GetView(object inputCustomProperties, MyFormHandler myFormHandler);
		object GetValue();
		void SetValue(object value);
	}
}