namespace AndroidApp
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content.Res;
    using Android.Graphics;
    using Android.OS;
    using Android.Support.V4.View;
    using Android.Support.V4.Widget;
    using Android.Support.V7.App;
    using Android.Support.V7.Widget;
    using Android.Views;
    using Android.Widget;
    using AndroidApp.Forms;
    using AndroidApp.Forms.Inputs;
    using AndroidApp.Forms.Menu;
    using AndroidApp.Forms.Outputs;
    using AndroidApp.Styles;
    using AndroidUiMetadataFramework.Core.EventHandlers;
    using AndroidUiMetadataFramework.Core.Inputs;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using AndroidUiMetadataFramework.Core.Outputs;
    using App.Style;
    using Java.Lang;
    using Newtonsoft.Json;
    using UiMetadataFramework.Core;

    [Activity(Label = "GMS", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat")]
    public class FormsActivity : AppCompatActivity, DrawerListAdapter.IOnItemClickListener
    {
        public List<MyFormFragment> AppLayouts = new List<MyFormFragment>();

        public CustomFormWrapper FormWrapper { get; set; }
        public Handler Handler { get; } = new Handler();
        public ManagersCollection ManagersCollection { get; set; }
        public ProgressBar ProgressBar { get; set; }
        private Dictionary<string, FormMetadata> AllForms { get; set; } = new Dictionary<string, FormMetadata>();
        private DrawerLayout DrawerLayout { get; set; }
        private RecyclerView DrawerList { get; set; }
        private ActionBarDrawerToggle DrawerToggle { get; set; }
        private List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private MyFormHandler MyFormHandler { get; set; }
        private UiMetadataWebApi UiMetadataWebApi { get; set; }

        /* The click listener for RecyclerView in the navigation drawer */
        public void OnClick(View view, int position)
        {
            this.SelectItem(position);
        }

        public void FetchMetadata()
        {
            new Thread(new Runnable(() =>
            {
                this.ProgressBar.Visibility = ViewStates.Visible;
                this.GetAllFormsMetadata();
                this.RunOnUiThread(this.RefreshDrawerList);

                this.Handler.Post(() => { this.ProgressBar.Visibility = ViewStates.Invisible; });
            })).Start();
        }

        public void GetAllFormsMetadata()
        {
            this.AllForms = new Dictionary<string, FormMetadata>();
            var appPreference = new AppSharedPreference(Application.Context);
            this.MenuItems = new List<MenuItem>();
            this.MyFormHandler.AllFormsMetadata = new Dictionary<string, FormMetadata>();

            try
            {
                var result = Task.Run(
                    () => UiMetadataHttpRequestHelper.GetAllFormsMetadata(this.UiMetadataWebApi.MetadataUrl,
                        appPreference.GetSharedKey("Cookies")));
                var metadata = JsonConvert.DeserializeObject<MyForms>(result.Result);
                var orderedMenu = metadata.Menus.OrderBy(a => a.OrderIndex);
                var orderedForms = metadata.Forms
                    .OrderBy(a => a.CustomProperties != null ? a.CustomProperties?.GetCustomProperty<long>("menuOrderIndex") : 0)
                    .ToList();
                foreach (var menuItem in orderedMenu)
                {
                    var existingForms = false;
                    foreach (var form in orderedForms)
                    {
                        if (form.CustomProperties != null && this.MenuItems.All(a => a.FormMetadata != form))
                        {
                            var menuName = form.CustomProperties.GetCustomProperty<string>("menu");
                            if (!string.IsNullOrEmpty(menuItem.Name))
                            {
                                if (menuName != null && (menuName.Equals(menuItem.Name) || menuName == ""))
                                {
                                    if (!existingForms && menuName != "")
                                    {
                                        this.MenuItems.Add(new MenuItem(menuItem.Name));
                                    }
                                    existingForms = true;
                                    this.MenuItems.Add(new MenuItem("\t" + form.Label, form));
                                }
                            }
                        }
                        if (!this.AllForms.ContainsKey(form.Id))
                        {
                            this.AllForms.Add(form.Id, form);
                        }

                        if (!this.MyFormHandler.AllFormsMetadata.ContainsKey(form.Id))
                        {
                            this.MyFormHandler.AllFormsMetadata.Add(form.Id, form);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                this.RunOnUiThread(() => { Toast.MakeText(Application.Context, "Server is not available in this moment", ToastLength.Long).Show(); });
            }
        }

        public void InitializeDrawerLayout()
        {
            this.DrawerLayout = this.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            this.DrawerList = this.FindViewById<RecyclerView>(Resource.Id.left_drawer);

            // set a custom shadow that overlays the main content when the drawer opens
            this.DrawerLayout.SetDrawerShadow(Resource.Drawable.drawer_shadow, GravityCompat.Start);
            this.DrawerList.HasFixedSize = true;
            this.DrawerList.SetLayoutManager(new LinearLayoutManager(this));
            this.DrawerList.SetAdapter(new DrawerListAdapter(this.MenuItems, this));
            this.DrawerList.SetBackgroundColor(Color.ParseColor("#343d49"));
            this.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            this.SupportActionBar.SetHomeButtonEnabled(true);

            this.DrawerToggle = new MyActionBarDrawerToggle(this, this.DrawerLayout,
                Resource.String.drawer_open,
                Resource.String.drawer_close,
                this.Title);

            this.DrawerLayout.AddDrawerListener(this.DrawerToggle);
        }

        public override void OnBackPressed()
        {
            this.AppLayouts.RemoveAt(this.AppLayouts.Count - 1);
            if (this.AppLayouts.Count != 0)
            {
                this.AppLayouts[this.AppLayouts.Count - 1].UpdateFragment();
            }
            else
            {
                this.Finish();
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            // Pass any configuration change to the drawer toggls
            this.DrawerToggle.OnConfigurationChanged(newConfig);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (this.DrawerToggle.OnOptionsItemSelected(item))
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void RefreshDrawerList()
        {
            this.DrawerList.SetAdapter(new DrawerListAdapter(this.MenuItems, this));
            this.DrawerList.GetAdapter().NotifyDataSetChanged();
        }

        public Dictionary<string, FormMetadata> Reload()
        {
            this.GetAllFormsMetadata();
            this.RefreshDrawerList();
            return this.AllForms;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.SetContentView(Resource.Layout.Magic);
            this.UiMetadataWebApi = new UiMetadataWebApi
            {
                FormMetadataUrl = "http://10.0.2.2:58337/api/form/metadata",
                MetadataUrl = "http://10.0.2.2:58337/api/form/metadata",
                RunFormUrl = "http://10.0.2.2:58337/api/form/run"
            };
            this.RegisterManagers();
            this.ProgressBar = this.FindViewById<ProgressBar>(Resource.Id.progressBar);
            this.FormWrapper = new CustomFormWrapper(this, this.AppLayouts);
            this.MyFormHandler = new MyFormHandler(this.UiMetadataWebApi, this.ManagersCollection, this.FormWrapper, this.AllForms);
            this.InitializeDrawerLayout();
            if (this.AppLayouts.Count == 0)
            {
                this.AppLayouts.Add(new MyFormFragment(this));
            }

            var refresher = this.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
#pragma warning disable 618
            refresher.SetColorScheme(Resource.Color.blue);
#pragma warning restore 618
            refresher.Refresh += delegate
            {
                this.GetAllFormsMetadata();
                this.RefreshDrawerList();
                var wrapper = this.AppLayouts[this.AppLayouts.Count - 1];
                this.FormWrapper.UpdateView(wrapper.MyFormHandler, wrapper.FormParameter, wrapper.SubmitAction);
                this.AppLayouts.RemoveAt(this.AppLayouts.Count - 1);

                refresher.Refreshing = false;
            };

            this.FetchMetadata();
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            // Sync the toggle state after onRestoreInstanceState has occurred.
            this.DrawerToggle.SyncState();
        }

        protected override void OnTitleChanged(ICharSequence title, Color color)
        {
            this.SupportActionBar.Title = title?.ToString();
        }

        private void RegisterManagers()
        {
            var inputManager = new InputManagerCollection();
            inputManager.RegisterAssembly(typeof(TextInput).Assembly);
            inputManager.RegisterAssembly(typeof(DynamicFormInput).Assembly);

            var outputManager = new OutputManagerCollection();
            outputManager.RegisterAssembly(typeof(TextOutput).Assembly);
            outputManager.RegisterAssembly(typeof(ObjectListOutput).Assembly);

            var eventManager = new EventHandlerManagerCollection();
            eventManager.RegisterAssembly(typeof(BindToOutputEventHandler).Assembly);

            var styleRegister = new StyleRegister();
            styleRegister.RegisterAssembly(typeof(TextViewStyle).Assembly);
            this.ManagersCollection = new ManagersCollection
            {
                InputManagerCollection = inputManager,
                OutputManagerCollection = outputManager,
                EventHandlerManagerCollection = eventManager,
                StyleRegister = styleRegister
            };
        }

        private void SelectItem(int position)
        {
            if (this.MenuItems[position].FormMetadata != null)
            {
                // update selected item title, then close the drawer
                this.DrawerLayout.CloseDrawer(this.DrawerList);
                this.FormWrapper.UpdateView(this.MyFormHandler, new FormParameter(this.MenuItems[position].FormMetadata));
            }
        }
    }
}