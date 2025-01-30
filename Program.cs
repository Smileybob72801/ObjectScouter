using ObjectScouter.App;
using ObjectScouter.Repositories;
using ObjectScouter.Services;
using ObjectScouter.UserInteraction;
using System.Net;

namespace ObjectScouter
{
    internal class Program
	{
		static async Task Main()
		{
			const string ApiBaseAddress = "https://api.restful-api.dev/";

			IUserInteraction userInteraction = new UserInteractionConsole();
			HttpClient httpClient = new()
			{
				BaseAddress = new Uri(ApiBaseAddress)
			};
			IItemRepository itemRepository = new ItemRepository();
			IApiService apiService = new ApiService(httpClient, itemRepository);
			IItemService itemService = new ItemService(userInteraction, apiService);
			IMenuService menuService = new MenuService(userInteraction, apiService, itemRepository, itemService);
			AsyncApiApp asyncApiApp = new(userInteraction, itemRepository, itemService, menuService);

			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			try
			{
				await asyncApiApp.Run();
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex);
            }
        }
	}
}
