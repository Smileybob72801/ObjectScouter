using System.Text.Json;
using ObjectScouter.Model;
using System.Net.Http;
using System.Text;
using System.Net.Http.Json;
using ObjectScouter.Repositories;

namespace ObjectScouter.Services
{
	internal interface IApiReaderService
	{
		Task PostAsync(string requestUri, Item item);
		Task<T> ReadAsync<T>(string requestUri);
	}

	internal partial class ApiReaderService(HttpClient httpClient, IItemRepository itemRepository) : IApiReaderService
	{
		private readonly HttpClient _httpClient = httpClient;
		private readonly IItemRepository _itemRepository = itemRepository;

		private string BuildObjectsUri()
		{
			StringBuilder stringBuilder = new();

			string[] ids = _itemRepository.GetIds().ToArray();

			stringBuilder.Append($"?id={ids[0]}");

			for (int i = 1; i < ids.Length; i++)
			{
				stringBuilder.Append($"&id={ids[i]}");
			}

			return stringBuilder.ToString();
		}

		public async Task<T> ReadAsync<T>(string requestUri)
		{
			string fullUri = requestUri + BuildObjectsUri();

			HttpResponseMessage responseMessage = await _httpClient.GetAsync(fullUri);

            responseMessage.EnsureSuccessStatusCode();

			string result = await responseMessage.Content.ReadAsStringAsync();

			T? roots = JsonSerializer.Deserialize<T>(result);

			return roots ?? throw new InvalidOperationException("Result is null.");
		}

		public async Task PostAsync(string requestUri, Item item)
		{
			string jsonContent = JsonSerializer.Serialize(item);

			StringContent content = new(jsonContent, Encoding.UTF8, "application/json");

			HttpResponseMessage responseMessage = await _httpClient.PostAsync(requestUri, content);

			if (responseMessage.IsSuccessStatusCode)
			{
				string responseBody  = await responseMessage.Content.ReadAsStringAsync();
                await Console.Out.WriteLineAsync("Successfull response:");
                await Console.Out.WriteLineAsync(responseBody);
				Item? returnedAsPosted = JsonSerializer.Deserialize<Item>(responseBody);
				string? postedId = returnedAsPosted?.Id;
				if (postedId is not null)
				{
					await _itemRepository.AddId(postedId); 
				}
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
