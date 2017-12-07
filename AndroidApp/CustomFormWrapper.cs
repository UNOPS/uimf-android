namespace AndroidApp
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Android.App;
    using AndroidUiMetadateFramework.Core.Models;
    using UiMetadataFramework.Basic.Response;
    using UiMetadataFramework.Core;

    public class CustomFormWrapper : IFormWrapper
    {
        public FormsActivity Activity { get; set; }
        public List<MyFormWrapper> AppFragments { get; set; }
        public int ContentResourceId { get; set; }

        public CustomFormWrapper(FormsActivity activity, List<MyFormWrapper> appFragments, int contentResourceId)
        {
            this.Activity = activity;
            this.AppFragments = appFragments;
            this.ContentResourceId = contentResourceId;
        }
        public void UpdateView(MyFormHandler myFormHandler, FormMetadata metadata, IDictionary<string, object> inputFieldValues = null)
        {
            var fragment = new MyFormWrapper(metadata, myFormHandler, this.Activity, inputFieldValues);
            this.AppFragments?.Add(fragment);
            fragment.UpdateFragment(this.ContentResourceId);

        }

        public Task UpdateViewAsync(MyFormHandler myFormHandler, FormMetadata metadata, IDictionary<string, object> inputFieldValues = null)
        {
            throw new System.NotImplementedException();
        }

        public void ReloadView(MyFormHandler myFormHandler, ReloadResponse reloadResponse)
        {
            var allForms = this.Activity.Reload();
            var metadata = allForms[reloadResponse.Form];
            this.UpdateView(myFormHandler, metadata ,reloadResponse.InputFieldValues);
        }
    }
}