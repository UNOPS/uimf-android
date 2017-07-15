namespace AndroidUiMetadateFramework.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Managers;
	using MediatR;
	using Newtonsoft.Json.Linq;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.MediatR;

	public class FormActivity
	{
		public FormActivity(Activity activity,
			IMediator mediator,
			FormRegister formRegister,
			InputManagerCollection inputManager,
			OutputManagerCollection outputManager,
			List<View> appLayouts)
		{
			this.Activity = activity;
			this.Mediator = mediator;
			this.InputsManager = new List<FormInputManager>();
			this.InputManagerCollection = inputManager;
			this.OutputManagerCollection = outputManager;
			this.FormRegister = formRegister;
			this.AppLayouts = appLayouts;
		}

		public List<View> AppLayouts { get; set; }
		public IDictionary<string, object> InputFieldValues { get; set; }
		private Activity Activity { get; }
		private FormMetadata FormMetadata { get; set; }
		private FormRegister FormRegister { get; }
		private InputManagerCollection InputManagerCollection { get; }
		private List<FormInputManager> InputsManager { get; }
		private IMediator Mediator { get; }
		private OutputManagerCollection OutputManagerCollection { get; }
		private ProgressBar ProgressBar { get; set; }

		public void DrawInputs(LinearLayout layout)
		{
			var orderedInputs = this.FormMetadata.InputFields.OrderBy(a => a.OrderIndex).ToList();
			var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
				ViewGroup.LayoutParams.WrapContent);
			this.InputsManager.Clear();
			foreach (var input in orderedInputs)
			{
				if (!input.Hidden)
				{
					var label = new TextView(this.Activity) { Text = input.Label };
					layout.AddView(label, param);
				}

				var manager = this.InputManagerCollection.GetManager(input.Type);

				var view = manager.GetView(this.Activity);
				if (this.InputFieldValues != null)
				{
					var value = this.InputFieldValues.SingleOrDefault(a => a.Key.Equals(input.Id)).Value;
					if (value != null)
					{
						manager.SetValue(value);
					}
				}
				this.InputsManager.Add(new FormInputManager(input, manager, view));
				if (input.DefaultValue != null)
				{
					manager.SetValue(input.DefaultValue.Id);
				}
				if (input.Hidden)
				{
					view.Visibility = ViewStates.Gone;
				}
				layout.AddView(view, param);
			}
		}

		public async Task<View> StartIForm(Type form, IDictionary<string, object> inputFieldValues = null)
		{
			this.FormMetadata = this.FormRegister.GetFormInfo(form.FullName)?.Metadata;
			this.InputFieldValues = inputFieldValues;
			return await this.StartIForm();
		}

		public async Task<View> StartIForm(string form, IDictionary<string, object> inputFieldValues = null)
		{
			this.FormMetadata = this.FormRegister.GetFormInfo(form)?.Metadata;
			this.InputFieldValues = inputFieldValues;
			return await this.StartIForm();
		}

		public async Task<View> StartIForm()
		{
			try
			{
				var layout = await this.DrawIFormAsync();
				if (layout != null)
				{
					this.Activity.SetContentView(layout);
					this.AppLayouts.Add(layout);
				}
				return layout;
			}
			catch (Exception ex)
			{
				Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
				return null;
			}
		}

		private async Task<View> DrawIFormAsync()
		{
			var scroll = new ScrollView(this.Activity);
			var linearLayout = new LinearLayout(this.Activity) { Orientation = Orientation.Vertical };
			linearLayout.SetPadding(20, 10, 20, 10);
			var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
				ViewGroup.LayoutParams.WrapContent);

			this.ProgressBar = new ProgressBar(this.Activity);
			var progressParam = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
				ViewGroup.LayoutParams.WrapContent) { Gravity = GravityFlags.Center };
			this.ProgressBar.LayoutParameters = progressParam;
			this.ProgressBar.Visibility = ViewStates.Gone;

			if (this.FormMetadata != null)
			{
				InvokeForm.Response result = null;
				var resultLayout = new LinearLayout(this.Activity) { Orientation = Orientation.Vertical };
				resultLayout.SetPadding(20, 10, 20, 10);
				if (this.FormMetadata.InputFields.Count > 0)
				{
					this.DrawInputs(linearLayout);
					if (this.FormMetadata.InputFields.Count(a => !a.Hidden) > 0)
					{
						var btn = new Button(this.Activity) { Text = "Submit" };
						linearLayout.AddView(btn, param);
						btn.Click += async (sender, args) =>
						{
							var valid = this.ValidateForm();
							if (valid)
							{
								resultLayout.RemoveAllViews();

								result = await this.HandelForm();
								this.DrawOutput(resultLayout, result);

								//Activity.RunOnUiThread(() =>
								//{
								//	Toast.MakeText(Activity, "Data Saved", ToastLength.Long).Show();
								//});
							}
						};
					}
				}
				//linearLayout.AddView(ProgressBar);
				if (this.FormMetadata.PostOnLoad)
				{
					var valid = this.ValidateForm();
					if (valid)
					{
						if (this.ProgressBar != null)
						{
							this.ProgressBar.Visibility = ViewStates.Visible;
						}
						resultLayout.RemoveAllViews();
						result = await this.HandelForm();
						this.DrawOutput(resultLayout, result);
					}
				}
				linearLayout.AddView(resultLayout, param);
				scroll.AddView(linearLayout, param);
			}
			return scroll;
		}

		private void DrawOutput(LinearLayout layout, InvokeForm.Response result)
		{
			if (result != null)
			{
				var orderedOutputs = this.FormMetadata.OutputFields.OrderBy(a => a.OrderIndex).ToList();
				var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
					ViewGroup.LayoutParams.WrapContent);
				foreach (var output in orderedOutputs)
				{
					var propertyInfo = result.Data.GetType().GetProperty(output.Id);
					if (propertyInfo != null)
					{
						if (!output.Hidden)
						{
							var value = propertyInfo.GetValue(result.Data, null);
							var manager = this.OutputManagerCollection.GetManager(output.Type);
							var view = manager.GetView(this.Activity, output.Label, value, this);
							view.SetPadding(0, 10, 0, 10);
							layout.AddView(view, param);
						}
					}
				}
			}
		}

		private object GetFormValues()
		{
			var jsonObject = new JObject();

			foreach (var inputManager in this.InputsManager)
			{
				var value = inputManager.Manager.GetValue();
				if (value != null)
				{
					jsonObject.Add(inputManager.Input.Id, value.ToString());
				}
			}
			return jsonObject;
		}

		private async Task<InvokeForm.Response> HandelForm()
		{
			try
			{
				var obj = this.GetFormValues();
				var request = new InvokeForm.Request
				{
					Form = this.FormMetadata.Id,
					InputFieldValues = obj
				};

				var response = await this.Mediator.Send(request);

				return new InvokeForm.Response
				{
					RequestId = request.RequestId,
					Data = response.Data
				};
			}
			catch (Exception ex)
			{
				Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
			}

			return new InvokeForm.Response();
		}

		private bool ValidateForm()
		{
			var valid = true;
			foreach (var inputManager in this.InputsManager)
			{
				var value = inputManager.Manager.GetValue();
				if (inputManager.Input.Required)
				{
					if (string.IsNullOrEmpty(value?.ToString()))
					{
						valid = false;
						inputManager.View.SetBackgroundResource(Resource.Drawable.ValidationBorders);
					}
				}
			}

			return valid;
		}

		public class FormInputManager
		{
			public FormInputManager(InputFieldMetadata input, IInputManager manager, View view)
			{
				this.Input = input;
				this.Manager = manager;
				this.View = view;
			}

			public InputFieldMetadata Input { get; set; }
			public IInputManager Manager { get; set; }
			public View View { get; set; }
		}
	}
}