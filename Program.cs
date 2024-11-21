using ObjectScouter.App;
using ObjectScouter.Repositories;
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
			IItemRepository itemRepository = new ItemRepository();
			IApiService apiReaderService = new ApiService(httpClient, itemRepository);
			IItemService itemService = new ItemService(userInteraction);
			AsyncApiApp asyncApiApp = new(apiReaderService, userInteraction, itemRepository, itemService);

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
        }
	}
}
