namespace AndroidUiMetadataFramework.Core.Models
{
	using System.Collections.Generic;
	using UiMetadataFramework.Core;

	public class FormParameter
	{
		public FormParameter(FormMetadata form, IDictionary<string, object> parameters = null)
		{
			this.Form = form;
			this.Parameters = parameters;
		}

		public FormMetadata Form { get; set; }
		public IDictionary<string, object> Parameters { get; set; }
	}
}