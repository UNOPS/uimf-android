namespace AndroidUiMetadataFramework.Core.Models
{
	using Android.Views;
	using AndroidUiMetadataFramework.Core.Managers;
	using UiMetadataFramework.Core;

	public class FormInputManager
	{
		public FormInputManager(InputFieldMetadata input, IInputManager manager, View view)
		{
			this.Input = input;
			this.Manager = manager;
			this.View = view;
		}

		public InputFieldMetadata Input { get; set; }
		public IInputManager Manager { get; set; }
		public View View { get; set; }
	}
}