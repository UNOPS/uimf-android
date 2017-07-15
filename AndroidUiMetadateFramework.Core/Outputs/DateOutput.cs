namespace AndroidUiMetadateFramework.Core.Outputs
{
	using System;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;

	[Output(Type = "datetime")]
	public class DateOutput : IOutputManager
	{
		private TextView OutputText { get; set; }

		public View GetView(Activity activity, string name, object value, FormActivity formActivity)
		{
			this.OutputText = new TextView(activity);
			if (value != null)
			{
				DateTime datetime = (DateTime)value;
				this.OutputText.Text = name + ": " + datetime.ToShortDateString();
			}

			return this.OutputText;
		}
	}
}