namespace AndroidUiMetadateFramework.Core.Models
{
	using System.Collections.Generic;
	using UiMetadataFramework.Core;

	public class FormParameters
	{
		public FormParameters(FormMetadata form, IDictionary<string, object> parameters)
		{
			this.Form = form;
			this.Parameters = parameters;
		}

		public FormMetadata Form { get; set; }
		public IDictionary<string, object> Parameters { get; set; }
	}
}