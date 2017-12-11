namespace AndroidApp.Forms
{
    using System;
    using System.Collections.Generic;
    using Android.App;
    using Android.OS;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Core;

    public class MyFormWrapper : Fragment
    {
        public MyFormWrapper(FormMetadata form,
            MyFormHandler myFormHandler,
            Activity ownerActivity,
            IDictionary<string, object> inputFieldValues = null,
            string submitAction = null)
        {
            this.MyFormHandler = myFormHandler;
            this.FormMetadata = form;
            this.OwnerActivity = ownerActivity;
            this.InputFieldValues = inputFieldValues;
            this.SubmitAction = submitAction;
        }

        public MyFormWrapper(Activity ownerActivity)
        {
            this.OwnerActivity = ownerActivity;
        }

        public FormMetadata FormMetadata { get; set; }
        public IDictionary<string, object> InputFieldValues { get; set; }
        public MyFormHandler MyFormHandler { get; set; }
        public Activity OwnerActivity { get; set; }
        public View RootView { get; set; }

        public string SubmitAction { get; set; }

        public override View OnCreateView(LayoutInflater inflater,
            ViewGroup container,
            Bundle savedInstanceState)
        {
            return this.RootView ?? (this.RootView = new LinearLayout(Application.Context));
        }

        public void UpdateFragment(int resId)
        {
            if (this.MyFormHandler != null)
            {
                try
                {
                    var form = this.MyFormHandler.GetIForm(this.FormMetadata, this.InputFieldValues, this.SubmitAction);
                    if (form != null)
                    {
                        this.RootView = form;
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                    return;
                }
            }            
           
            var fragmentManager = this.OwnerActivity.FragmentManager;
            var ft = fragmentManager.BeginTransaction();
            ft.Replace(resId, this);
            ft.Commit();
            this.OwnerActivity.Title = this.FormMetadata?.Label;
        }
    }
}