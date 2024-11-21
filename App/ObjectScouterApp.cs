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

        const string CreateOption = "Create";
        const string SearchOption = "Search";
        const string ListOption = "List";
        const string DeleteOption = "Delete";
        const string ExitOption = "Exit";

        private readonly IApiService _apiService;

        private readonly IUserInteraction _userInteraction;

		private readonly IItemRepository _itemRepository;

        private readonly Dictionary<string, Func<Task>> _menuOptions;

        private readonly IItemService _itemService;

		public AsyncApiApp(
        IApiService apiService,
        IUserInteraction userInteraction,
        IItemRepository itemRepository,
        IItemService itemService)
		{
			_apiService = apiService;
			_userInteraction = userInteraction;
			_itemRepository = itemRepository;
            _itemService = itemService;

            _menuOptions = new Dictionary<string, Func<Task>>(StringComparer.OrdinalIgnoreCase)
            {
                { CreateOption, HandleCreateItemAsync },
                { SearchOption, HandleSearchAsync },
                { ListOption, HandleListItemsAsync },
                { DeleteOption, HandleDeleteItemAsync },
                { ExitOption, HandleExitAsync }
            };
		}

		// This method does not really need to be async, this is just to demonstrate
		// that the ui thread does not have to be blocked while work is being done.
		// Artificial delays have been added to various modules to aid demonstration.
		public async Task Run()
		{
			string connectingMessage = $"Contacting database...{Environment.NewLine}";
			_userInteraction.DisplayText(connectingMessage);

			Task mainTask = MainTask();

			string choice;
			do
			{
				choice = GetMenuChoice();

				await AwaitTaskIfPending(mainTask);

				await HandleMenuChoiceAsync(choice);
			}
			while (!string.Equals(choice, ExitOption, StringComparison.OrdinalIgnoreCase));
		}

		private Task MainTask()
		{
			Task? mainTask = Task.Run(async () =>
			{
				await _itemRepository.LoadFromFileAsync();
				_itemService.Items = await _itemService.GetAllObjects();
				_itemService.PropertyNames = _itemService.GetAllProperties();
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

		private async Task HandleMenuChoiceAsync(string choice)
        {
            if (_menuOptions.TryGetValue(choice, out Func<Task>? action))
            {
                await action();
            }
            else
            {
				_userInteraction.DisplayText($"Invalid choice.{Environment.NewLine}");
            }
		}

		private async Task HandleCreateItemAsync()
		{
			Item itemToAdd = new()
			{
				Name = _userInteraction.GetValidString(
				$"Enter the name for the new item:{Environment.NewLine}")
			};

			bool finishedAddingProperties;
            do
            {
                string nameOfPropertyToAdd = _userInteraction.GetValidString(
                    $"Enter new property name:{Environment.NewLine}");

                string valueOfPropertyToAdd = _userInteraction.GetValidString(
                    $"Enter value for {nameOfPropertyToAdd}:{Environment.NewLine}");

                itemToAdd.Data[nameOfPropertyToAdd] = valueOfPropertyToAdd;

                finishedAddingProperties = _userInteraction.GetYesOrNo(
                    $"Finished adding properties? Y or N?{Environment.NewLine}",
                    "Invalid response.");
            }
            while (!finishedAddingProperties);

			Task mainTask = MainTask();
			await AwaitTaskIfPending(mainTask);
		}

		private async Task HandleSearchAsync()
        {
			if (_itemService.PropertyNames is not null)
			{
				_userInteraction.ListStrings([.. _itemService.PropertyNames]);

				string targetName = _userInteraction.GetValidString(
                    $"Enter a property to search for:{Environment.NewLine}");

				string?[] validPropertiesValues = _itemService.GetValuesOfAllMatchingProperties(targetName);

                _userInteraction.ListStrings(validPropertiesValues);

                string targetValue = _userInteraction.GetValidString(
                    $"Enter a value to search all {targetName} properties for:{Environment.NewLine}");

                _itemService.FindPropertiesByValue(targetValue);
			}
			else
			{
				_userInteraction.DisplayText(
                    $"No properties found to search for.{Environment.NewLine}");
			}
		}
		private async Task HandleDeleteItemAsync()
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

			Task mainTask = MainTask();
			await AwaitTaskIfPending(mainTask);
		}

		private async Task HandleListItemsAsync()
        {
			if (_itemService.Items is not null)
			{
				_userInteraction.ListItems(_itemService.Items);
			}
			else
			{
				_userInteraction.DisplayText($"No items to display.{Environment.NewLine}");
			}
		}

        private async Task HandleExitAsync()
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
	}
}
