using System;
using MediatR;
using UiMetadataFramework.Core;
using UiMetadataFramework.Core.Binding;
using UiMetadataFramework.MediatR;

namespace App.Core
{
	using System.Collections.Generic;
	using UiMetadataFramework.Basic.Output;

	[Form(Label = "Do some magic", PostOnLoad = true)]
	public class DoMoreMagic : IForm<DoMoreMagic.Request, DoMoreMagic.Response>
	{
		public Response Handle(Request message)
		{
			return new Response
			{
				Tabs = GetTabs(typeof(GeneralInfo).FullName, message.FirstName),
				FirstName = message.FirstName,
				Height = message.Height,
				DateOfBirth = message.DateOfBirth
			};
		}

		internal static Tabstrip GetTabs(string currentTab, string name)
		{
			return new Tabstrip
			{
				CurrentTab = currentTab,
				Tabs = new List<Tab>
				{
					new Tab
					{
						Label = "Basic info",
						Form = typeof(GeneralInfo).FullName,
						InputFieldValues = new Dictionary<string, object>
						{
							{ nameof(GeneralInfo.Request.FirstName), name }
						}
					},
					new Tab
					{
						Label = "Relatives",
						Form = typeof(Details).FullName,
						InputFieldValues = new Dictionary<string, object>
						{
							{ nameof(Details.Request.FirstName), name }
						}
					}
				}
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

			[OutputField(OrderIndex = 50)]
			public Tabstrip Tabs { get; set; }
		}

		public class Request : IRequest<Response>
		{
			[InputField(Hidden = true)]
			public string FirstName { get; set; }

			[InputField(Hidden = true)]
			public DateTime? DateOfBirth { get; set; }

			[InputField(Hidden = true)]
			public int Height { get; set; }
		}
	}
}