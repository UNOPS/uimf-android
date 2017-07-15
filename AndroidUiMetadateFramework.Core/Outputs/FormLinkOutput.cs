namespace AndroidUiMetadateFramework.Core.Outputs
{
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using UiMetadataFramework.Basic.Output;

	[Output(Type = "formlink")]
	public class FormLinkOutput : IOutputManager
	{
		private Button Button { get; set; }

		public View GetView(Activity activity, string name, object value, FormActivity formActivity)
		{
			var formLink = (FormLink)value;
			this.Button = new Button(activity) { Text = formLink.Label };

			this.Button.Click += async (sender, args) => { await formActivity.StartIForm(formLink.Form, formLink.InputFieldValues); };

			return this.Button;
		}
	}
}