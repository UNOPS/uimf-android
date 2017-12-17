namespace AndroidUiMetadataFramework.Core.Outputs
{
	using System;
	using System.Collections.Generic;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadataFramework.Core.Attributes;
	using AndroidUiMetadataFramework.Core.Managers;
	using AndroidUiMetadataFramework.Core.Models;
	using Newtonsoft.Json.Linq;
	using UiMetadataFramework.Core;

	[Output(Type = "datetime")]
	public class DateOutput : IOutputManager
	{
		private TextView OutputText { get; set; }

		public View GetView(OutputFieldMetadata outputField, object value, MyFormHandler myFormHandler, FormMetadata formMetadata, List<FormInputManager> inputsManager)
		{
			this.OutputText = new TextView(Application.Context);
		    myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("TextView", this.OutputText);
            if (value != null)
			{
			    var result = "";
			    if (value.GetType() == typeof(JValue))
			    {
			        var jValue = (JValue)value;
			        if (jValue.HasValues)
			        {
			            var datetime = jValue.CastTObject<DateTime>();
			            result = datetime.ToShortDateString();
			        }
			    }
                
				this.OutputText.Text = outputField.Label + ": " + result;
			}

			return this.OutputText;
		}
	}
}