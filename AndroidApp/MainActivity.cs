namespace AndroidApp
{
	using System.Collections.Generic;
	using Android.App;
	using Android.Content;
	using Android.OS;
	using Android.Support.V7.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Inputs;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;
	using AndroidUiMetadateFramework.Core.Outputs;

	[Activity(Label = "My Magic App", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat")]
	public class MainActivity : AppCompatActivity, View.IOnClickListener
	{
		public List<MyFormWrapper> AppLayouts = new List<MyFormWrapper>();
		private Button Btn { get; set; }
		private MyFormHandler MyFormHandler { get; set; }
		private InputManagerCollection InputManager { get; set; }
		private OutputManagerCollection OutputManager { get; set; }
		private UiMetadataWebApi UiMetadataWebApi { get; set; }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			this.SetContentView(Resource.Layout.Main);
			this.RegisterInputOutputManagers();
			this.UiMetadataWebApi = new UiMetadataWebApi
			{
				FormMetadataUrl = "http://10.0.2.2:50072/api/form/metadata",
				MetadataUrl = "http://10.0.2.2:50072/api/form/metadata",
				RunFormUrl = "http://10.0.2.2:50072/api/form/run"
			};
			this.MyFormHandler = new MyFormHandler(this, this.UiMetadataWebApi, this.InputManager, this.OutputManager, this.AppLayouts);
			this.Btn = this.FindViewById<Button>(Resource.Id.button1);
			this.Btn.SetOnClickListener(this);
			var appPreference = new AppSharedPreference(Application.Context);
			if (string.IsNullOrEmpty(appPreference.GetSharedKey("Cookies")))
			{
				var view = this.MyFormHandler.StartIFormAsync("login");
			}				 
			else
			{
				Intent i = new Intent(this, typeof(FormsActivity));
				this.StartActivity(i);
			}
		}

		private void RegisterInputOutputManagers()
		{
			this.InputManager = new InputManagerCollection();
			this.InputManager.RegisterAssembly(typeof(TextInput).Assembly);

			this.OutputManager = new OutputManagerCollection();
			this.OutputManager.RegisterAssembly(typeof(TextOutput).Assembly);
			this.OutputManager.RegisterAssembly(typeof(InstallmentList).Assembly);
		}

		public void OnClick(View v)
		{
			throw new System.NotImplementedException();
		}
	}
}