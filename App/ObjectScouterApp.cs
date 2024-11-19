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

        const string ExitOption = "Exit";

        private readonly IApiReaderService _apiReaderService;

		private HashSet<string>? _propertyNames;

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
                { "Create", HandleCreateItem },
                { "Search", HandleSearch },
                { "List", HandleListItems },
                { "Delete", HandleDeleteItem },
                { ExitOption, HandleExit }
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

			Task mainTask = MainTask();

			string choice;
			do
			{
				choice = GetMenuChoice();

				await AwaitTaskIfPending(mainTask);

				HandleMenuChoice(choice);
			}
			while (!string.Equals(choice, ExitOption, StringComparison.OrdinalIgnoreCase));
		}

		private Task MainTask()
		{
			Task? mainTask = Task.Run(async () =>
			{
				_items = await GetAllObjects();

				// TODO: Update GetAllObjects to get all custom objects by id
				//Item testItem = await GetObjectById(_itemRepository.GetIds().First());
				//_items = [testItem];

				_propertyNames = GetAllProperties();
			});
			return mainTask;
		}

		private static async Task AwaitTaskIfPending(Task? task)
		{
			if (task is not null)
			{
				await task;
				task = null;
				_ = task?.Status; // Needed to satisfy compiler, no function.
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

		private void HandleCreateItem()
		{
			//Demo Data
			/*
            Item cherryTube = new()
            {
				Name = "Cherry Tubes"
            };

            cherryTube.Data["flavor"] = "cherry";
            cherryTube.Data["shape"] = "tube";

            await _apiReaderService.PutAync(RequestUri, cherryTube);
            */
		}

		private void HandleSearch()
        {
			if (_propertyNames is not null)
			{
				_userInteraction.ListStrings(_propertyNames);

				string targetName = _userInteraction.GetValidString(
                    $"Enter a property to search for:{Environment.NewLine}");

                IEnumerable<string> validPropertiesValues = GetValuesOfAllMatchingProperties(targetName);

                _userInteraction.ListStrings(validPropertiesValues);

                string targetValue = _userInteraction.GetValidString(
                    $"Enter a value to search all {targetName} properties for:{Environment.NewLine}");

                FindPropertiesByValue(targetValue);
			}
			else
			{
				_userInteraction.DisplayText(
                    $"No properties found to search for.{Environment.NewLine}");
			}
		}

		private async void HandleDeleteItem()
		{
            string userInput;
            string cancelCommand = "cancel";

            bool complete;
            do
            {
                userInput = _userInteraction.GetValidString(
                $"Enter ID of object to delete, or type 'Cancel':{Environment.NewLine}");

                if (string.Equals(userInput, cancelCommand, StringComparison.OrdinalIgnoreCase))
                {
                    complete = true;
                }
                else
                {
					complete = await _itemRepository.RemoveId(userInput);

					if (!complete)
					{
						_userInteraction.DisplayText(
                            $"Item ID not found. Please try again.{Environment.NewLine}");
					}
                    else if (complete)
                    {
						_userInteraction.DisplayText(
                            $"ID {userInput} removed.{Environment.NewLine}");
					}
				}
            }
            while (!complete);

            // TODO: This is the same task from Run(), need to refactor this
			Task? mainTask = Task.Run(async () =>
			{
				_items = await GetAllObjects();
				_propertyNames = GetAllProperties();
			});
		}

		private void HandleListItems()
        {
			if (_items is not null)
			{
				_userInteraction.ListItems(_items);
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
				IEnumerable<KeyValuePair<string, object>> properties =
                    item.GetNonNullProperties();

                foreach (var property in properties)
                {
                    result.Add(property.Key);
                }
            }

            return result;
        }

        // TODO: Change method to first ask for property name, then property value to search for.
        // Have a blank entry for value act as method does now, return all items with the property.
        private void FindItemsByPropertyNames(string target)
        {
			if (_items is null)
			{
				throw new InvalidOperationException("No valid items to search.");
			}

			foreach (Item item in _items)
            {
				IEnumerable<KeyValuePair<string, object>> properties =
                    item.GetNonNullProperties();

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

		private IEnumerable<string> GetValuesOfAllMatchingProperties(string targetName)
		{
            // Go through all properties, if property name matches target then print value
            if (_items is null)
            {
                throw new InvalidOperationException($"{nameof(_items)} is null.");
            }

            List<string> result = [];

			foreach (Item item in _items)
			{
				IEnumerable<KeyValuePair<string, object>> properties = item.GetNonNullProperties();

				foreach (KeyValuePair<string, object> property in properties)
				{
					if (string.Equals(targetName, property.Key, StringComparison.OrdinalIgnoreCase))
					{
                        string foundValue = property.Value.ToString() ??
                            throw new InvalidOperationException($"{nameof(foundValue)} cannot be null.");
						result.Add(foundValue);
					}
				}
			}

            return [.. result];
		}

		private void FindPropertiesByValue(string target)
        {
			if (_items is null)
			{
				throw new InvalidOperationException("No valid items to search.");
			}

            foreach (Item item in _items)
            {
				IEnumerable<KeyValuePair<string, object>> properties = item.GetNonNullProperties();
                
                foreach (KeyValuePair<string, object> property in properties)
                {
                    if (string.Equals(target, property.Value.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
						_userInteraction.DisplayText(
							$"{item.Name} has matching {property.Key}: {property.Value}{Environment.NewLine}");
					}
                }
            }
		}
    }
}
