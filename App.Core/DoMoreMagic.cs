using System;
using MediatR;
using UiMetadataFramework.Core;
using UiMetadataFramework.Core.Binding;
using UiMetadataFramework.MediatR;

namespace App.Core
{
	[Form(Label = "Do some magic", PostOnLoad = true)]
	public class DoMoreMagic : IForm<DoMoreMagic.Request, DoMoreMagic.Response>
	{
		public Response Handle(Request message)
		{
			return new Response
			{
				FirstName = message.FirstName,
				Height = message.Height,
				DateOfBirth = message.DateOfBirth
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
			[InputField]
			public string FirstName { get; set; }

			[InputField]
			public DateTime? DateOfBirth { get; set; }

			[InputField]
			public int Height { get; set; }
		}
	}
}