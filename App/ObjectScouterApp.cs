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
		const string CreateOption = "Create";
        const string SearchOption = "Search";
        const string ListOption = "List";
        const string DeleteOption = "Delete";
        const string ExitOption = "Exit";

        private readonly IUserInteraction _userInteraction;

        private readonly Dictionary<string, Func<Task>> _menuOptions;

		private readonly IMenuService _menuService;

		private readonly IItemRepository _itemRepository;

		private readonly IItemService _itemService;

		public AsyncApiApp(
        IUserInteraction userInteraction,
		IItemRepository itemRepository,
		IItemService itemService,
		IMenuService menuService)
		{
			_userInteraction = userInteraction;
			_menuService = menuService;
			_itemService = itemService;
			_itemRepository = itemRepository;

            _menuOptions = new Dictionary<string, Func<Task>>(StringComparer.OrdinalIgnoreCase)
            {
                { CreateOption, _menuService.HandleCreateItemAsync },
                { SearchOption, _menuService.HandleSearchAsync },
                { ListOption, _menuService.HandleListItemsAsync },
                { DeleteOption, _menuService.HandleDeleteItemAsync },
                { ExitOption, _menuService.HandleExitAsync }
            };
		}

		// This method does not really need to be async, this is just to demonstrate
		// that the ui thread does not have to be blocked while work is being done.
		// Artificial delays have been added to various modules to aid demonstration.
		public async Task Run()
		{
			string connectingMessage = $"Contacting database...{Environment.NewLine}";
			_userInteraction.DisplayText(connectingMessage);

			Task getDataTask = GetDataAsync();

			string choice;
			do
			{
				choice = GetMenuChoice();

				await AwaitTaskIfPendingAsync(getDataTask);

				await HandleMenuChoiceAsync(choice);
			}
			while (!string.Equals(choice, ExitOption, StringComparison.OrdinalIgnoreCase));
		}

        private Task GetDataAsync()
		{
			Task? mainTask = Task.Run(async () =>
			{
				await _itemRepository.LoadFromFileAsync();
				_itemService.Items = await _itemService.GetAllObjects();
				_itemService.PropertyNames = _itemService.GetAllProperties();
			});
			return mainTask;
		}

		private async Task AwaitTaskIfPendingAsync(Task? task)
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
