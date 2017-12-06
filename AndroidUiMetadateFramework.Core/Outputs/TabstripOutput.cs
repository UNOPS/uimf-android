namespace AndroidUiMetadateFramework.Core.Outputs
{
	using System.Collections.Generic;
	using System.Linq;
	using Android.App;
	using Android.Content;
	using Android.Graphics;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;

	[Output(Type = "tabstrip")]
	public class TabstripOutput : IOutputManager
	{
		private RelativeLayout LinearLayout { get; set; }

		public View GetView(OutputFieldMetadata outputField, object value, MyFormHandler myFormHandler, FormMetadata formMetadata, List<FormInputManager> inputsManager)
		{
			LayoutInflater inflater = (LayoutInflater)Application.Context
				.GetSystemService(Context.LayoutInflaterService);
			this.LinearLayout = new RelativeLayout(Application.Context);
			View rowView = inflater.Inflate(Resource.Layout.TabText, this.LinearLayout, false);
			var tabstrip = value.CastTObject<Tabstrip>();
			var currentTab = tabstrip.Tabs.SingleOrDefault(a => a.Form == tabstrip.CurrentTab);
			var linearL = new LinearLayout(Application.Context){Orientation = Orientation.Horizontal};
			foreach (var tab in tabstrip.Tabs)
			{
				if(tab != null)
				{
					var tv = new TextView(Application.Context);
					if (tab == currentTab)
					{
						tv = rowView.FindViewById<TextView>(Resource.Id.tv);
						tv.SetTextColor(Color.White);
					}
					tv.Text = tab.Label;
					tv.SetPadding(10, 5, 10, 5);
					tv.Click += async (sender, args) =>
					{
						var metadata = myFormHandler.GetFormMetadata(tab.Form);

						 myFormHandler.FormWrapper.UpdateView(myFormHandler, metadata, tab.InputFieldValues);
					};
					linearL.AddView(tv);
				}
				
			}
			this.LinearLayout.AddView(linearL);
			return this.LinearLayout;
		}
	}
}