﻿namespace AndroidApp
{
	using System.Collections.Generic;
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
	using AndroidUiMetadateFramework.Core.Inputs;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;
	using AndroidUiMetadateFramework.Core.Outputs;
	using Java.Lang;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using UiMetadataFramework.Core;

	[Activity(Label = "My Magic App", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat")]
	public class FormsActivity : AppCompatActivity, DrawerListAdapter.IOnItemClickListener
	{
		public List<MyFormWrapper> AppLayouts = new List<MyFormWrapper>();
		private DrawerLayout DrawerLayout { get; set; }
		private RecyclerView DrawerList { get; set; }
		private ActionBarDrawerToggle DrawerToggle { get; set; }
		private MyFormHandler MyFormHandler { get; set; }
		private InputManagerCollection InputManager { get; set; }
		private List<object> MenuItems { get; set; }
		private OutputManagerCollection OutputManager { get; set; }
		private List<string> MenuTitles { get; set; }
		private UiMetadataWebApi UiMetadataWebApi { get; set; }
		private Dictionary<string,FormMetadata> AllForms { get; set; }

		/* The click listener for RecyclerView in the navigation drawer */
		public void OnClick(View view, int position)
		{
			this.SelectItem(position);
		}

		public override void OnBackPressed()
		{
			this.AppLayouts.RemoveAt(this.AppLayouts.Count - 1);
			if (this.AppLayouts.Count != 0)
			{
				this.AppLayouts[this.AppLayouts.Count - 1].UpdateFragment(Resource.Id.content_frame);
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

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			this.SetContentView(Resource.Layout.Magic);
			this.UiMetadataWebApi = new UiMetadataWebApi
			{
				FormMetadataUrl = "http://10.0.2.2:50072/api/form/metadata",
				MetadataUrl = "http://10.0.2.2:50072/api/form/metadata",
				RunFormUrl = "http://10.0.2.2:50072/api/form/run"
			};
			this.RegisterInputOutputManagers();			
			this.GetAllFormsMetadata();
			this.MyFormHandler = new MyFormHandler(this, this.UiMetadataWebApi, this.InputManager, this.OutputManager, 
				 this.AppLayouts, this.AllForms, Resource.Id.content_frame);
			this.InitializeDrawerLayout();
			if(this.AppLayouts.Count == 0)
			this.AppLayouts.Add(new MyFormWrapper(this));
		}

		private void InitializeDrawerLayout()
		{
			this.DrawerLayout = this.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			this.DrawerList = this.FindViewById<RecyclerView>(Resource.Id.left_drawer);

			// set a custom shadow that overlays the main content when the drawer opens
			this.DrawerLayout.SetDrawerShadow(Resource.Drawable.drawer_shadow, GravityCompat.Start);
			this.DrawerList.HasFixedSize = true;
			this.DrawerList.SetLayoutManager(new LinearLayoutManager(this));
			this.DrawerList.SetAdapter(new DrawerListAdapter(this.MenuTitles.ToArray(), this));
			this.DrawerList.SetBackgroundColor(Color.White);
			this.SupportActionBar.Title = "My ActionBar";
			this.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			this.SupportActionBar.SetHomeButtonEnabled(true);

			this.DrawerToggle = new MyActionBarDrawerToggle(this, this.DrawerLayout,
				Resource.String.drawer_open,
				Resource.String.drawer_close,
				this.Title);

			this.DrawerLayout.SetDrawerListener(this.DrawerToggle);
		}

		private void GetAllFormsMetadata()
		{
			this.AllForms = new Dictionary<string, FormMetadata>();
			var appPreference = new AppSharedPreference(Application.Context);
			var result = Task.Run(
				() => UiMetadataHttpRequestHelper.GetAllFormsMetadata(this.UiMetadataWebApi.MetadataUrl, appPreference.GetSharedKey("Cookies")));
			var metadata = JsonConvert.DeserializeObject<MyForms>(result.Result);

			this.MenuItems = new List<object>();
			this.MenuTitles = new List<string>();

			foreach (var menuItem in metadata.Menus)
			{
				if (!string.IsNullOrEmpty(menuItem.Name))
				{					
					var existingForms = false;
					foreach (var form in metadata.Forms)
					{
						if (form.CustomProperties != null)
						{
							var customeProperties = (JObject)form.CustomProperties;
							var menuName = customeProperties.GetValue("menu").ToString();
							if (menuName.Equals(menuItem.Name))
							{
								if (!existingForms)
								{
									this.MenuItems.Add(menuItem.Name);
									this.MenuTitles.Add(menuItem.Name);
								}
								existingForms = true;
								this.MenuItems.Add(form);
								this.MenuTitles.Add("\t" + form.Label);								
							}
						}
						if (!this.AllForms.ContainsKey(form.Id))
						{
							this.AllForms.Add(form.Id, form);
						}
						
					}
					
				}
			}
		}

		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			// Sync the toggle state after onRestoreInstanceState has occurred.
			this.DrawerToggle.SyncState();
		}

		protected override void OnTitleChanged(ICharSequence title, Color color)
		{
			this.SupportActionBar.Title = title.ToString();
		}

		private void RegisterInputOutputManagers()
		{
			this.InputManager = new InputManagerCollection();
			this.InputManager.RegisterAssembly(typeof(TextInput).Assembly);

			this.OutputManager = new OutputManagerCollection();
			this.OutputManager.RegisterAssembly(typeof(TextOutput).Assembly);
			this.OutputManager.RegisterAssembly(typeof(InstallmentList).Assembly);
		}

		private void SelectItem(int position)
		{
			if (this.MenuItems[position].GetType() == typeof(FormMetadata))
			{
				this.MyFormHandler.ReplaceFragment((FormMetadata)this.MenuItems[position]);
				// update selected item title, then close the drawer
				this.Title = this.MenuTitles[position];
				this.DrawerLayout.CloseDrawer(this.DrawerList);
			}
		}
	}
}