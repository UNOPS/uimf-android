namespace AndroidUiMetadataFramework.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using UiMetadataFramework.Core;
    using UiMetadataFramework.MediatR;

    public static class UiMetadataHttpRequestHelper
    {
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
                var data = await ReadResponseContent(response);
                if (response.IsSuccessStatusCode)
                {
                    return data;
                }
                var exception = JsonConvert.DeserializeObject<HttpException>(data);
                throw new Exception(exception.Error);
            }
        }

        public static async Task<FormMetadata> GetFormMetadata(string formId, string url, string requestCookies)
        {
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
                var data = await ReadResponseContent(response);
                if (response.IsSuccessStatusCode)
                {
                    var formResponse = JsonConvert.DeserializeObject<FormMetadata>(data);
                    return formResponse;
                }
                var exception = JsonConvert.DeserializeObject<HttpException>(data);
                throw new Exception(exception.Error);
            }
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
                var data = await ReadResponseContent(response);
                if (response.IsSuccessStatusCode)
                {
                    formResponse.Response = JsonConvert.DeserializeObject<List<InvokeForm.Response>>(data);
                }
                else
                {
                    var exception = JsonConvert.DeserializeObject<HttpException>(data);
                    throw new Exception(exception.Error);
                }
                var newCookies = GetCookiesResponse(handler.CookieContainer, address, response.Headers);
                formResponse.Cookies = JsonConvert.SerializeObject(newCookies);
            }
            return formResponse;
        }

        private static void FillCookiesRequest(List<KeyValuePair<string, string>> fromCookiesList, CookieContainer cookieContainer, Uri address)
        {
            foreach (KeyValuePair<string, string> kvpCookie in fromCookiesList)
            {
                cookieContainer.Add(address, new Cookie(kvpCookie.Key, kvpCookie.Value));
            }
        }

        private static List<KeyValuePair<string, string>> GetCookiesResponse(CookieContainer cookieContainer,
            Uri address,
            HttpHeaders responseHeaders)
        {
            var cookies = new List<KeyValuePair<string, string>>();
            IEnumerable<string> rawCookies;
            var isCookieHeader = responseHeaders.TryGetValues("Set-Cookie", out rawCookies);

            if (isCookieHeader)
            {
                foreach (var rawCookie in rawCookies.ToList())
                {
                    var cookie = rawCookie.Split(';')[0];
                    var cookieValues = cookie.Split('=');
                    if (cookieValues.Length > 1 && !string.IsNullOrEmpty(cookieValues[1]))
                    {
                        cookies.Add(new KeyValuePair<string, string>(cookieValues[0], cookieValues[1]));
                    }
                }
            }
            else
            {
                var responseCookies = cookieContainer.GetCookies(address).Cast<Cookie>();
                foreach (Cookie cookie in responseCookies)
                {
                    cookies.Add(new KeyValuePair<string, string>(cookie.Name, cookie.Value));
                }
            }

            return cookies;
        }

        private static async Task<string> ReadResponseContent(HttpResponseMessage response)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}