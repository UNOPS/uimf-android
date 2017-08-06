namespace AndroidUiMetadateFramework.Core.Outputs
{
	using System;
	using Android.App;
	using Android.Graphics;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using UiMetadataFramework.Basic.Output;

	[Output(Type = "formlink")]
	public class FormLinkOutput : IOutputManager
	{
		private Button Button { get; set; }

		public View GetView(string name, object value, FormActivity formActivity)
		{
			var formLink = (FormLink)value;
			this.Button = new Button(Application.Context) { Text = formLink.Label };

			this.Button.Click += async (sender, args) =>
			{
				try
				{
					var linearLayout = new LinearLayout(Application.Context)
					{
						Orientation = Orientation.Vertical
					};
					var layout = await formActivity.GetIForm(formLink.Form, formLink.InputFieldValues);
					linearLayout.SetBackgroundColor(Color.Black);
					var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
						ViewGroup.LayoutParams.WrapContent);
					var closeBtn = new Button(Application.Context){Text = "Close"};
					closeBtn.SetBackgroundColor(Color.Black);
					linearLayout.AddView(layout, param);
					param.Weight = 1;
					linearLayout.AddView(closeBtn, param);
					var metrics = Application.Context.Resources.DisplayMetrics;
					PopupWindow popup = new PopupWindow(linearLayout, metrics.WidthPixels - 40, 3 * metrics.HeightPixels /4 );
					popup.ShowAtLocation(this.Button, GravityFlags.Center, 5, 0);
					closeBtn.Click += (o, eventArgs) =>
					{
						popup.Dismiss();
					};
				}
				catch (Exception ex)
				{
					
				}
				
			};

			return this.Button;
		}
	}
}