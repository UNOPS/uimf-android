namespace AndroidUiMetadateFramework.Core.Models
{
	using Android.Preferences;
	using Android.Content;

	public class AppSharedPreference
	{
		private ISharedPreferences SharedPrefs { get; }
		private ISharedPreferencesEditor PrefsEditor { get; }
		private Context Context { get; }

		public AppSharedPreference(Context context)
		{
			this.Context = context;
			this.SharedPrefs = PreferenceManager.GetDefaultSharedPreferences(this.Context);
			this.PrefsEditor = this.SharedPrefs.Edit();
		}

		public void SetSharedKey(string key, string value)
		{
			this.PrefsEditor.PutString(key, value);
			this.PrefsEditor.Commit();
		}

		public string GetSharedKey(string key)
		{
			return this.SharedPrefs.GetString(key, "");
		}
	}
}