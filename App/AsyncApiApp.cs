using AsyncRestApi.Model;
using AsyncRestApi.Services;
using System.Reflection;

namespace AsyncRestApi.App
{
    internal class AsyncApiApp
	{
		const string ApiBaseAddress = "https://api.restful-api.dev/";
        const string RequestUri = "/objects";

        private readonly IApiReaderService _apiReaderService;

		private readonly IEnumerable<PropertyInfo> _properties;

        private readonly IEnumerable<Item> _items;

		public AsyncApiApp(IApiReaderService apiReaderService)
		{
			_apiReaderService = apiReaderService;

            _items = GetAllObjects().Result;

            _properties = FindAllProperties();
		}

		public void Run()
        {
            PrintObjects();

            FindPropertyByName();
        }

        private void PrintObjects()
        {
            foreach (Item item in _items)
            {
                Console.WriteLine(item);
            }
        }

        private async Task<IEnumerable<Item>> GetAllObjects()
        {
            var result = await _apiReaderService.ReadAsync(ApiBaseAddress, RequestUri);

            return result;
        }

        private IEnumerable<PropertyInfo> FindAllProperties()
        {
            List<PropertyInfo> result = [];

            foreach (Item item in _items)
            {
                IEnumerable<PropertyInfo> properties = item.GetNonNullProperties();

                result.AddRange(properties);
            }

            return result;
        }

        private void ListProperties()
        {

        }

        private void FindPropertyByName()
        {
            foreach (Item item in _items)
            {
				IEnumerable<PropertyInfo> properties = item.GetNonNullProperties();

                foreach (PropertyInfo property in properties)
                {
                    if (property.Name.Equals("description", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Found a matching property: {property.Name}");
                    }
                }
            }
        }
    }
}
