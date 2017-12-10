namespace AndroidApp.Forms
{
    using System.Collections.Generic;
    using Android.App;
    using Android.OS;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Core;

    public class MyFormWrapper : Fragment
    {
        public MyFormWrapper(FormMetadata form, MyFormHandler myFormHandler, Activity ownerActivity, 
            IDictionary<string, object> inputFieldValues = null,
            string submitAction = null)
        {
            this.MyFormHandler = myFormHandler;
            this.FormMetadata = form;
            this.OwnerActivity = ownerActivity;
            this.InputFieldValues = inputFieldValues;
            this.SubmitAction = submitAction;
        }

        public string SubmitAction { get; set; }

        public MyFormWrapper(Activity ownerActivity)
        {
            this.OwnerActivity = ownerActivity;
        }

        public FormMetadata FormMetadata { get; set; }
        public IDictionary<string, object> InputFieldValues { get; set; }
        public MyFormHandler MyFormHandler { get; set; }
        public Activity OwnerActivity { get; set; }

        public override View OnCreateView(LayoutInflater inflater,
            ViewGroup container,
            Bundle savedInstanceState)
        {
            View rootView = new LinearLayout(Application.Context);

            if (this.FormMetadata != null)
            {
                rootView = this.MyFormHandler.GetIForm(this.FormMetadata, this.InputFieldValues, this.SubmitAction);
            }
            return rootView;
        }

        public void UpdateFragment(int resId)
        {
            var fragmentManager = this.OwnerActivity.FragmentManager;
            var ft = fragmentManager.BeginTransaction();
            ft.Replace(resId, this);
            ft.Commit();
            this.OwnerActivity.Title = this.FormMetadata?.Label;
        }
    }
}