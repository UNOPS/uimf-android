namespace AndroidApp
{
	using Android.Support.V4.Widget;
	using Android.Support.V7.App;
	using Android.Views;

	public class MyActionBarDrawerToggle : ActionBarDrawerToggle
	{
		public MyActionBarDrawerToggle(AppCompatActivity activity, DrawerLayout layout, int openRes, int closeRes, string title)
			: base(activity, layout, openRes, closeRes)
		{
			this.Owner = activity;
			this.Title = title;
		}

		private AppCompatActivity Owner { get; }
		private string Title { get; }

		public override void OnDrawerClosed(View drawerView)
		{
			this.Owner.SupportActionBar.Title = this.Owner.Title;
			this.Owner.InvalidateOptionsMenu();
		}

		public override void OnDrawerOpened(View drawerView)
		{
			this.Owner.SupportActionBar.Title = this.Title;
			this.Owner.InvalidateOptionsMenu();
		}
	}
}