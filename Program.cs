using ObjectScouter.App;
using ObjectScouter.Services;
using ObjectScouter.UserInteraction;

namespace ObjectScouter
{
    internal class Program
	{
		static void Main()
		{
			IUserInteraction userInteraction = new UserInteractionConsole();
			HttpClient httpClient = new();
			IApiReaderService apiReaderService = new ApiReaderService(httpClient);
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
