namespace AndroidApp
{
    using System.Collections.Generic;
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Support.V7.App;
    using AndroidApp.Forms;
    using AndroidUiMetadataFramework.Core.EventHandlers;
    using AndroidUiMetadataFramework.Core.Inputs;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using AndroidUiMetadataFramework.Core.Outputs;

    [Activity(Label = "GMS", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat")]
    public class MainActivity : AppCompatActivity
    {
        public List<MyFormFragment> AppLayouts = new List<MyFormFragment>();
        public EventHandlerManagerCollection EventManager { get; set; }
        public CustomFormWrapper FormWrapper { get; set; }
        private InputManagerCollection InputManager { get; set; }
        private MyFormHandler MyFormHandler { get; set; }
        private OutputManagerCollection OutputManager { get; set; }
        private UiMetadataWebApi UiMetadataWebApi { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.RegisterManagers();
            this.SetContentView(Resource.Layout.Main);
            this.UiMetadataWebApi = new UiMetadataWebApi
            {
                FormMetadataUrl = "http://10.0.2.2:58337/api/form/metadata",
                MetadataUrl = "http://10.0.2.2:58337/api/form/metadata",
                RunFormUrl = "http://10.0.2.2:58337/api/form/run"
            };
            // this.FormWrapper = new CustomFormWrapper(this, this.AppLayouts, Resource.Id.main_content_frame);
            this.MyFormHandler = new MyFormHandler(this.UiMetadataWebApi, new ManagersCollection(),
                this.FormWrapper);
            var appPreference = new AppSharedPreference(Application.Context);

            if (string.IsNullOrEmpty(appPreference.GetSharedKey("Cookies")))
            {
                var metadata = this.MyFormHandler.GetFormMetadata("login");
                this.FormWrapper.UpdateView(this.MyFormHandler, new FormParameter(metadata));
            }
            else
            {
                var i = new Intent(this, typeof(FormsActivity));
                this.StartActivity(i);
            }
        }

        private void RegisterManagers()
        {
            this.InputManager = new InputManagerCollection();
            this.InputManager.RegisterAssembly(typeof(TextInput).Assembly);

            this.OutputManager = new OutputManagerCollection();
            this.OutputManager.RegisterAssembly(typeof(TextOutput).Assembly);
            this.EventManager = new EventHandlerManagerCollection();
            this.EventManager.RegisterAssembly(typeof(BindToOutputEventHandler).Assembly);
        }
    }
}