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

		private HashSet<string>? _properties;

        private IEnumerable<Item>? _items;

        private readonly IUserInteraction _userInteraction = userInteraction;

        private readonly IItemRepository _itemRepository = itemRepository;

        private readonly List<string> _menuOptions = ["Exit", "Search"];

        // This method does not really need to be async, this is just to demonstrate
        // that the ui thread does not have to be blocked while work is being done.
        // Artificial delays have been added to various modules to aid demonstration.
		public async Task Run()
        {
			// TODO: Move all awaited Tasks to another method so we can get to UI
			await _itemRepository.LoadFromFileAsync();
            _userInteraction.DisplayText("Contacting database...");

            //Demo Data
            //Item noIdItem = new()
            //{
            //    Name = "No Id Item"
            //};

            //noIdItem.Data["Flavor"] = "Cherry";
            //noIdItem.Data["Shape"] = "Tube";

            //await _apiReaderService.PostAsync(RequestUri, noIdItem);


            Task mainTask = Task.Run(async () =>
            {
                _items = await GetAllObjects();

                // TODO: Update GetAllObjects to get all custom objects by id
                //Item testItem = await GetObjectById(_itemRepository.GetIds().First());
                //_items = [testItem];

				_properties = GetAllProperties();
				//_userInteraction.PrintObjects(_items);
				
			});

			string choice = GetMenuChoice();

			await mainTask;

            if (string.Equals(choice, "search", StringComparison.OrdinalIgnoreCase))
            {
                if (_properties is not null)
                {
					_userInteraction.ListProperties(_properties);

					string targetProperty = _userInteraction.GetValidString("Enter a property to search for: ");

					FindPropertyByName(targetProperty);
				}
                else
                {
                    _userInteraction.DisplayText("No properties found to search for.");
                }
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

            var result = await _apiReaderService.ReadAsync<IEnumerable<Item>>(RequestUri);

            return result;
        }

        private async Task<Item> GetObjectById(string id)
        {
            string objectUri = RequestUri + $"/{id}";
			var result = await _apiReaderService.ReadAsync<Item>(objectUri);

			return result;
		}

        private HashSet<string> GetAllProperties()
        {
            HashSet<string> result = new (new StringComparerIgnoreCase());

            if (_items is null)
            {
                throw new InvalidOperationException("Items is null.");
            }

            foreach (Item item in _items)
            {
				IEnumerable<KeyValuePair<string, object>> properties = item.GetNonNullProperties();

                foreach (var property in properties)
                {
                    result.Add(property.Key);
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
				IEnumerable<KeyValuePair<string, object>> properties = item.GetNonNullProperties();

                foreach (var property in properties)
                {
                    if (property.Key.Contains(target, StringComparison.OrdinalIgnoreCase))
                    {
                        string propertyName = property.Key;
                        object propertyValue = property.Value;

                        _userInteraction.DisplayText(
                            $"Found an item with target property: {item.Name} with {propertyName}: {propertyValue}");
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
                return item.Data;
            }

            throw new InvalidOperationException($"Target instance must be {nameof(Data)} or {nameof(Item)}");
        }
    }
}
