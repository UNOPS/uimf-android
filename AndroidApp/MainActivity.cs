namespace AndroidApp
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Android.App;
	using Android.OS;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core;
	using AndroidUiMetadateFramework.Core.Inputs;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Outputs;
	using App.Core;
	using MediatR;
	using StructureMap;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;

	[Activity(Label = "My Magic App", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity, View.IOnClickListener
	{
		public List<View> AppLayouts = new List<View>();
		private readonly Container Container = new Container();
		private Button Btn { get; set; }
		private FormActivity FormActivity { get; set; }
		private FormRegister FormRegister { get; set; }
		private InputManagerCollection InputManager { get; set; }
		private OutputManagerCollection OutputManager { get; set; }

		public async void OnClick(View v)
		{
			if (v.Id == Resource.Id.button1)
			{
				await this.FormActivity.StartIForm(typeof(DoMagic));
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
				this.SetContentView(this.AppLayouts[this.AppLayouts.Count - 1]);
			}
			else
			{
				this.Finish();
			}
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			this.SetContentView(Resource.Layout.Main);
			this.AppLayouts.Add(this.FindViewById(Resource.Id.mainLayout));
			this.RegisterInputOutputManagers();
			this.ConfigureContainer();
			this.FormRegister = this.Container.GetInstance<FormRegister>();
			this.FormActivity = new FormActivity(this, this.Container.GetInstance<IMediator>(), this.FormRegister, this.InputManager, this.OutputManager,
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
			this.InputManager.RegisterAssembly(typeof(NumericInput).Assembly);
			this.InputManager.RegisterAssembly(typeof(DateInput).Assembly);

			this.OutputManager = new OutputManagerCollection();
			this.OutputManager.RegisterAssembly(typeof(TextOutput).Assembly);
			this.OutputManager.RegisterAssembly(typeof(NumericOutput).Assembly);
			this.OutputManager.RegisterAssembly(typeof(DateOutput).Assembly);
			this.OutputManager.RegisterAssembly(typeof(TableOutput).Assembly);
		}
	}
}