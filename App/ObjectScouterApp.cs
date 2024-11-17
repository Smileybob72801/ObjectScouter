using ObjectScouter.Model;
using ObjectScouter.Services;
using ObjectScouter.UserInteraction;
using ObjectScouter.Helpers;
using System.Reflection;
using System.Text.Json;
using ObjectScouter.Repositories;

namespace ObjectScouter.App
{
	internal class AsyncApiApp(
        IApiReaderService apiReaderService,
        IUserInteraction userInteraction,
        IItemRepository itemRepository)
	{
		const string ApiBaseAddress = "https://api.restful-api.dev/";
        const string RequestUri = "/objects";

        private readonly IApiReaderService _apiReaderService = apiReaderService;

		private IEnumerable<PropertyInfo>? _properties;

        private IEnumerable<Item>? _items;

        private readonly IUserInteraction _userInteraction = userInteraction;

        private readonly IItemRepository _itemRepository = itemRepository;

        private readonly List<string> _menuOptions = ["Exit", "Search"];

        // This method does not really need to be async, this is just to demonstrate
        // that the ui thread does not have to be blocked while work is being done.
		public async Task Run()
        {
            _userInteraction.DisplayText("Contacting database...");

            Data testData = new()
			{
				Color = "Neon Green",
				Description = "Test description",
				capacityGB = 128,
				price = 999_999,
				year = 1984
			};


            Item testItem = new()
            {
                id = "14",
                name = "test",
                data = testData
            };

			await _apiReaderService.PostAsync(RequestUri, testItem);

			Task mainTask = Task.Run(async () =>
            {
				_items = await GetAllObjects();
				_properties = GetAllProperties();
				_userInteraction.PrintObjects(_items);
				_userInteraction.ListProperties(_properties);
			});
			await mainTask;

			string choice = GetMenuChoice();

            if (string.Equals(choice, "search", StringComparison.OrdinalIgnoreCase))
            {
				string targetProperty = _userInteraction.GetValidString("Enter a property to search for: ");
				
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
            // Just to simulate background work
            //await Task.Delay(4000);

            var result = await _apiReaderService.ReadAsync(RequestUri);

            return result;
        }

        private HashSet<PropertyInfo> GetAllProperties()
        {
            HashSet<PropertyInfo> result = new (new PropertyInfoComparer());

            if (_items is null)
            {
                throw new InvalidOperationException("Items is null.");
            }

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
			if (_items is null)
			{
				throw new InvalidOperationException("Items is null.");
			}

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
