using System.Text.Json;
using NumberVerifier.Model;

namespace NumberVerifier.Services
{
    internal interface IApiReaderService
	{
		Task<IEnumerable<Root>> ReadAsync(string baseAddress, string requestUri);
	}

	internal class ApiReaderService : IApiReaderService
	{
		public async Task<IEnumerable<Root>> ReadAsync(string baseAddress, string requestUri)
		{
			using HttpClient client = new();

			client.BaseAddress = new Uri(baseAddress);

			HttpResponseMessage responseMessage = await client.GetAsync(requestUri);

            responseMessage.EnsureSuccessStatusCode();

			string result = await responseMessage.Content.ReadAsStringAsync();

			IEnumerable<Root>? roots = JsonSerializer.Deserialize<IEnumerable<Root>>(result);

			return roots ?? throw new InvalidOperationException("Result is null.");
		}
	}


}
