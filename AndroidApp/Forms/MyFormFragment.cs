namespace AndroidApp.Forms
{
    using System;
    using Android.App;
    using Android.OS;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Models;

    public class MyFormFragment : Fragment
    {
        public MyFormFragment(FormParameter formParameter,
            MyFormHandler myFormHandler,
            Activity ownerActivity,
            string submitAction = null)
        {
            this.MyFormHandler = myFormHandler;
            this.FormParameter = formParameter;
            this.OwnerActivity = ownerActivity;
            this.SubmitAction = submitAction;
        }

        public MyFormFragment(Activity ownerActivity)
        {
            this.OwnerActivity = ownerActivity;
        }

        public FormParameter FormParameter { get; set; }
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
                    var form = this.MyFormHandler.GetIForm(this.FormParameter, this.SubmitAction);
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
            this.OwnerActivity.Title = this.FormParameter?.Form == null ? "GMS" : this.FormParameter.Form?.Label;
        }
    }
}