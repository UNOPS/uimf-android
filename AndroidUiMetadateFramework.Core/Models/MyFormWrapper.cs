namespace AndroidUiMetadateFramework.Core.Models
{
	using System.Collections.Generic;
	using Android.App;
	using Android.OS;
	using Android.Views;
	using Android.Widget;
	using UiMetadataFramework.Core;

	public class MyFormWrapper : Fragment
	{
		public MyFormHandler MyFormHandler { get; set; }
		public FormMetadata FormMetadata { get; set; }
		public Activity OwnerActivity { get; set; }
		public IDictionary<string, object> InputFieldValues { get; set; }

		public MyFormWrapper(FormMetadata form, MyFormHandler myFormHandler, Activity ownerActivity, IDictionary<string, object> inputFieldValues = null)
		{
			this.MyFormHandler = myFormHandler;
			this.FormMetadata = form;
			this.OwnerActivity = ownerActivity;
			this.InputFieldValues = inputFieldValues;
		}
		public MyFormWrapper(Activity ownerActivity)
		{
			this.OwnerActivity = ownerActivity;
		}
		public override View OnCreateView(LayoutInflater inflater,
			ViewGroup container,
			Bundle savedInstanceState)
		{
			View rootView = new LinearLayout(Application.Context);

			if (this.FormMetadata != null)
			{
				rootView = this.MyFormHandler.GetIForm(this.FormMetadata, this.InputFieldValues);
			}
			return rootView;
		}

		public void UpdateFragment(int resId)
		{
			var fragmentManager = this.OwnerActivity.FragmentManager;
			var ft = fragmentManager.BeginTransaction();
			ft.Replace(resId, this);
			ft.Commit();
			this.OwnerActivity.Title = this.FormMetadata.Label;
		}
	}
}