using Android.App;
using Android.Views;

namespace AndroidUiMetadateFramework.Core.Managers
{
	public interface IOutputManager
	{
		View GetView(string name, object value, FormActivity formActivity);
	}
}