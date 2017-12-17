namespace AndroidUiMetadataFramework.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Support.V4.Widget;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Managers;
    using Humanizer;
    using MediatR;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Basic.Response;
    using UiMetadataFramework.Core;
    using UiMetadataFramework.MediatR;

    public class MyFormHandler
    {
        public MyFormHandler(IMediator mediator,
            FormRegister formRegister,
            ManagersCollection managersCollection)
        {
            this.Mediator = mediator;
            this.ManagersCollection = managersCollection ?? new ManagersCollection();
            this.FormRegister = formRegister;
        }

        public MyFormHandler(UiMetadataWebApi uiMetadataWebApi,
            ManagersCollection managersCollection,
            IFormWrapper formWrapper,
            Dictionary<string, FormMetadata> allForms = null)
        {
            this.ManagersCollection = managersCollection ?? new ManagersCollection();
            this.AllFormsMetadata = allForms;
            this.UiMetadataWebApi = uiMetadataWebApi;
            this.AppPreference = new AppSharedPreference(Application.Context);
            this.FormWrapper = formWrapper;
        }

        public Dictionary<string, FormMetadata> AllFormsMetadata { get; set; }
        public AppSharedPreference AppPreference { get; }
        public IFormWrapper FormWrapper { get; set; }
        public ManagersCollection ManagersCollection { get; set; }
        public UiMetadataWebApi UiMetadataWebApi { get; }
        private FormRegister FormRegister { get; }
        private IMediator Mediator { get; }

        public FormMetadata GetFormMetadata(string form)
        {
            FormMetadata formMetadata = null;
            if (this.AllFormsMetadata != null && this.AllFormsMetadata.ContainsKey(form))
            {
                formMetadata = this.AllFormsMetadata[form];
            }
            else if (this.UiMetadataWebApi != null)
            {
                try
                {
                    var result = Task.Run(
                        () => this.GetFormMetadataAsync(form));
                    formMetadata = result.Result;
                }
                catch (AggregateException ex)
                {
                    ex.ThrowInnerException();
                }
            }
            else
            {
                formMetadata = this.FormRegister.GetFormInfo(form)?.Metadata;
            }

            return formMetadata;
        }

        public async Task<FormMetadata> GetFormMetadataAsync(string form)
        {
            var response = await UiMetadataHttpRequestHelper.GetFormMetadata(form, this.UiMetadataWebApi.FormMetadataUrl,
                this.AppPreference.GetSharedKey("Cookies"));
            return response;
        }

        public View GetIForm(FormParameter formParameter, string submitAction = null)
        {
            if (formParameter?.Form == null)
            {
                Toast.MakeText(Application.Context, "You don't have access to this form", ToastLength.Long).Show();
                return null;
            }
            var layout = this.RenderForm(formParameter, submitAction);
            return layout;
        }

        public View GetIForm(string form, IDictionary<string, object> inputFieldValues = null, string submitAction = null)
        {
            var formMetadata = this.GetFormMetadata(form);
            var layout = this.GetIForm(new FormParameter(formMetadata, inputFieldValues), submitAction);
            return layout;
        }

        public async Task<InvokeForm.Response> HandleFormAsync(FormMetadata formMetadata, List<FormInputManager> inputsManager)
        {
            var obj = this.GetFormValues(inputsManager);
            var request = new InvokeForm.Request
            {
                Form = formMetadata.Id,
                InputFieldValues = obj
            };
            // run on form posting events
            EventsManager.OnFormPostingEvent(formMetadata, inputsManager);

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
                var response = await this.Mediator.Send(request);
                resultData = response.Data;
            }

            // run on response received events
            EventsManager.OnResponseReceivedEvent(formMetadata, inputsManager, resultData);

            return new InvokeForm.Response
            {
                Data = resultData
            };
        }

        public async Task<List<InvokeForm.Response>> InvokeFormAsync(object param)
        {
            var response = await UiMetadataHttpRequestHelper.InvokeForm(this.UiMetadataWebApi.RunFormUrl, this.AppPreference.GetSharedKey("Cookies"),
                param);

            this.AppPreference.SetSharedKey("Cookies", response.Cookies);
            return response.Response;
        }

        public void RenderInputs(LinearLayout inputsLayout, FormParameter formParameter, List<FormInputManager> inputsManager)
        {
            var orderedInputs = formParameter.Form.InputFields.OrderBy(a => a.OrderIndex).ToList();
            inputsManager.Clear();

            foreach (var input in orderedInputs)
            {
                if (!input.Hidden)
                {
                    var label = new TextView(Application.Context)
                    {
                        Text = input.Label.Humanize(LetterCasing.Sentence),
                        LayoutParameters = inputsLayout.MatchParentWrapContent()
                    };

                    this.ManagersCollection.StyleRegister.ApplyStyle("TextView", label);

                    inputsLayout.AddView(label);
                }

                var manager = this.ManagersCollection.InputManagerCollection.GetManager(input.Type);

                var view = manager.GetView(input.CustomProperties, this);
                if (view.LayoutParameters == null)
                {
                    view.LayoutParameters = inputsLayout.MatchParentWrapContent();
                }
                var value = formParameter.Parameters?.SingleOrDefault(a => a.Key.ToLower().Equals(input.Id.ToLower())).Value;
                if (value != null)
                {
                    manager.SetValue(value);
                }
                inputsManager.Add(new FormInputManager(input, manager, view));

                if (input.Hidden)
                {
                    view.Visibility = ViewStates.Gone;
                }
                inputsLayout.AddView(view);
            }
        }

        public async Task<View> StartIFormAsync(string form, IDictionary<string, object> inputFieldValues = null, string submitAction = null)
        {
            var formMetadata = this.GetFormMetadata(form);
            var formParameters = new FormParameter(formMetadata, inputFieldValues);
            var layout = this.RenderForm(formParameters, submitAction);
            return layout;
        }

        private string GetFormValues(IEnumerable<FormInputManager> inputsManager)
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

        private View RenderForm(FormParameter formParameter, string submitAction)
        {
            var scroll = new NestedScrollView(Application.Context);
            var linearLayout = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };

            if (formParameter != null)
            {
                InvokeForm.Response result = null;
                var inputsManager = new List<FormInputManager>();
                var resultLayout = new LinearLayout(Application.Context)
                {
                    Orientation = Orientation.Vertical,
                    LayoutParameters = linearLayout.MatchParentWrapContent()
                };

                if (formParameter.Form.InputFields.Count > 0)
                {
                    var inputsLayout = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };

                    this.RenderInputs(inputsLayout, formParameter, inputsManager);

                    if (formParameter.Form.InputFields.Count(a => !a.Hidden) > 0)
                    {
                        this.ManagersCollection.StyleRegister.ApplyStyle("FormLayout", inputsLayout);
                        var submitLabel = "Submit";
                        var submitbuttonlabel = formParameter.Form.CustomProperties?.GetCustomProperty<string>("submitButtonLabel");

                        if (!string.IsNullOrEmpty(submitbuttonlabel))
                        {
                            submitLabel = submitbuttonlabel;
                        }
                        var btn = new Button(Application.Context)
                        {
                            Text = submitLabel,
                            LayoutParameters = inputsLayout.MatchParentWrapContent()
                        };
                        this.ManagersCollection.StyleRegister.ApplyStyle("Button SubmitButton", btn);
                        inputsLayout.AddView(btn);

                        btn.Click += async (sender, args) =>
                        {
                            try
                            {
                                result = await this.SubmitFormAsync(resultLayout, formParameter.Form, inputsManager);
                                if (submitAction == FormLinkActions.OpenModal && result != null)
                                {
                                    this.FormWrapper.CloseForm();
                                }
                                else
                                {
                                    this.RenderOutput(resultLayout, result, formParameter.Form, inputsManager);
                                }
                            }
                            catch (Exception ex)
                            {
                                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                            }
                        };
                    }
                    linearLayout.AddView(inputsLayout);
                }
                // run on response handled events
                EventsManager.OnFormLoadedEvent(formParameter);

                if (formParameter.Form.PostOnLoad || submitAction == FormLinkActions.Run)
                {
                    try
                    {
                        var taskToRun = Task.Run(() => this.SubmitFormAsync(resultLayout, formParameter.Form, inputsManager,
                            formParameter.Form.PostOnLoadValidation));
                        result = taskToRun.Result;
                    }
                    catch (AggregateException ex)
                    {
                        ex.ThrowInnerException();
                    }

                    if (submitAction == FormLinkActions.Run)
                    {
                        this.FormWrapper.CloseForm();
                    }
                    else
                    {
                        this.RenderOutput(resultLayout, result, formParameter.Form, inputsManager);
                    }
                }
                this.ManagersCollection.StyleRegister.ApplyStyle("ResultsLayout", resultLayout);

                linearLayout.AddView(resultLayout);
                scroll.AddView(linearLayout, scroll.MatchParentWrapContent());
            }
            return scroll;
        }

        private void RenderOutput(ViewGroup layout, InvokeForm.Response result, FormMetadata formMetadata, List<FormInputManager> inputsManager)
        {
            if (result?.Data == null)
            {
                return;
            }
            var reloadResponse = result.Data.CastTObject<ReloadResponse>();
            if (reloadResponse?.Form != null)
            {
                this.FormWrapper.ReloadView(this, reloadResponse);

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
                        var manager = this.ManagersCollection.OutputManagerCollection.GetManager(output.Type);
                        var view = manager.GetView(output, value, this, formMetadata, inputsManager);
                        view.SetPadding(0, 10, 0, 10);
                        if (view.LayoutParameters == null)
                        {
                            view.LayoutParameters = layout.MatchParentWrapContent();
                        }
                        layout.AddView(view);
                    }
                }
            }
            // run on response handled events
            EventsManager.OnResponseHandledEvent(this, formMetadata, inputsManager, result);
        }

        private async Task<InvokeForm.Response> SubmitFormAsync(ViewGroup resultLayout,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager,
            bool validate = true)
        {
            var valid = !validate || this.ValidateForm(inputsManager);
            InvokeForm.Response result = null;
            if (valid)
            {
                resultLayout.RemoveAllViews();
                result = await this.HandleFormAsync(formMetadata, inputsManager);
            }
            return result;
        }

        private bool ValidateForm(IEnumerable<FormInputManager> inputsManager)
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
                        this.ManagersCollection.StyleRegister.ApplyStyle("ValidationError", inputManager.View);
                    }
                }
            }

            return valid;
        }
    }
}