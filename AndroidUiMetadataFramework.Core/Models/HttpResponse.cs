namespace AndroidUiMetadataFramework.Core.Models
{
	using UiMetadataFramework.MediatR;
	using System.Collections.Generic;

	public class InvokeFormResponse : HttpResponse
	{
		public List<InvokeForm.Response> Response { get; set; }
	}

	public class HttpResponse
	{
		public string Cookies { get; set; }
	}
}