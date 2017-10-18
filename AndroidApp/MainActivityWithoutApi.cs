namespace AndroidApp
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Android.App;
	using Android.OS;
	using Android.Support.V7.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Inputs;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;
	using AndroidUiMetadateFramework.Core.Outputs;
	using App.Core;
	using MediatR;
	using StructureMap;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;

	[Activity(Label = "My Magic App", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat")]
	public class MainActivityWithoutApi : AppCompatActivity, View.IOnClickListener
	{
		public  List<MyFormWrapper> AppLayouts = new List<MyFormWrapper>();
		private Container Container { get;} = new Container();
		private Button Btn { get; set; }
		private MyFormHandler MyFormHandler { get; set; }
		private FormRegister FormRegister { get; set; }
		private InputManagerCollection InputManager { get; set; }
		private OutputManagerCollection OutputManager { get; set; }
		
		public async void OnClick(View v)
		{
			if (v.Id == Resource.Id.button1)
			{
				await this.MyFormHandler.StartIFormAsync("Unops.Spgs.Web.Forms.Login");
			}
		}

		public FormRegister GetRegisterForms()
		{
			try
			{
				var binder = new MetadataBinder();
				binder.RegisterAssembly(typeof(StringOutputFieldBinding).GetTypeInfo().Assembly);
				binder.RegisterAssembly(typeof(OutputFieldBinding).GetTypeInfo().Assembly);
				this.FormRegister = new FormRegister(binder);
				this.FormRegister.RegisterAssembly(typeof(DoMagic).GetTypeInfo().Assembly);
			}
			catch (Exception ex)
			{
				throw new Java.Lang.Exception(ex.Message);
			}

			return this.FormRegister;
		}

		public override void OnBackPressed()
		{
			this.AppLayouts.RemoveAt(this.AppLayouts.Count - 1);
			if (this.AppLayouts.Count != 0)
			{
			//	this.SetContentView(this.AppLayouts[this.AppLayouts.Count - 1]);
			}
			else
			{
				this.Finish();
			}
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			this.SetContentView(Resource.Layout.Main);
			//this.AppLayouts.Add(this.FindViewById(Resource.Id.mainLayout));
			this.RegisterInputOutputManagers();
			this.ConfigureContainer();
			this.FormRegister = this.Container.GetInstance<FormRegister>();
			this.MyFormHandler = new MyFormHandler(this, this.Container.GetInstance<IMediator>(), this.FormRegister, this.InputManager, this.OutputManager,
				this.AppLayouts);
			this.Btn = this.FindViewById<Button>(Resource.Id.button1);
			this.Btn.SetOnClickListener(this);
		}

		private void ConfigureContainer()
		{
			this.Container.Configure(config =>
			{
				config.For<IMediator>().Use<Mediator>();
				config.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
				config.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
				config.For<FormRegister>().Singleton().Use(() => this.GetRegisterForms());

				config.Scan(_ =>
				{
					_.TheCallingAssembly();
					_.AssemblyContainingType<DoMagic>();
					_.AssemblyContainingType<InvokeForm>();
					_.WithDefaultConventions();
					_.AddAllTypesOf(typeof(IRequestHandler<,>));
					_.AddAllTypesOf(typeof(INotificationHandler<>));
					_.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
					_.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>));
					_.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
					_.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
				});
			});
		}

		private void RegisterInputOutputManagers()
		{
			this.InputManager = new InputManagerCollection();
			this.InputManager.RegisterAssembly(typeof(TextInput).Assembly);

			this.OutputManager = new OutputManagerCollection();
			this.OutputManager.RegisterAssembly(typeof(TextOutput).Assembly);
			this.OutputManager.RegisterAssembly(typeof(InstallmentList).Assembly);
		}
	}
}