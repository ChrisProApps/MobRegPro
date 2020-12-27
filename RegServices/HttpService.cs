using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RegServices.Data;
using System.Text;


// System.Net.Http package also adds
//	Microsoft.Bcl
//  Microsoft.Bcl.Build
//
// Other code only requires Newtonsoft.Json package
using System.IO;

namespace RegServices
{
	// Used to 
	//	- up/download binary files
	//  - base class for
	//		- LicenseService.cs
	//		- MobRegService.cs
	//
	public class HttpService
	{
		private HttpClient client; 
		private string url;

		protected HttpService(string url)
		{
			var handler = new HttpClientHandler();
			handler.ClientCertificateOptions = ClientCertificateOption.Manual;
			handler.ServerCertificateCustomValidationCallback =
				(httpRequestMessage, cert, cetChain, policyErrors) =>
				{
					return true;
				};


			client = new HttpClient(handler);
			client.MaxResponseContentBufferSize = 256000;
			this.url = url;
		}

		/// <summary>
		/// Processes the async, can be POST or GET depending on input.
		/// </summary>
		/// <returns>Object T</returns>
		/// <param name="methodName">Method name</param>
		/// <param name="input">Input can be null then GET, else POST</param>
		/// <param name="result">Result type as output</param>
		/// <typeparam name="V">V input type</typeparam>
		/// <typeparam name="T">T output type</typeparam>
		protected async Task<T> ProcessAsync<V,T>(string methodName, V input, T result)
		{
			//MS formtat  : "Date":"\/Date(1448187300000+0100)\/"
			//Standard    : "Date":"2015-12-27T12:22:26.971227"
			JsonSerializerSettings microsoftDateFormatSettings = 
				new JsonSerializerSettings{ DateFormatHandling = DateFormatHandling.MicrosoftDateFormat};
				        
			string restRequest = input==null ? "" : JsonConvert.SerializeObject(input, microsoftDateFormatSettings);
			RestResult restResult;
			if(input != null) 
				restResult = await ExecuteAsync (restRequest, methodName);
			else 
				restResult = await GetAsync (methodName);

			if (restResult.statusCode == 0)
				result = JsonConvert.DeserializeObject<T> (restResult.RestResponse);
			else {
				(result as BaseResult).status = restResult.status;
				(result as BaseResult).statusCode = restResult.statusCode;
			}
			return result;
		}

		protected async Task<RestResult> ExecuteAsync(string postData, string method)
		{
			RestResult result = new RestResult ();
			try {
				HttpContent content = new System.Net.Http.StringContent (postData, System.Text.Encoding.UTF8, "application/json");
				string uri = url + method;
				var response = await client.PostAsync (uri, content);
				result.RestResponse = await response.Content.ReadAsStringAsync ();
				if(!response.IsSuccessStatusCode)
				{
					result.statusCode = ((int)response.StatusCode) ;
					result.status = response.ReasonPhrase;
					result.RestResponse = "";
				}
				
			} catch (Exception ex) {
				result.statusCode = 1;
				result.status = ex.Message;
			}
			return result;
		}

		protected async Task<RestResult> GetAsync(string method)
		{
			RestResult result = new RestResult ();
			try {
				string uri = url + method;
				var response = await client.GetAsync (uri);
				result.RestResponse = await response.Content.ReadAsStringAsync ();
				if(!response.IsSuccessStatusCode)
				{
					result.statusCode = ((int)response.StatusCode) ;
					result.status = response.ReasonPhrase;
					result.RestResponse = "";
				}

			} catch (Exception ex) {
				result.statusCode = 1;
				result.status = ex.Message;
			}
			return result;
		}

		/// <summary>
		/// Download into stream
		/// </summary>
		/// <returns>StatusResult: statusCode= Http response statuscode, status=Http response reasonphrase</returns>
		/// <param name="url">formatted URL (contains query parameters)</param>
		/// <param name="stream">Stream to download to</param>
		public async Task<StatusResult> LoadBinaryAsync(string url, Stream stream)
		{
			StatusResult result = new StatusResult ();
			try {
				HttpResponseMessage response =  await client.GetAsync(new Uri(url));
				await response.Content.CopyToAsync(stream);

				if(!response.IsSuccessStatusCode)
				{
					result.statusCode = ((int)response.StatusCode) ;
					result.status = response.ReasonPhrase;
				}

			} catch (Exception ex) {
				result.statusCode = 1;
				result.status = ex.Message;
			}
			return result;
		}

		/// <summary>
		/// Upload from stream
		/// </summary>
		/// <returns>StatusResult: statusCode= Http response statuscode, status=Http response reasonphrase</returns>
		/// <param name="url">formatted URL (contains query parameters)</param>
		/// <param name="stream">Stream to upload from</param>
		public async Task<StatusResult> SaveBinaryAsync(string url, Stream stream)
		{
			StatusResult result = new StatusResult ();
			try {
				HttpContent content = new System.Net.Http.StreamContent(stream); //StringContent (postData, System.Text.Encoding.UTF8, "application/json");
				//content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/image");
				HttpResponseMessage response =  await client.PostAsync(new Uri(url), content);
				if(!response.IsSuccessStatusCode)
				{
					result.statusCode = ((int)response.StatusCode) ;
					result.status = response.ReasonPhrase;
				}

			} catch (Exception ex) {
				result.statusCode = 1;
				result.status = ex.Message;
			}
			return result;
		}


	}
}

