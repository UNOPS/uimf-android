namespace AndroidUiMetadateFramework.Core.Outputs
{
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;

	[Output(Type = "text")]
	public class TextOutput : IOutputManager
	{
		private TextView OutputText { get; set; }

		public View GetView(string name, object value, FormActivity formActivity)
		{
			this.OutputText = new TextView(Application.Context) { Text = name + ": " + value };
			return this.OutputText;
		}
	}
}