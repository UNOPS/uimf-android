using System;
using MediatR;
using UiMetadataFramework.Core;
using UiMetadataFramework.Core.Binding;
using UiMetadataFramework.MediatR;

namespace App.Core
{
	[Form(Label = "General Info", PostOnLoad = true)]
	public class GeneralInfo : IForm<GeneralInfo.Request, GeneralInfo.Response>
	{
		public Response Handle(Request message)
		{
			return new Response
			{
				FirstName = message.FirstName,
				Height = 4,
				DateOfBirth = DateTime.Now
			};
		}

		public class Response : FormResponse
		{
			[OutputField(Label = "First name", OrderIndex = 1)]
			public string FirstName { get; set; }

			[OutputField]
			public DateTime? DateOfBirth { get; set; }

			[OutputField]
			public int Height { get; set; }
		}

		public class Request : IRequest<Response>
		{
			[InputField(Hidden = true)]
			public string FirstName { get; set; }

		}
	}
}