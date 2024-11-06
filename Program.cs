using NumberVerifier.Model;
using NumberVerifier.Services;

namespace NumberVerifier
{
    internal class Program
	{
		const string ApiBaseAddress = "https://api.restful-api.dev/";
		const string RequestUri = "/objects";
		static void Main()
		{
			IApiReaderService apiReaderService = new ApiReaderService();

			var root = UseApiAsync(apiReaderService).Result;

			Console.ReadKey();
        }

		static async Task<IEnumerable<Root>> UseApiAsync(IApiReaderService apiReaderService)
		{
			var result = await apiReaderService.ReadAsync(ApiBaseAddress, RequestUri);

			return result;
        }
	}
}
