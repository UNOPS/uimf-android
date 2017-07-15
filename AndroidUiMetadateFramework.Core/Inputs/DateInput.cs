﻿namespace AndroidUiMetadateFramework.Core.Inputs
{
	using System;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;

	[Input(Type = "datetime")]
	public class DateInput : IInputManager
	{
		private DatePicker dateInput { get; set; }

		public View GetView(Activity activity)
		{
			this.dateInput = new DatePicker(activity);
			this.dateInput.ScaleX = 0.5f;
			this.dateInput.ScaleY = 0.5f;
			return this.dateInput;
		}

		public object GetValue()
		{
			return this.dateInput.DateTime;
		}

		public void SetValue(object value)
		{
			this.dateInput.DateTime = (DateTime)value;
		}
	}
}