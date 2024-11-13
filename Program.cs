using AsyncRestApi.App;
using AsyncRestApi.Services;
using AsyncRestApi.UserInteraction;

namespace AsyncRestApi
{
    internal class Program
	{
		static void Main()
		{
			IUserInteraction userInteraction = new UserInteractionConsole();
			IApiReaderService apiReaderService = new ApiReaderService();
			AsyncApiApp asyncApiApp = new(apiReaderService, userInteraction);

			try
			{
				Task appTask = asyncApiApp.Run();
				appTask.GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex);
            }

            Console.WriteLine("Press any key to close application...");
            Console.ReadKey();
        }
	}
}
