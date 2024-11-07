using AsyncRestApi.Model;
using AsyncRestApi.Services;

namespace AsyncRestApi
{
    internal class Program
	{
		static void Main()
		{
			IApiReaderService apiReaderService = new ApiReaderService();
			AsyncApiApp asyncApiApp = new(apiReaderService);

			try
			{
				asyncApiApp.Run();
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex);
            }

			Console.ReadKey();
        }
	}

	internal class AsyncApiApp(IApiReaderService apiReaderService)
	{
		const string ApiBaseAddress = "https://api.restful-api.dev/";
		const string RequestUri = "/objects";

		private readonly IApiReaderService _apiReaderService = apiReaderService;

		public void Run()
		{
			IEnumerable<Item> items = GetAllObjects(_apiReaderService).Result;

			PrintObjects(items);
		}

		private void PrintObjects(IEnumerable<Item> objects)
		{
			foreach (Item item in objects)
			{
				Console.WriteLine(item);
			}
		}

		private async Task<IEnumerable<Item>> GetAllObjects(IApiReaderService apiReaderService)
		{
			var result = await apiReaderService.ReadAsync(ApiBaseAddress, RequestUri);

			return result;
		}
	}
}
