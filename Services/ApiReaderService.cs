using System.Text.Json;
using System.Text.Json.Serialization;

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

	public class Data
	{
		[JsonPropertyName("color")]
		public string? color { get; set; }

		[JsonPropertyName("capacity")]
		public string? capacity { get; set; }

		[JsonPropertyName("capacity GB")]
		public int? capacityGB { get; set; }

		[JsonPropertyName("price")]
		public double? price { get; set; }

		[JsonPropertyName("generation")]
		public string? generation { get; set; }

		[JsonPropertyName("year")]
		public int? year { get; set; }

		[JsonPropertyName("CPU model")]
		public string? CPUmodel { get; set; }

		[JsonPropertyName("Hard disk size")]
		public string? Harddisksize { get; set; }

		[JsonPropertyName("Strap Colour")]
		public string? StrapColour { get; set; }

		[JsonPropertyName("Case Size")]
		public string? CaseSize { get; set; }

		[JsonPropertyName("Color")]
		public string? Color { get; set; }

		[JsonPropertyName("Description")]
		public string? Description { get; set; }

		[JsonPropertyName("Capacity")]
		public string? Capacity { get; set; }

		[JsonPropertyName("Screen size")]
		public double? Screensize { get; set; }

		[JsonPropertyName("Generation")]
		public string? Generation { get; set; }

		[JsonPropertyName("Price")]
		public string? Price { get; set; }
	}

	public class Root
	{
		[JsonPropertyName("id")]
		public string? id { get; set; }

		[JsonPropertyName("name")]
		public string? name { get; set; }

		[JsonPropertyName("data")]
		public Data? data { get; set; }
	}


}
