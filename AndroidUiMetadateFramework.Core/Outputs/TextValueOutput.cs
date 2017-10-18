﻿namespace AndroidUiMetadateFramework.Core.Outputs
{
	using System.Collections.Generic;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;

	[Output(Type = "text-value")]
	public class TextValueOutput : IOutputManager
	{
		private TextView OutputText { get; set; }

		public View GetView(OutputFieldMetadata outputField, object value, MyFormHandler myFormHandler, FormMetadata formMetadata, List<FormInputManager> inputsManager)
		{
			var textValue = value.CastTObject<TextValue<object>>();
			this.OutputText = new TextView(Application.Context) { Text = outputField.Label + ": " + textValue.Value };
			return this.OutputText;
		}
	}
}