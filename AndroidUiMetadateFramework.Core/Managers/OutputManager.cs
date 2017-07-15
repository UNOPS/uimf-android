using Android.App;
using Android.Views;

namespace AndroidUiMetadateFramework.Core.Managers
{
	public interface IOutputManager
	{
		View GetView(Activity activity,string name, object value, FormActivity formActivity);
	}
}