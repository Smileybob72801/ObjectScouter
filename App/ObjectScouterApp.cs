using ObjectScouter.Model;
using ObjectScouter.Services;
using ObjectScouter.UserInteraction;
using ObjectScouter.Helpers;
using System.Reflection;
using System.Text.Json;
using ObjectScouter.Repositories;

namespace ObjectScouter.App
{
	internal class AsyncApiApp
	{
        const string RequestUri = "/objects";

        private readonly IApiReaderService _apiReaderService;

		private HashSet<string>? _properties;

        private IEnumerable<Item>? _items;

        private readonly IUserInteraction _userInteraction;

		private readonly IItemRepository _itemRepository;

        private readonly Dictionary<string, Action> _menuOptions;

		public AsyncApiApp(
        IApiReaderService apiReaderService,
        IUserInteraction userInteraction,
        IItemRepository itemRepository)
		{
			_apiReaderService = apiReaderService;
			_userInteraction = userInteraction;
			_itemRepository = itemRepository;

            _menuOptions = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
            {
                { "Search", HandleSearch },
                { "List Items", HandleListItems },
                { "Exit", HandleExit },
            };
		}

		// This method does not really need to be async, this is just to demonstrate
		// that the ui thread does not have to be blocked while work is being done.
		// Artificial delays have been added to various modules to aid demonstration.
		public async Task Run()
        {
			// TODO: Move all awaited Tasks to another method so we can get to UI
			await _itemRepository.LoadFromFileAsync();

            string connectingMessage = $"Contacting database...{Environment.NewLine}";
			_userInteraction.DisplayText(connectingMessage);

            //Demo Data
            //Item noIdItem = new()
            //{
            //    Name = "No Id Item"
            //};

            //noIdItem.Data["Flavor"] = "Cherry";
            //noIdItem.Data["Shape"] = "Tube";

            //await _apiReaderService.PostAsync(RequestUri, noIdItem);


            Task? mainTask = Task.Run(async () =>
            {
                _items = await GetAllObjects();

                // TODO: Update GetAllObjects to get all custom objects by id
                //Item testItem = await GetObjectById(_itemRepository.GetIds().First());
                //_items = [testItem];

				_properties = GetAllProperties();
			});

            string choice;
            do
			{
				choice = GetMenuChoice();

				await AwaitTaskIfPending(mainTask);

				HandleMenuChoice(choice);
			}
			while (!string.Equals(choice, "exit", StringComparison.OrdinalIgnoreCase));
        }

		private static async Task AwaitTaskIfPending(Task? task)
		{
			if (task is not null)
			{
				await task;
				task = null;
				_ = task?.Status; // Line needed to satisfy compiler, no function.
			}
		}

		private void HandleMenuChoice(string choice)
        {
            if (_menuOptions.TryGetValue(choice, out Action? action))
            {
                action();
            }
            else
            {
                _userInteraction.DisplayText($"Invalid choice.{Environment.NewLine}");
            }
		}

        private void HandleSearch()
        {
			if (_properties is not null)
			{
				_userInteraction.ListProperties(_properties);

				string targetProperty = _userInteraction.GetValidString($"Enter a property to search for:{Environment.NewLine}");

				FindPropertyByName(targetProperty);
			}
			else
			{
				_userInteraction.DisplayText($"No properties found to search for.{Environment.NewLine}");
			}
		}

        private void HandleListItems()
        {
			if (_items is not null)
			{
				_userInteraction.PrintObjects(_items);
			}
			else
			{
				_userInteraction.DisplayText($"No items to display.{Environment.NewLine}");
			}
		}

        private void HandleExit()
        {
			_userInteraction.DisplayText($"Press any key to close application...");
            _userInteraction.WaitForAnyInput();
		}

        private string GetMenuChoice()
        {
            _userInteraction.DisplayText($"Choose an option: ");

            foreach (var option in _menuOptions)
            {
                _userInteraction.DisplayText($"{option.Key}");
            }

            _userInteraction.DisplayText("");

            string result = _userInteraction.GetValidString();

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

        // TODO: Change method to first ask for property name, then property value to search for.
        // Have a blank entry for value act as method does now, return all items with the property.
        private void FindPropertyByName(string target)
        {
			if (_items is null)
			{
				throw new InvalidOperationException("No valid items to search.");
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
                            $"Found an item, {item.Name}, with {propertyName}: {propertyValue}{Environment.NewLine}");
                    }
                }
            }
        }
    }
}
