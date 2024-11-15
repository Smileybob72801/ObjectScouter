using System.Text.Json;
using ObjectScouter.Model;
using System.Net.Http;
using System.Text;
using System.Net.Http.Json;

namespace ObjectScouter.Services
{
    internal interface IApiReaderService
	{
		Task PostAsync(string baseAddress, string requestUri, Item item);
		Task<IEnumerable<Item>> ReadAsync(string baseAddress, string requestUri);
	}

	internal class ApiReaderService(HttpClient httpClient) : IApiReaderService
	{
		private readonly HttpClient _httpClient = httpClient;
		public async Task<IEnumerable<Item>> ReadAsync(string baseAddress, string requestUri)
		{
			_httpClient.BaseAddress = new Uri(baseAddress);

			HttpResponseMessage responseMessage = await _httpClient.GetAsync(requestUri);

            responseMessage.EnsureSuccessStatusCode();

			string result = await responseMessage.Content.ReadAsStringAsync();

			IEnumerable<Item>? roots = JsonSerializer.Deserialize<IEnumerable<Item>>(result);

			return roots ?? throw new InvalidOperationException("Result is null.");
		}

		public async Task PostAsync(string baseAddress, string requestUri, Item item)
		{
			_httpClient.BaseAddress = new Uri(baseAddress);

			string jsonContent = JsonSerializer.Serialize(item);

			StringContent content = new(jsonContent, Encoding.UTF8, "application/json");

			HttpResponseMessage responseMessage = await _httpClient.PostAsync(requestUri, content);

			if (responseMessage.IsSuccessStatusCode)
			{
				string responseBody  = await responseMessage.Content.ReadAsStringAsync();
                await Console.Out.WriteLineAsync("Successfull response:");
                await Console.Out.WriteLineAsync(responseBody);
            }
			else
			{
                await Console.Out.WriteLineAsync($"Failed with status code: {responseMessage.StatusCode}");
				string errorBody = await responseMessage.Content.ReadAsStringAsync();
                await Console.Out.WriteLineAsync("Error details:");
                await Console.Out.WriteLineAsync(errorBody);
            }
        }
	}
}
