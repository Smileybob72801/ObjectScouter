using AsyncRestApi.Model;
using AsyncRestApi.Services;

namespace AsyncRestApi
{
    internal class Program
	{
		const string ApiBaseAddress = "https://api.restful-api.dev/";
		const string RequestUri = "/objects";
		static void Main()
		{
			IApiReaderService apiReaderService = new ApiReaderService();

			try
			{
				IEnumerable<Item> items = GetAllObjects(apiReaderService).Result;

				PrintObjects(items);
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex);
            }

			Console.ReadKey();
        }

		private static void PrintObjects(IEnumerable<Item> objects)
		{
			foreach (Item item in objects)
			{
                Console.WriteLine(item);
            }
		}

		static async Task<IEnumerable<Item>> GetAllObjects(IApiReaderService apiReaderService)
		{
			var result = await apiReaderService.ReadAsync(ApiBaseAddress, RequestUri);

			return result;
        }
	}
}
