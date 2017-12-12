namespace AndroidApp.Forms
{
    using System.Collections.Generic;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Basic.Response;

    public class CustomFormWrapper : IFormWrapper
    {
        public CustomFormWrapper(FormsActivity activity, List<MyFormWrapper> appFragments, int contentResourceId)
        {
            this.Activity = activity;
            this.AppFragments = appFragments;
            this.ContentResourceId = contentResourceId;
        }

        public FormsActivity Activity { get; set; }
        public List<MyFormWrapper> AppFragments { get; set; }
        public int ContentResourceId { get; set; }

        public void UpdateView(MyFormHandler myFormHandler, FormParameter formParameter, string submitAction = null)
        {
            var fragment = new MyFormWrapper(formParameter, myFormHandler, this.Activity, submitAction);
            this.AppFragments?.Add(fragment);
            fragment.UpdateFragment(this.ContentResourceId);
        }

        public void CloseForm()
        {
            this.AppFragments.RemoveAt(this.AppFragments.Count - 1);
            if (this.AppFragments.Count != 0)
            {
                this.AppFragments[this.AppFragments.Count - 1].UpdateFragment(Resource.Id.content_frame);
            }
        }

        public void ReloadView(MyFormHandler myFormHandler, ReloadResponse reloadResponse)
        {
            var allForms = this.Activity.Reload();
            var metadata = allForms[reloadResponse.Form];
            this.UpdateView(myFormHandler, new FormParameter(metadata, reloadResponse.InputFieldValues));
        }
    }
}