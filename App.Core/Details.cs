using System;
using MediatR;
using UiMetadataFramework.Core;
using UiMetadataFramework.Core.Binding;
using UiMetadataFramework.MediatR;

namespace App.Core
{
	[Form(Label = "More Details", PostOnLoad = true)]
	public class Details : IForm<Details.Request, Details.Response>
	{
		public Response Handle(Request message)
		{
			return new Response
			{
				FirstName = message.FirstName,
				Email = "narutouzumaki@gmail.com",
				Telephone = "0597228747"
			};
		}

		public class Response : FormResponse
		{
			[OutputField(Label = "First name", OrderIndex = 1)]
			public string FirstName { get; set; }

			[OutputField]
			public string Email { get; set; }

			[OutputField]
			public string Telephone { get; set; }

		}

		public class Request : IRequest<Response>
		{
			[InputField(Hidden = true)]
			public string FirstName { get; set; }
		}
	}
}