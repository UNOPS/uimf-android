﻿namespace AndroidUiMetadateFramework.Core.Outputs
{
	using System;
	using System.Collections.Generic;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;
	using UiMetadataFramework.Core;

	[Output(Type = "datetime")]
	public class DateOutput : IOutputManager
	{
		private TextView OutputText { get; set; }

		public View GetView(OutputFieldMetadata outputField, object value, MyFormHandler myFormHandler, FormMetadata formMetadata, List<FormInputManager> inputsManager)
		{
			this.OutputText = new TextView(Application.Context);
			if (value != null)
			{
				DateTime datetime = value.CastTObject<DateTime>();
				this.OutputText.Text = outputField.Label + ": " + datetime.ToShortDateString();
			}

			return this.OutputText;
		}
	}
}