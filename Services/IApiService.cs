using ObjectScouter.Model;

namespace ObjectScouter.Services
{
	internal interface IApiService
	{
		Task PostAsync(string requestUri, Item item);
		Task PutAync(string requestUri, Item item);
		Task<T> ReadAsync<T>(string requestUri);
	}
}
