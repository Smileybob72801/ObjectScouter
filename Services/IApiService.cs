using ObjectScouter.Model;

namespace ObjectScouter.Services
{
	internal interface IApiService
	{
		Task PostAsync(string requestUri, Item item);
		Task PutAsync(string requestUri, Item item);
		Task<T> ReadAsync<T>(string requestUri);
	}
}
