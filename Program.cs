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

			try
			{
				IEnumerable<Root> roots = GetAllObjects(apiReaderService).Result;

				PrintObjects(roots);
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex);
            }

			Console.ReadKey();
        }

		private static void PrintObjects(IEnumerable<Root> objects)
		{
			foreach (Root root in objects)
			{
                Console.WriteLine(root);
            }
		}

		static async Task<IEnumerable<Root>> GetAllObjects(IApiReaderService apiReaderService)
		{
			var result = await apiReaderService.ReadAsync(ApiBaseAddress, RequestUri);

			return result;
        }
	}
}
