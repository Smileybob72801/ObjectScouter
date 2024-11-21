

namespace ObjectScouter.Services
{
	internal interface IMenuService
	{
		Task HandleCreateItemAsync();
		Task HandleDeleteItemAsync();
		Task HandleExitAsync();
		Task HandleListItemsAsync();
		Task HandleSearchAsync();
	}
}