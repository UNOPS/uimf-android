﻿namespace AndroidUiMetadataFramework.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Graphics;
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
        public MyFormHandler(Activity activity,
            IMediator mediator,
            FormRegister formRegister,
            InputManagerCollection inputManager,
            OutputManagerCollection outputManager,
            EventHandlerManagerCollection eventHandlerManager)
        {
            this.EventHandlerManager = eventHandlerManager;
            this.Activity = activity;
            this.Mediator = mediator;
            this.InputManagerCollection = inputManager;
            this.OutputManagerCollection = outputManager;
            this.FormRegister = formRegister;
        }

        public MyFormHandler(Activity activity,
            UiMetadataWebApi uiMetadataWebApi,
            InputManagerCollection inputManager,
            OutputManagerCollection outputManager,
            EventHandlerManagerCollection eventHandlerManager,
            IFormWrapper formWrapper,
            Dictionary<string, FormMetadata> allForms = null)
        {
            this.EventHandlerManager = eventHandlerManager;
            this.Activity = activity;
            this.InputManagerCollection = inputManager;
            this.OutputManagerCollection = outputManager;
            this.AllFormsMetadata = allForms;
            this.UiMetadataWebApi = uiMetadataWebApi;
            this.AppPreference = new AppSharedPreference(Application.Context);
            this.FormWrapper = formWrapper;
        }

        public Activity Activity { get; }
        public Dictionary<string, FormMetadata> AllFormsMetadata { get; set; }
        public AppSharedPreference AppPreference { get; }
        public EventHandlerManagerCollection EventHandlerManager { get; }
        public IFormWrapper FormWrapper { get; set; }
        public InputManagerCollection InputManagerCollection { get; }
        public OutputManagerCollection OutputManagerCollection { get; }
        public UiMetadataWebApi UiMetadataWebApi { get; }
        private FormRegister FormRegister { get; }
        private IMediator Mediator { get; }

        public void RenderInputs(LinearLayout layout, FormParameters formParameters, List<FormInputManager> inputsManager)
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

                var view = manager.GetView(input.CustomProperties, this);
                var value = formParameters.Parameters?.SingleOrDefault(a => a.Key.ToLower().Equals(input.Id.ToLower())).Value;
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

        public FormMetadata GetFormMetadata(string form)
        {
            FormMetadata formMetadata;
            if (this.AllFormsMetadata != null && this.AllFormsMetadata.ContainsKey(form))
            {
                formMetadata = this.AllFormsMetadata[form];
            }
            else if (this.UiMetadataWebApi != null)
            {
                var result = Task.Run(
                    () => this.GetFormMetadataAsync(form));
                formMetadata = result.Result;
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

            if (response == null)
            {
                Toast.MakeText(Application.Context, "Error fetching data. Server returned status code: {0}", ToastLength.Long).Show();
                return null;
            }
            return response;
        }

        public View GetIForm(FormMetadata formMetadata, IDictionary<string, object> inputFieldValues = null, string submitAction = null)
        {
            if (formMetadata == null)
            {
                Toast.MakeText(Application.Context, "You don't have access to this form", ToastLength.Long).Show();
                return null;
            }
            var formParameters = new FormParameters(formMetadata, inputFieldValues);
            var layout = this.RenderForm(formParameters, submitAction);
            return layout;
        }

        public View GetIForm(string form, IDictionary<string, object> inputFieldValues = null, string submitAction = null)
        {
            var formMetadata = this.GetFormMetadata(form);
            var layout = this.GetIForm(formMetadata, inputFieldValues, submitAction);
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
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                return null;
            }
        }

        public async Task<List<InvokeForm.Response>> InvokeFormAsync(object param, bool setCookies = true)
        {
            var response = await UiMetadataHttpRequestHelper.InvokeForm(this.UiMetadataWebApi.RunFormUrl, this.AppPreference.GetSharedKey("Cookies"),
                param);

            if (setCookies)
            {
                this.AppPreference.SetSharedKey("Cookies", response.Cookies);
            }

            if (response.Response == null)
            {
                Toast.MakeText(Application.Context, "Error fetching data. Server returned status code: {0}", ToastLength.Long).Show();
                return null;
            }
            return response.Response;
        }

        public async Task<View> StartIFormAsync(string form, IDictionary<string, object> inputFieldValues = null, string submitAction = null)
        {
            try
            {
                var formMetadata = this.GetFormMetadata(form);
                var formParameters = new FormParameters(formMetadata, inputFieldValues);
                var layout = this.RenderForm(formParameters, submitAction);
                return layout;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                return null;
            }
        }

        private View RenderForm(FormParameters formParameters, string submitAction)
        {
            var scroll = new ScrollView(Application.Context);
            var linearLayout = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };
            linearLayout.SetPadding(20, 10, 20, 10);

            if (formParameters != null)
            {
                InvokeForm.Response result = null;
                var inputsManager = new List<FormInputManager>();
                var resultLayout = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };
                resultLayout.SetPadding(20, 10, 20, 10);

                if (formParameters.Form.InputFields.Count > 0)
                {
                    this.RenderInputs(linearLayout, formParameters, inputsManager);

                    if (formParameters.Form.InputFields.Count(a => !a.Hidden) > 0)
                    {
                        var submitLabel = "Submit";
                        var submitbuttonlabel = formParameters.Form.CustomProperties?.GetCustomProperty<string>("submitButtonLabel");

                        if (!string.IsNullOrEmpty(submitbuttonlabel))
                        {
                            submitLabel = submitbuttonlabel;
                        }
                        var btn = new Button(Application.Context) { Text = submitLabel };
                        linearLayout.AddView(btn, linearLayout.MatchParentWrapContent());
                        btn.SetBackgroundColor(new Color(22, 156, 133));

                        btn.Click += async (sender, args) =>
                        {
                            result = await this.SubmitFormAsync(resultLayout, formParameters.Form, inputsManager);
                            if (submitAction == FormLinkActions.OpenModal)
                            {
                                this.FormWrapper.CloseForm();
                            }
                            else
                            {
                                this.RenderOutput(resultLayout, result, formParameters.Form, inputsManager);
                            }                           
                        };
                    }
                }
                // run on response handled events
                EventsManager.OnFormLoadedEvent(formParameters);

                if (formParameters.Form.PostOnLoad)
                {
                    var task = Task.Run(() => this.SubmitFormAsync(resultLayout, formParameters.Form, inputsManager,
                        formParameters.Form.PostOnLoadValidation));
                    result = task.Result;
                    if (submitAction == FormLinkActions.Run)
                    {
                        this.FormWrapper.CloseForm();
                    }
                    else
                    {
                        this.RenderOutput(resultLayout, result, formParameters.Form, inputsManager);
                    }                   
                }
                linearLayout.AddView(resultLayout, linearLayout.MatchParentWrapContent());
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
                        var manager = this.OutputManagerCollection.GetManager(output.Type);
                        var view = manager.GetView(output, value, this, formMetadata, inputsManager);
                        view.SetPadding(0, 10, 0, 10);
                        layout.AddView(view, layout.MatchParentWrapContent());
                    }
                }
            }
            // run on response handled events
            EventsManager.OnResponseHandledEvent(this, formMetadata, inputsManager, result);
        }

        private object GetFormValues(IEnumerable<FormInputManager> inputsManager)
        {
            var list = new Dictionary<string, object>();
            foreach (var inputManager in inputsManager)
            {
                var value =  inputManager.Manager.GetValue();
                if (value != null)
                {
                    list.Add(inputManager.Input.Id, value);
                }
            }
            return JsonConvert.SerializeObject(list);
        }

        private async Task<InvokeForm.Response> SubmitFormAsync(ViewGroup resultLayout,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager,
            bool validate = true)
        {
            var valid = !validate || this.ValidateForm(inputsManager);
            if (valid)
            {
                resultLayout.RemoveAllViews();
                return await this.HandleFormAsync(formMetadata, inputsManager);
            }
            return null;
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
                        inputManager.View.SetBackgroundResource(Resource.Drawable.ValidationBorders);
                    }
                }
            }

            return valid;
        }
    }
}