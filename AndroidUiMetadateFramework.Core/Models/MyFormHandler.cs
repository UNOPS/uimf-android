namespace AndroidUiMetadateFramework.Core.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Managers;
	using Humanizer;
	using MediatR;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using UiMetadataFramework.Basic.Response;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.MediatR;

	public class MyFormHandler
	{
		public MyFormHandler(Activity activity,
			IMediator mediator,
			FormRegister formRegister,
			InputManagerCollection inputManager,
			OutputManagerCollection outputManager,
			List<MyFormWrapper> appFragments)
		{
			this.Activity = activity;
			this.Mediator = mediator;
			this.InputManagerCollection = inputManager;
			this.OutputManagerCollection = outputManager;
			this.FormRegister = formRegister;
			this.AppFragments = appFragments;
		}

		public MyFormHandler(Activity activity,
			UiMetadataWebApi uiMetadataWebApi,
			InputManagerCollection inputManager,
			OutputManagerCollection outputManager,
			List<MyFormWrapper> appFragments,
			Dictionary<string, FormMetadata> allForms = null,
			int? contentResourceId = null)
		{
			this.Activity = activity;
			this.InputManagerCollection = inputManager;
			this.OutputManagerCollection = outputManager;
			this.AllFormsMetadata = allForms;
			this.UiMetadataWebApi = uiMetadataWebApi;
			this.AppPreference = new AppSharedPreference(Application.Context);
			this.AppFragments = appFragments;
			this.ContentResourceId = contentResourceId;
		}

		public Activity Activity { get; }
		public Dictionary<string, FormMetadata> AllFormsMetadata { get; set; }
		public List<MyFormWrapper> AppFragments { get; set; }
		public int? ContentResourceId { get; set; }

		public OutputManagerCollection OutputManagerCollection { get; }
		private AppSharedPreference AppPreference { get; }
		private FormRegister FormRegister { get; }
		private InputManagerCollection InputManagerCollection { get; }
		private IMediator Mediator { get; }
		private UiMetadataWebApi UiMetadataWebApi { get; }

		public void DrawInputs(LinearLayout layout, FormParameters formParameters, List<FormInputManager> inputsManager)
		{
			var orderedInputs = formParameters.Form.InputFields.OrderBy(a => a.OrderIndex).ToList();
			inputsManager.Clear();

			foreach (var input in orderedInputs)
			{
				if (!input.Hidden)
				{
					var label = new TextView(Application.Context) { Text = input.Label.Humanize(LetterCasing.Sentence) };
					layout.AddView(label, layout.MatchParentWrapContent());
				}

				var manager = this.InputManagerCollection.GetManager(input.Type);

				var view = manager.GetView(input.CustomProperties);
				var value = formParameters.Parameters?.SingleOrDefault(a => a.Key.Equals(input.Id)).Value;
				if (value != null)
				{
					manager.SetValue(value);
				}
				inputsManager.Add(new FormInputManager(input, manager, view));

				if (input.Hidden)
				{
					view.Visibility = ViewStates.Gone;
				}
				layout.AddView(view, layout.MatchParentWrapContent());
			}
		}

		public async Task<FormMetadata> GetFormMetadataAsync(string form)
		{
			var response = await UiMetadataHttpRequestHelper.GetFormMetadata(form, this.UiMetadataWebApi.FormMetadataUrl,
				this.AppPreference.GetSharedKey("Cookies"));

			if (response == null)
			{
				Toast.MakeText(Application.Context, "Error fetching data. Server returned status code: {0}", ToastLength.Long).Show();
				return null;
			}
			return response;
		}

		public View GetIForm(FormMetadata formMetadata, IDictionary<string, object> inputFieldValues = null)
		{
			if (formMetadata == null)
			{
				Toast.MakeText(Application.Context, "You don't have access to this form", ToastLength.Long).Show();
				return null;
			}
			var formParameters = new FormParameters(formMetadata, inputFieldValues);
			var layout = this.DrawForm(formParameters);
			return layout;
		}

		public View GetIForm(string form, IDictionary<string, object> inputFieldValues = null)
		{
			FormMetadata formMetadata;
			if (this.AllFormsMetadata != null)
			{
				formMetadata = this.AllFormsMetadata[form];
			}
			else if (this.UiMetadataWebApi != null)
			{
				formMetadata = this.GetFormMetadataAsync(form).Result;
			}
			else
			{
				formMetadata = this.FormRegister.GetFormInfo(form)?.Metadata;
			}
			var layout = this.GetIForm(formMetadata, inputFieldValues);
			return layout;
		}

		public async Task<View> GetIFormAsync(string form, IDictionary<string, object> inputFieldValues = null)
		{
			FormMetadata formMetadata;
			if (this.UiMetadataWebApi != null)
			{
				formMetadata = await this.GetFormMetadataAsync(form);
			}
			else
			{
				formMetadata = this.FormRegister.GetFormInfo(form)?.Metadata;
			}
			var formParameters = new FormParameters(formMetadata, inputFieldValues);
			var layout = this.DrawForm(formParameters);
			return layout;
		}

		public async Task<InvokeForm.Response> HandleFormAsync(FormMetadata formMetadata, List<FormInputManager> inputsManager)
		{
			try
			{
				var obj = this.GetFormValues(inputsManager);
				var request = new InvokeForm.Request
				{
					Form = formMetadata.Id,
					InputFieldValues = obj
				};
				object resultData = null;
				if (this.UiMetadataWebApi != null)
				{
					var result = await this.InvokeFormAsync(new[] { request });
					if (result != null)
					{
						resultData = result[0].Data;
					}
				}
				else
				{
					InvokeForm.Response response = await this.Mediator.Send(request);
					resultData = response.Data;
				}

				return new InvokeForm.Response
				{
					Data = resultData
				};
			}
			catch (Exception ex)
			{
				Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
				return null;
			}
		}

		public void ReplaceFragment(FormMetadata formMetadata, IDictionary<string, object> inputFieldValues = null)
		{
			var fragment = new MyFormWrapper(formMetadata, this, this.Activity, inputFieldValues);
			this.AppFragments?.Add(fragment);
			if (this.ContentResourceId.HasValue)
			{
				fragment.UpdateFragment(this.ContentResourceId.Value);
			}
		}

		public async Task<View> StartIFormAsync(string form, IDictionary<string, object> inputFieldValues = null)
		{
			try
			{
				FormMetadata formMetadata;
				if (this.AllFormsMetadata != null)
				{
					formMetadata = this.AllFormsMetadata[form];
				}
				else if (this.UiMetadataWebApi != null)
				{
					formMetadata = await this.GetFormMetadataAsync(form);
				}
				else
				{
					formMetadata = this.FormRegister.GetFormInfo(form)?.Metadata;
				}

				var formParameters = new FormParameters(formMetadata, inputFieldValues);
				var layout = this.DrawForm(formParameters);
				return layout;
			}
			catch (Exception ex)
			{
				Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
				return null;
			}
		}

		private View DrawForm(FormParameters formParameters)
		{
			var scroll = new ScrollView(Application.Context);
			var linearLayout = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };
			linearLayout.SetPadding(20, 10, 20, 10);

			if (formParameters != null)
			{
				InvokeForm.Response result = null;
				List<FormInputManager> inputsManager = new List<FormInputManager>();
				var resultLayout = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };
				resultLayout.SetPadding(20, 10, 20, 10);

				if (formParameters.Form.InputFields.Count > 0)
				{
					this.DrawInputs(linearLayout, formParameters, inputsManager);

					if (formParameters.Form.InputFields.Count(a => !a.Hidden) > 0)
					{
						var btn = new Button(Application.Context) { Text = "Submit" };
						linearLayout.AddView(btn, linearLayout.MatchParentWrapContent());

						btn.Click += async (sender, args) =>
						{
							result = await this.SubmitFormAsync(resultLayout, formParameters.Form, inputsManager);
							this.DrawOutput(resultLayout, result, formParameters.Form, inputsManager);
						};
					}
				}
				if (formParameters.Form.PostOnLoad)
				{
					var task = Task.Run(() => this.SubmitFormAsync(resultLayout, formParameters.Form, inputsManager));
					result = task.Result;
					this.DrawOutput(resultLayout, result, formParameters.Form, inputsManager);
				}
				linearLayout.AddView(resultLayout, linearLayout.MatchParentWrapContent());
				scroll.AddView(linearLayout, scroll.MatchParentWrapContent());
			}
			return scroll;
		}

		private void DrawOutput(LinearLayout layout, InvokeForm.Response result, FormMetadata formMetadata, List<FormInputManager> inputsManager)
		{
			if (result != null)
			{
				var reloadResponse = result.Data.CastTObject<ReloadResponse>();
				if (reloadResponse?.Form != null)
				{
					if (this.AllFormsMetadata != null)
					{
						var metadata = this.AllFormsMetadata[reloadResponse.Form];
						this.ReplaceFragment(metadata, reloadResponse.InputFieldValues);
					}
					else
					{
						this.StartIFormAsync(reloadResponse.Form, reloadResponse.InputFieldValues);
					}
					return;
				}
				var orderedOutputs = formMetadata.OutputFields.OrderBy(a => a.OrderIndex).ToList();
				foreach (var output in orderedOutputs)
				{
					if (!output.Hidden)
					{
						object value;
						if (result.Data.GetType() == typeof(JObject))
						{
							var jsonObj = (JObject)result.Data;
							value = jsonObj.GetValue(output.Id, StringComparison.OrdinalIgnoreCase);
						}
						else
						{
							var propertyInfo = result.Data.GetType().GetProperty(output.Id);
							value = propertyInfo?.GetValue(result.Data, null);
						}
						if (value != null)
						{
							var manager = this.OutputManagerCollection.GetManager(output.Type);
							var view = manager.GetView(output, value, this, formMetadata, inputsManager);
							view.SetPadding(0, 10, 0, 10);
							layout.AddView(view, layout.MatchParentWrapContent());
						}
					}
				}
			}
		}

		private object GetFormValues(List<FormInputManager> inputsManager)
		{
			var list = new Dictionary<string, object>();
			foreach (var inputManager in inputsManager)
			{
				var value = inputManager.Manager.GetValue();
				if (value != null)
				{
					list.Add(inputManager.Input.Id, value);
				}
			}
			return JsonConvert.SerializeObject(list);
		}

		private async Task<List<InvokeForm.Response>> InvokeFormAsync(object param)
		{
			var response = await UiMetadataHttpRequestHelper.InvokeForm(this.UiMetadataWebApi.RunFormUrl, this.AppPreference.GetSharedKey("Cookies"),
				param);

			this.AppPreference.SetSharedKey("Cookies", response.Cookies);

			if (response.Response == null)
			{
				Toast.MakeText(Application.Context, "Error fetching data. Server returned status code: {0}", ToastLength.Long).Show();
				return null;
			}
			return response.Response;
		}

		private async Task<InvokeForm.Response> SubmitFormAsync(LinearLayout resultLayout, FormMetadata formMetadata, List<FormInputManager> inputsManager)
		{
			var valid = this.ValidateForm(inputsManager);
			if (valid)
			{
				resultLayout.RemoveAllViews();
				return await this.HandleFormAsync(formMetadata, inputsManager);
			}
			return null;
		}

		private bool ValidateForm(List<FormInputManager> inputsManager)
		{
			var valid = true;
			foreach (var inputManager in inputsManager)
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
	}
}