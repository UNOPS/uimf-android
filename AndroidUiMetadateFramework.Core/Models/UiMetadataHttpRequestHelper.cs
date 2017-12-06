namespace AndroidUiMetadateFramework.Core.Models
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Text;
	using System.Threading.Tasks;
	using Newtonsoft.Json;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.MediatR;

	public static class UiMetadataHttpRequestHelper
	{
		public static async Task<FormMetadata> GetFormMetadata(string formId, string url, string requestCookies)
		{
			var formResponse = new FormMetadata();
			var cookies = new CookieContainer();
			var handler = new HttpClientHandler { CookieContainer = cookies };
			var address = new Uri(url + "/" + formId);

			var cookiesList = !string.IsNullOrEmpty(requestCookies)
				? JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(requestCookies)
				: new List<KeyValuePair<string, string>>();

			FillCookiesRequest(cookiesList, cookies, address);

		    using (var client = new HttpClient(handler))
		    {
		        client.Timeout = TimeSpan.FromSeconds(30);
		        client.BaseAddress = address;

		        var response = await client.GetAsync(address);

		        if (response.IsSuccessStatusCode)
		        {
		            var data = await ReadResponseContent(response);
		            formResponse = JsonConvert.DeserializeObject<FormMetadata>(data);
                }
		    }

			return formResponse;
		}

		public static async Task<InvokeFormResponse> InvokeForm(string url, string requestCookies, object param = null)
		{
			var formResponse = new InvokeFormResponse();
			var cookies = new CookieContainer();
			var handler = new HttpClientHandler { CookieContainer = cookies };
			var address = new Uri(url);

			var cookiesList = !string.IsNullOrEmpty(requestCookies)
				? JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(requestCookies)
				: new List<KeyValuePair<string, string>>();

			FillCookiesRequest(cookiesList, cookies, address);

			var jsonReq = JsonConvert.SerializeObject(param);
			var content = new StringContent(jsonReq, Encoding.UTF8, "application/json");

		    using (var client = new HttpClient(handler))
		    {
		        client.Timeout = TimeSpan.FromSeconds(30);
		        client.BaseAddress = address;

		        var response = await client.PostAsync(address, content);
		        if (response.IsSuccessStatusCode)
		        {
		            var data = await ReadResponseContent(response);
		            formResponse.Response = JsonConvert.DeserializeObject<List<InvokeForm.Response>>(data);

		            GetCookiesResponse(cookies, cookiesList, address);
		            formResponse.Cookies = JsonConvert.SerializeObject(cookiesList);
		        }
            }         
			return formResponse;
		}

		public static async Task<string> GetAllFormsMetadata(string url, string requestCookies)
		{
			var cookies = new CookieContainer();
			var handler = new HttpClientHandler { CookieContainer = cookies };
			var address = new Uri(url);

			var cookiesList = !string.IsNullOrEmpty(requestCookies)
				? JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(requestCookies)
				: new List<KeyValuePair<string, string>>();

			FillCookiesRequest(cookiesList, cookies, address);
			using (var client = new HttpClient(handler))
			{
				client.Timeout = TimeSpan.FromSeconds(30);
				client.BaseAddress = address;

				var response = await client.GetAsync(address);


				if (response.IsSuccessStatusCode)
				{
					var data = await ReadResponseContent(response);
					return data;
				}
			}
			
			return null;
		}

		private static void FillCookiesRequest(List<KeyValuePair<string, string>> fromCookiesList, CookieContainer cookieContainer, Uri address)
		{
			foreach (KeyValuePair<string, string> kvpCookie in fromCookiesList)
			{
				cookieContainer.Add(address, new Cookie(kvpCookie.Key, kvpCookie.Value));
			}
		}

		private static void GetCookiesResponse(CookieContainer cookieContainer, List<KeyValuePair<string, string>> toCookiesList, Uri address)
		{
			IEnumerable<Cookie> responseCookies = cookieContainer.GetCookies(address).Cast<Cookie>();
			foreach (Cookie cookie in responseCookies)
			{
				toCookiesList.Add(new KeyValuePair<string, string>(cookie.Name, cookie.Value));
			}
		}

		private static async Task<string> ReadResponseContent(HttpResponseMessage response)
		{
			var stream = await response.Content.ReadAsStreamAsync();
			StreamReader reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}
	}
}