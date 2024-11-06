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

			UseApiAsync(apiReaderService);

			Console.ReadKey();
        }

		static async void UseApiAsync(IApiReaderService apiReaderService)
		{
			var result = await apiReaderService.ReadAsync(ApiBaseAddress, RequestUri);

            await Console.Out.WriteLineAsync(result);
        }
	}
}
