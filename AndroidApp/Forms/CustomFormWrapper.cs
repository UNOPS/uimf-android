namespace AndroidApp.Forms
{
    using System.Collections.Generic;
    using Android.OS;
    using Android.Views;
    using AndroidUiMetadataFramework.Core.Models;
    using Java.Lang;
    using UiMetadataFramework.Basic.Response;

    public class CustomFormWrapper : IFormWrapper
    {
        public CustomFormWrapper(FormsActivity activity, List<MyFormFragment> appFragments)
        {
            this.Activity = activity;
            this.AppFragments = appFragments;
        }

        public FormsActivity Activity { get; set; }
        public List<MyFormFragment> AppFragments { get; set; }

        public void UpdateView(MyFormHandler myFormHandler, FormParameter formParameter, string submitAction = null)
        {
            new UpdateViewTask(this, myFormHandler, formParameter, submitAction).Execute();
           
        }

        public void UpdateViewAsync(MyFormHandler myFormHandler, FormParameter formParameter, string submitAction = null)
        {
            var fragment = new MyFormFragment(formParameter, myFormHandler, this.Activity, submitAction);
            this.AppFragments?.Add(fragment);
            fragment.UpdateFragment();
        }

        public void CloseForm()
        {
            this.AppFragments.RemoveAt(this.AppFragments.Count - 1);
            if (this.AppFragments.Count != 0)
            {
                var wrapper = this.AppFragments[this.AppFragments.Count - 1];
                this.UpdateView(wrapper.MyFormHandler, wrapper.FormParameter, wrapper.SubmitAction);
                this.AppFragments.RemoveAt(this.AppFragments.Count - 1);
            }
        }

        public void ReloadView(MyFormHandler myFormHandler, ReloadResponse reloadResponse)
        {
            var allForms = this.Activity.Reload();
            var metadata = allForms[reloadResponse.Form];
            this.UpdateView(myFormHandler, new FormParameter(metadata, reloadResponse.InputFieldValues));
        }

        public class UpdateViewTask : AsyncTask
        {
            public UpdateViewTask(CustomFormWrapper wrapper, MyFormHandler myFormHandler, FormParameter formParameter, string submitAction)
            {
                this.Wrapper = wrapper;
                this.MyFormHandler = myFormHandler;
                this.FormParameter = formParameter;
                this.SubmitAction = submitAction;
            }

            public CustomFormWrapper Wrapper { get; set; }
            private MyFormHandler MyFormHandler { get; }
            private FormParameter FormParameter { get; }
            private string SubmitAction { get; }

            protected override Object DoInBackground(params Object[] @params)
            {
                this.Wrapper.Activity.RunOnUiThread(() =>
                {
                    this.Wrapper.UpdateViewAsync(this.MyFormHandler, this.FormParameter, this.SubmitAction);
                });
                return null;
            }

            protected override void OnPostExecute(Object result)
            {
                this.Wrapper.Activity.ProgressBar.Visibility = ViewStates.Invisible;
            }

            protected override void OnPreExecute()
            {
                this.Wrapper.Activity.ProgressBar.Visibility = ViewStates.Visible;
            }
        }
    }

  
}