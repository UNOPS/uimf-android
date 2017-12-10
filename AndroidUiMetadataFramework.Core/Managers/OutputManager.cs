namespace AndroidUiMetadataFramework.Core.Managers
{
	using System.Collections.Generic;
	using Android.Views;
	using AndroidUiMetadataFramework.Core.Models;
	using UiMetadataFramework.Core;

	public interface IOutputManager
	{
		View GetView(OutputFieldMetadata outputField, 
			object value, 
			MyFormHandler myFormHandler, 
			FormMetadata formMetadata, 
			List<FormInputManager> inputsManager);
	}
}