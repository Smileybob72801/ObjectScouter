using System.Text.Json.Serialization;

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

	public record Data(
		[property: JsonPropertyName("color")] string color,
		[property: JsonPropertyName("capacity")] string capacity,
		[property: JsonPropertyName("capacity GB")] int? capacityGB,
		[property: JsonPropertyName("price")] double? price,
		[property: JsonPropertyName("generation")] string generation,
		[property: JsonPropertyName("year")] int? year,
		[property: JsonPropertyName("CPU model")] string CPUmodel,
		[property: JsonPropertyName("Hard disk size")] string Harddisksize,
		[property: JsonPropertyName("Strap Colour")] string StrapColour,
		[property: JsonPropertyName("Case Size")] string CaseSize,
		[property: JsonPropertyName("Color")] string Color,
		[property: JsonPropertyName("Description")] string Description,
		[property: JsonPropertyName("Capacity")] string Capacity,
		[property: JsonPropertyName("Screen size")] double? Screensize,
		[property: JsonPropertyName("Generation")] string Generation,
		[property: JsonPropertyName("Price")] string Price
	);

	public record Root(
		[property: JsonPropertyName("id")] string id,
		[property: JsonPropertyName("name")] string name,
		[property: JsonPropertyName("data")] Data data
	);
}
