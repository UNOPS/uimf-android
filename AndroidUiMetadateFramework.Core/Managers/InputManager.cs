using Android.App;
using Android.Views;

namespace AndroidUiMetadateFramework.Core.Managers
{
	public interface IInputManager
	{
		View GetView(object inputCustomProperties);
		object GetValue();
		void SetValue(object value);
	}
}