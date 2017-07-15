using Android.App;
using Android.Views;

namespace AndroidUiMetadateFramework.Core.Managers
{
	public interface IInputManager
	{
		View GetView(Activity activity);
		object GetValue();
		void SetValue(object value);
	}
}