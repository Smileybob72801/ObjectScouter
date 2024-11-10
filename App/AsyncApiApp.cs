using AsyncRestApi.Model;
using AsyncRestApi.Services;
using AsyncRestApi.UserInteraction;
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

        private readonly IUserInteraction _userInteraction;

		public AsyncApiApp(IApiReaderService apiReaderService, IUserInteraction userInteraction)
		{
			_apiReaderService = apiReaderService;

            _items = GetAllObjects().Result;

            _properties = GetAllProperties();

            _userInteraction = userInteraction;
		}

		public void Run()
        {
            _userInteraction.PrintObjects(_items);

            _userInteraction.ListProperties(_properties);

            FindPropertyByName("capacity");
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

        private void FindPropertyByName(string target)
        {
            foreach (Item item in _items)
            {
				IEnumerable<PropertyInfo> properties = item.GetNonNullProperties();

                foreach (PropertyInfo property in properties)
                {
					if (property.Name.Contains(target, StringComparison.OrdinalIgnoreCase))
                    {
						object? targetInstance = GetTargetInstance(item, property);

						object? value = property.GetValue(targetInstance);

                        Console.WriteLine($"Found a matching property: {property.Name}, {value}");
                    }
                }
            }
        }

        private object? GetTargetInstance(Item item, PropertyInfo property)
        {
            if (property.DeclaringType == typeof(Item))
            {
                return item;
            }

            if (property.DeclaringType == typeof(Data))
            {
                return item.data;
            }

            throw new InvalidOperationException($"Target instance must be {nameof(Data)} or {nameof(Item)}");
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
