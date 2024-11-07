using AsyncRestApi.App;
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
}
