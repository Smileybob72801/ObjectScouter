using ObjectScouter.Model;

namespace ObjectScouter.Services
{
	internal interface IApiReaderService
	{
		Task PostAsync(string requestUri, Item item);
		Task PutAync(string requestUri, Item item);
		Task<T> ReadAsync<T>(string requestUri);
	}
}
