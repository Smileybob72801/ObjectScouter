using AsyncRestApi.Model;
using AsyncRestApi.Services;
using AsyncRestApi.UserInteraction;
using AsyncRestApi.Helpers;
using System.Reflection;

namespace AsyncRestApi.App
{
	internal class AsyncApiApp
	{
		const string ApiBaseAddress = "https://api.restful-api.dev/";
        const string RequestUri = "/objects";

        private readonly IApiReaderService _apiReaderService;

		private IEnumerable<PropertyInfo>? _properties;

        private IEnumerable<Item>? _items;

        private readonly IUserInteraction _userInteraction;

        private readonly List<string> _menuOptions = ["Exit", "Search"];

		public AsyncApiApp(IApiReaderService apiReaderService, IUserInteraction userInteraction)
		{
			_apiReaderService = apiReaderService;

            _userInteraction = userInteraction;
		}

		public async Task Run()
        {
            _userInteraction.DisplayText("Contacting database...");

            Task mainTask = Task.Run(async () =>
            {
				_items = await GetAllObjects();
				_properties = GetAllProperties();
				_userInteraction.PrintObjects(_items);
				_userInteraction.ListProperties(_properties);
			});

            string choice = GetMenuChoice();

            if (string.Equals(choice, "search", StringComparison.OrdinalIgnoreCase))
            {
				string targetProperty = _userInteraction.GetValidString("Enter a property to search for: ");
				await mainTask;
				FindPropertyByName(targetProperty);
			}

            _userInteraction.DisplayText("");
        }

        private string GetMenuChoice()
        {
            _userInteraction.DisplayText("Choose an option: ");

            foreach (string option in _menuOptions)
            {
                _userInteraction.DisplayText($"{option}");
            }

            string result;

            do
            {
                result = _userInteraction.GetValidString();
            }
            while (!_menuOptions.Contains(result, StringComparer.OrdinalIgnoreCase));

            return result;
        }

        private async Task<IEnumerable<Item>> GetAllObjects()
        {
            await Task.Delay(4000);

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

                        _userInteraction.DisplayText($"Found an item with target property: {item.name} with {property.Name}: {value}");
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
}
