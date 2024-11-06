namespace NumberVerifier.Services
{
	internal interface IApiReaderService
	{
		Task<string> ReadAsync(string baseAddress, string requestUri);
	}

	internal class ApiReaderService : IApiReaderService
	{
		public async Task<string> ReadAsync(string baseAddress, string requestUri)
		{
			using HttpClient client = new();

			client.BaseAddress = new Uri(baseAddress);

			HttpResponseMessage responseMessage = await client.GetAsync(requestUri);

            responseMessage.EnsureSuccessStatusCode();

			string result = await responseMessage.Content.ReadAsStringAsync();

			return result ?? throw new InvalidOperationException("Result is null.");
		}
	}
}
