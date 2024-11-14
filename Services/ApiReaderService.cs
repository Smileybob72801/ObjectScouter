using System.Text.Json;
using ObjectScouter.Model;

namespace ObjectScouter.Services
{
    internal interface IApiReaderService
	{
		Task<IEnumerable<Item>> ReadAsync(string baseAddress, string requestUri);
	}

	internal class ApiReaderService : IApiReaderService
	{
		public async Task<IEnumerable<Item>> ReadAsync(string baseAddress, string requestUri)
		{
			using HttpClient client = new();

			client.BaseAddress = new Uri(baseAddress);

			HttpResponseMessage responseMessage = await client.GetAsync(requestUri);

            responseMessage.EnsureSuccessStatusCode();

			string result = await responseMessage.Content.ReadAsStringAsync();

			IEnumerable<Item>? roots = JsonSerializer.Deserialize<IEnumerable<Item>>(result);

			return roots ?? throw new InvalidOperationException("Result is null.");
		}
	}


}
