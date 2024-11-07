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

            _properties = GetAllProperties();
		}

		public void Run()
        {
            PrintObjects();

            ListProperties();
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

        private HashSet<PropertyInfo> GetAllProperties()
        {
            HashSet<PropertyInfo> result = new (new PropertyInfoComparer());

            foreach (Item item in _items)
            {
                IEnumerable<PropertyInfo> properties = item.GetNonNullProperties();

                foreach (PropertyInfo property in properties)
                {
                    result.Add(property);
                }
            }

            return result;
        }

        private void ListProperties()
        {
            Console.WriteLine("Searchable properties: ");

            foreach (PropertyInfo property in _properties)
            {
                Console.WriteLine(property.Name);
            }
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

    internal class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
    {
        public bool Equals(PropertyInfo? x, PropertyInfo? y)
        {
            return string.Equals(x?.Name, y?.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(PropertyInfo obj)
        {
            return obj.Name.ToLowerInvariant().GetHashCode();
        }
    }
}
