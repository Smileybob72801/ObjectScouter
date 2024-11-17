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
		Task<IEnumerable<Item>> ReadAsync(string requestUri);
	}

	internal partial class ApiReaderService(HttpClient httpClient, IItemRepository itemRepository) : IApiReaderService
	{
		private readonly HttpClient _httpClient = httpClient;
		private readonly IItemRepository _itemRepository = itemRepository;
		public async Task<IEnumerable<Item>> ReadAsync(string requestUri)
		{
			HttpResponseMessage responseMessage = await _httpClient.GetAsync(requestUri);

            responseMessage.EnsureSuccessStatusCode();

			string result = await responseMessage.Content.ReadAsStringAsync();

			IEnumerable<Item>? roots = JsonSerializer.Deserialize<IEnumerable<Item>>(result);

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
				string? postedId = returnedAsPosted?.id;
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
