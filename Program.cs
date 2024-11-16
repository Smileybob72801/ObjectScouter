using ObjectScouter.App;
using ObjectScouter.Services;
using ObjectScouter.UserInteraction;
using System.Net;

namespace ObjectScouter
{
    internal class Program
	{
		static void Main()
		{
			const string ApiBaseAddress = "https://api.restful-api.dev/";

			IUserInteraction userInteraction = new UserInteractionConsole();
			HttpClient httpClient = new()
			{
				BaseAddress = new Uri(ApiBaseAddress)
			};
			IApiReaderService apiReaderService = new ApiReaderService(httpClient);
			AsyncApiApp asyncApiApp = new(apiReaderService, userInteraction);

			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

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
