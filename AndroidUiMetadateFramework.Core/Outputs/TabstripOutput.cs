namespace AndroidUiMetadateFramework.Core.Outputs
{
	using System.Collections.Generic;
	using System.Linq;
	using Android.App;
	using Android.Content;
	using Android.Content.Res;
	using Android.Runtime;
	using Android.Support.V4.View;
	using Android.Util;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using UiMetadataFramework.Basic.Output;

	[Output(Type = "tabstrip")]
	public class TabstripOutput : IOutputManager
	{
		private RelativeLayout LinearLayout { get; set; }

		public View GetView(string name, object value, FormActivity formActivity)
		{
			LayoutInflater inflater = (LayoutInflater)Application.Context
				.GetSystemService(Context.LayoutInflaterService);

			var heightInDp = Application.Context.ScreenHeightDp();
			this.LinearLayout = new RelativeLayout(Application.Context);
			View rowView = inflater.Inflate(Resource.Layout.TabStrip, this.LinearLayout, false);
			var tabstrip = (Tabstrip)value;
			var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, heightInDp);
			ViewPager viewPager = rowView.FindViewById<ViewPager>(Resource.Id.pager);
			viewPager.LayoutParameters = param;
			viewPager.Adapter = new CustomPagerAdapter(tabstrip.Tabs.ToList(), formActivity);
			this.LinearLayout.AddView(viewPager);
			return this.LinearLayout;
		}

	

	}

	public class CustomPagerAdapter : PagerAdapter
	{
		private readonly FormActivity formActivity;

		public CustomPagerAdapter(List<Tab> tabs, FormActivity formActivity)
		{
			this.formActivity = formActivity;
			this.Tabs = tabs;
		}

		public override int Count => this.Tabs.Count;
		private List<Tab> Tabs { get; }

		public override void DestroyItem(View container, int position, Java.Lang.Object view)
		{
			var viewPager = container.JavaCast<ViewPager>();
			viewPager.RemoveView(view as View);
		}

		public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
		{
			return new Java.Lang.String(this.Tabs[position].Label);
		}

		public override Java.Lang.Object InstantiateItem(View container, int position)
		{
			var form = this.formActivity.GetIForm(this.Tabs[position].Form, this.Tabs[position].InputFieldValues).Result;
			var viewPager = container.JavaCast<ViewPager>();
			viewPager.AddView(form);
			return form;
		}

		public override bool IsViewFromObject(View view, Java.Lang.Object obj)
		{
			return view == obj;
		}
	}
}